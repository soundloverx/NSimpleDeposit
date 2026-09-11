using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NSimpleDeposit
{
    internal static class ContainerService
    {
        // Container.CheckAccess(long) is private in the actual game assembly loaded at runtime -
        // compiling against a publicized reference assembly only satisfies the C# compiler, it does
        // not change the accessibility the CLR enforces against the real assembly, so a direct call
        // throws MethodAccessException. Call it via reflection instead, the same way
        // InventorySortService reaches Inventory's private Changed(bool,bool) method.
        private static readonly MethodInfo CheckAccessMethod = AccessTools.Method(typeof(Container), "CheckAccess", new[] { typeof(long) });
        internal static List<Container> GetNearbyContainers(Player player)
        {
            List<Container> containers = new List<Container>();

            if (player == null)
            {
                return containers;
            }

            Collider[] colliders = Physics.OverlapSphere(player.transform.position, Plugin.SearchRadius);
            HashSet<Container> foundContainers = new HashSet<Container>();

            foreach (Collider collider in colliders)
            {
                Container container = collider.GetComponentInParent<Container>();

                if (container == null)
                {
                    continue;
                }

                if (!foundContainers.Add(container))
                {
                    continue;
                }

                if (!IsAccessibleContainer(container))
                {
                    continue;
                }

                containers.Add(container);
            }

            return containers;
        }

        /// <summary>
        /// True if this is a player-built container (as opposed to a naturally generated/world
        /// container, e.g. dungeon loot) that the local player currently has access to. This is not
        /// limited to containers the local player personally built: any container inside a ward the
        /// player is permitted in (or not warded at all) counts, matching what the player could
        /// already do by opening the chest and dragging items in by hand.
        ///
        /// Mirrors the two gates the game itself applies in Container.Interact() (verified against
        /// the decompiled game code): a ward check via PrivateArea.CheckAccess() (only when the
        /// container actually opts into guard stone checking via m_checkGuardStone, same as vanilla),
        /// and the container's own private-vs-public setting via its internal CheckAccess(playerID),
        /// invoked via <see cref="CheckAccessMethod"/> since it's private at runtime. A container
        /// inside a ward the player lacks permission for, or one explicitly set to Private by someone
        /// else, is excluded either way - this never grants access to anything the player couldn't
        /// already open and use by hand.
        /// </summary>
        internal static bool IsAccessibleContainer(Container container)
        {
            if (container == null)
            {
                return false;
            }

            Piece piece = container.GetComponentInParent<Piece>();

            if (piece == null)
            {
                return false;
            }

            if (container.m_checkGuardStone && !PrivateArea.CheckAccess(container.transform.position, 0f, false, true))
            {
                return false;
            }

            if (CheckAccessMethod == null)
            {
                return false;
            }

            long playerId = Game.instance.GetPlayerProfile().GetPlayerID();

            return (bool)CheckAccessMethod.Invoke(container, new object[] { playerId });
        }

        /// <summary>
        /// Claims network (ZDO) ownership of the container if the local player does not already
        /// have it, and returns whether the local player now owns it (and is therefore safe to write
        /// to). Containers found via a proximity scan (Quick Stack, background sort) never go through
        /// the vanilla RPC_RequestOpen/RPC_OpenResponse handshake that normally establishes authority
        /// over a chest.
        ///
        /// This matters because Container's own change handling (verified against the decompiled
        /// game code) only persists a change if the local peer is the ZDO owner:
        ///   private void OnContainerChanged() { if (!m_loading && IsOwner()) Save(); }
        /// If we are not the owner, Inventory.AddItem() still succeeds locally, but Save() is never
        /// called, so nothing is written to the ZDO. The container reloads from the ZDO's actual
        /// (unchanged) data on its next periodic CheckForChanges() tick (every ~1s), silently
        /// reverting our in-memory addition - the item vanishes from the player's inventory without
        /// ever having actually been saved into the chest.
        ///
        /// The fix mirrors vanilla's own Container.RPC_TakeAllResponse, which is the one vanilla code
        /// path that already does "claim a container found through an RPC exchange, then immediately
        /// write to it": it calls ClaimOwnership() and then ForceSendZDO() to the previous owner
        /// before touching the inventory, so that peer is told about the ownership change right away
        /// instead of only finding out on the next periodic ZDO sync.
        ///
        /// Ownership is deliberately NOT claimed while another peer currently has the container open
        /// (ZDOVars.s_inUse), mirroring the IsInUse() check vanilla's own RPC_RequestOpen performs
        /// before granting access - Container.SetInUse() only ever updates the ZDO's "in use" flag
        /// while called by the current owner, so this flag is reliable evidence someone else has it
        /// open even though our own local, never-updated Container.m_inUse field is not. Yanking
        /// ownership away from a player mid-session would silently break their own unsaved edits the
        /// same way it broke ours before this fix: Container.OnContainerChanged() would stop calling
        /// Save() for them the moment they lose ownership.
        /// </summary>
        internal static bool EnsureOwnership(Container container)
        {
            if (container == null)
            {
                return false;
            }

            ZNetView nview = container.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            if (nview.IsOwner())
            {
                return true;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo.GetInt(ZDOVars.s_inUse) == 1)
            {
                return false;
            }

            long previousOwner = zdo.GetOwner();

            nview.ClaimOwnership();

            if (previousOwner != 0L && ZDOMan.instance != null)
            {
                ZDOMan.instance.ForceSendZDO(previousOwner, zdo.m_uid);
            }

            return true;
        }
    }
}
