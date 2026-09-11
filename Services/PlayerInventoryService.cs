using UnityEngine;

namespace NSimpleDeposit
{
    /// <summary>
    /// Shared helper for reasoning about the player's own vanilla inventory grid, as opposed to any
    /// extra rows added by other inventory-size mods. Quick Stack, Sort, and locking all need this
    /// same row count to stay correctly scoped, so it lives in one place instead of being
    /// recalculated by each of them.
    /// </summary>
    internal static class PlayerInventoryService
    {
        internal static int GetVanillaHeight(Player player)
        {
            if (player.TryGetUniqueKeyValue(Player.InventoryRowsKey, out string value) && int.TryParse(value, out int rows))
            {
                return Mathf.Clamp(rows, 0, 9);
            }

            return 4;
        }
    }
}
