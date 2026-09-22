namespace NSimpleDeposit
{
    // Station auto-supply features (smelter ore/fuel, fireplace fuel, cooking station fuel) should never
    // spend a locked item type out of the player's own inventory, but a locked type must still be pullable
    // from nearby containers - a chest full of the same material isn't what the player locked. Building
    // and manual crafting are deliberately exempt: those are expected to use locked inventory items.
    internal static class InventoryLockService
    {
        internal static bool HaveUnlockedItem(Inventory inventory, string itemName)
        {
            return !LockService.IsLocked(itemName) && inventory.HaveItem(itemName);
        }

        internal static int CountUnlockedItems(Inventory inventory, string itemName)
        {
            return LockService.IsLocked(itemName) ? 0 : inventory.CountItems(itemName);
        }
    }
}
