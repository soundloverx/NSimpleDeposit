using System.Collections.Generic;
using System.IO;

namespace NSimpleDeposit
{
    /// <summary>
    /// Tracks which item types the player has marked as locked (never auto-deposited, left in place
    /// when sorting). Locks are stored per character, not per BepInEx install: they live directly in
    /// the vanilla <see cref="Player.m_customData"/> dictionary, the same general-purpose string
    /// key/value store the base game already saves and loads as part of that character's own data
    /// (verified against the decompiled game code - Player.Save/Load read and write it via ZPackage
    /// alongside everything else on the character, independent of which world it's used in). This
    /// means a lock follows a specific character everywhere it goes, exactly like the character's
    /// skills or known recipes, rather than being shared globally across every character on the
    /// machine the way a BepInEx config-folder file would be.
    ///
    /// Each locked item type is stored as its own dictionary entry (key: <see cref="LockedKeyPrefix"/>
    /// + item name, value: "1") rather than one entry containing every name, so no custom
    /// serialization format is needed and no explicit save call is required - the entry rides along
    /// with the next time the game itself saves the character.
    /// </summary>
    internal static class LockService
    {
        private const string LockedKeyPrefix = "NSimpleDeposit.Locked:";
        private const string LegacyMigratedKey = "NSimpleDeposit.LegacyLocksMigrated";
        private const string LegacyLockedFileName = "NSimpleDeposit.locked.txt";

        internal static bool IsLocked(ItemDrop.ItemData item)
        {
            if (item == null || item.m_shared == null)
            {
                return false;
            }

            Player player = Player.m_localPlayer;

            if (player == null || player.m_customData == null)
            {
                return false;
            }

            return player.m_customData.ContainsKey(LockedKeyPrefix + item.m_shared.m_name);
        }

        internal static void ToggleLocked(Player player, ItemDrop.ItemData item)
        {
            if (player == null || item == null || item.m_shared == null || player.m_customData == null)
            {
                return;
            }

            if (!IsVanillaInventoryItem(player, item))
            {
                return;
            }

            string key = LockedKeyPrefix + item.m_shared.m_name;

            if (!player.m_customData.Remove(key))
            {
                player.m_customData[key] = "1";
            }
        }

        internal static bool IsVanillaInventoryItem(Player player, ItemDrop.ItemData item)
        {
            if (player == null || item == null)
            {
                return false;
            }

            int vanillaHeight = PlayerInventoryService.GetVanillaHeight(player);

            return item.m_gridPos.y >= 0 && item.m_gridPos.y < vanillaHeight;
        }

        internal static void MigrateLegacyLocksIfNeeded(Player player)
        {
            if (player == null || player.m_customData == null)
            {
                return;
            }

            if (player.m_customData.ContainsKey(LegacyMigratedKey))
            {
                return;
            }

            string legacyPath = Path.Combine(BepInEx.Paths.ConfigPath, LegacyLockedFileName);

            if (File.Exists(legacyPath))
            {
                foreach (string line in File.ReadAllLines(legacyPath))
                {
                    string itemName = line.Trim();

                    if (!string.IsNullOrEmpty(itemName))
                    {
                        player.m_customData[LockedKeyPrefix + itemName] = "1";
                    }
                }
            }

            player.m_customData[LegacyMigratedKey] = "1";
        }
    }
}
