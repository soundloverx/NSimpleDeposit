# Changelog

## v1.0.0
- Initial release, forked from HexQuickStackStorage v1.0.0 as its own mod.
- Quick Stack: automatically move matching items from your inventory into nearby accessible chests.
- Sort: sorts your own inventory, plus any player-built chest you open or have currently open, in place. Partial stacks of the same item type and quality are merged into as few stacks as possible first, freeing up slots.
- Lock: hold Alt and Right Click an item to mark that item type as locked (blue border) so it's skipped by Quick Stack and stays put when sorting.
- Lock selections are stored per character instead of globally for the whole BepInEx install. Locks live in the character's own save data (the same place the game keeps skills and known recipes), so they follow that character into any world but no longer leak into other characters on the same machine.
- Chest access follows the game's own rules: ward permissions and a chest's Public/Private/Group setting, not just who built it.
- Writing to a chest claims network ownership of it first, mirroring how the game's own "Take All" does it, so items can't be silently lost by writing into a chest another peer currently owns. A chest someone else currently has open is left alone entirely rather than claimed out from under them.
