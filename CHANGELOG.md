# Changelog

## v1.2.4
- Quick Stack and Hotbar Swap hotkeys now work while moving or running. Previously, holding any other key (such as movement keys or Shift) caused the hotkey press to be ignored.

## v1.2.3
- Shield generators now pull fuel (any of the bone types they accept) from nearby chests when your inventory has none, and support the fill-all modifier (default Left Shift) to fill them to capacity from your inventory first, then nearby chests. As with the other stations, a locked bone type in your own inventory is never spent automatically.
- Refuelling a fireplace/light, smelter/kiln/blast furnace, cooking station, or shield generator now shows a message when nothing could be added: `Inventory items locked` if the only fuel available is a locked item type in your inventory, or `Unable to find fuel` if there is none in your inventory or nearby chests. This replaces the game's own "out of"/"don't have any" message in that situation.

## v1.2.2
- Fireplaces, cooking stations, and smelters/kilns/blast furnaces no longer spend a locked item type out of your own inventory when auto-adding fuel or ore (including with the fill-all modifier) - a locked item type sitting in your inventory is left alone. The same item type can still be pulled from nearby chests, so a chest full of it is still used automatically; only your own carried, locked stack is protected. Building pieces and manual crafting are unaffected and still use locked inventory items as normal.

## v1.2.1
- Smelters, kilns, and blast furnaces: the fill-all modifier now uses ore from your inventory before pulling any from nearby chests, across all ore types the station accepts. Previously each ore type was handled in turn, so chest ore of an earlier type could fill the queue while a later type (e.g. a full stack of tin) was still sitting in your inventory.

## v1.2.0
- Hotbar Swap: new keyboard shortcut (default backtick, configurable as `HotbarSwapShortcut`) that swaps the first inventory row (your hotbar) with the second row, so you can flip between two hotbar setups (e.g. tools and weapons) without opening the inventory. Only those two rows are swapped. Locked and equipped items simply move with their row, and a `Hotbar swapped` message is shown so an accidental press is noticeable. The shortcut is ignored while a text field is focused.
- Sort: locks now only apply to your own inventory. Sorting a chest sorts every item in it, including item types you have locked.

## v1.1.1
- Cooking stations (cooking station, iron cooking station, and similar) no longer pull the food to cook from nearby chests. Interacting to collect finished food also triggered that pull, so each collection handed you a stray raw item from a chest. Fuel is still pulled from chests, including with the fill-all modifier.
- Build & Craft From Containers: no longer pays a second time when another mod that also pulls from nearby containers has already paid for the build/craft, which could consume double the materials.
- Quick Stack hotkey: no longer stays unresponsive until the inventory is opened after closing a text dialog (such as naming a portal); it now only ignores the hotkey while a text field is actually focused.
- Added log lines for Quick Stack results and for consumed build/craft materials to help diagnose issues.

## v1.1.0
- Build & Craft From Containers: building pieces, crafting recipes, and feeding fireplaces/lights, smelters (smelter, charcoal kiln, blast furnace), cooking stations (cooking station, iron cooking station, stone oven, and any other variant of the same object), and turrets can now pull missing materials from nearby accessible chests, using the same search radius and access rules as Quick Stack.
  - The crafting/building requirement panel now always shows the combined inventory + nearby-container amount (e.g. `200/4`) instead of just the required amount, and highlights it when containers are what's making up the difference.
  - Holding the new fill-all modifier key (default Left Shift) while using a fireplace/light or a smelter/kiln/cooking station fills its fuel to capacity from your inventory and nearby containers in one interaction, instead of adding one unit at a time. A hover-text hint shows the key when it would do something.
- Quick Stack: the "nothing to top off" message was reworded from `Cannot auto-deposit new items` to `No matching containers`.

## v1.0.2
- Quick Stack (keyboard shortcut only): now shows an on-screen message summarizing what happened, since pressing the hotkey outside your inventory previously gave no feedback:
  - `All items deposited` - everything that could be moved was moved.
  - `x/y items deposited` - only part of what you carried was moved; `y` counts every item you held that was eligible to be considered (including ones that never matched any nearby chest), `x` counts how many of those units actually got deposited.
  - `No room for items` - one or more of your items already exist in a nearby chest, but none of those chests had space.
  - `Cannot auto-deposit new items` - none of your eligible items exist in any nearby chest yet (Quick Stack only tops off item types a chest already holds, it never seeds a chest with a new item type).
  - `No available containers` - no accessible chest was found within the search radius.
  - No message at all when there's nothing in your inventory worth considering (e.g. it's empty, or everything left is equipped/locked).

## v1.0.1
- Quick Stack: a stack that only partially fits (e.g. topping off an existing 35/50 stack) is now topped off instead of being skipped entirely. Previously, an item was only moved if the whole carried stack could fit; now it deposits as much as the chest has room for and leaves the remainder in your inventory.

## v1.0.0
- Initial release, forked from HexQuickStackStorage v1.0.0 as its own mod.
- Quick Stack: automatically move matching items from your inventory into nearby accessible chests.
- Sort: sorts your own inventory, plus any player-built chest you open or have currently open, in place. Partial stacks of the same item type and quality are merged into as few stacks as possible first, freeing up slots.
- Lock: hold Alt and Right Click an item to mark that item type as locked (blue border) so it's skipped by Quick Stack and stays put when sorting.
- Lock selections are stored per character instead of globally for the whole BepInEx install. Locks live in the character's own save data (the same place the game keeps skills and known recipes), so they follow that character into any world but no longer leak into other characters on the same machine.
- Chest access follows the game's own rules: ward permissions and a chest's Public/Private/Group setting, not just who built it.
- Writing to a chest claims network ownership of it first, mirroring how the game's own "Take All" does it, so items can't be silently lost by writing into a chest another peer currently owns. A chest someone else currently has open is left alone entirely rather than claimed out from under them.
