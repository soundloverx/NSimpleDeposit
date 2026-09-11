using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NSimpleDeposit
{
    internal static class ContainerService
    {
        // Container.CheckAccess(long) is private in the actual game assembly loaded at runtime
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
