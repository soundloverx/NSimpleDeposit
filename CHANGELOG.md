# Changelog

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
