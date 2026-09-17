# NSimpleDeposit

A private mod for personal use with friends - not published on Thunderstore.

Automatically store items from your inventory into nearby chests you have access to, within a configurable radius. Also sorts your inventory and any chest you open, and lets you lock item types so they're never auto-deposited.

Originally forked from Hex_Viking's **HexQuickStackStorage** (itself inspired by Goldenrevolver's **Quick Stack Store Sort Trash**), then diverged enough - dropped the trash/delete feature, added item locking, and access to any chest you have rights to rather than only ones you built - to become its own mod.

## Instructions

### Quick Stack

Click the **Q** button in your inventory or use the configured Quick Stack keyboard shortcut (default `P`).

Items will only be moved if:

- The item is not equipped
- The item is not in your hotbar
- The item is not marked as **locked**
- The item is inside the normal player inventory
- A nearby chest you have access to already contains that item type
- The chest has at least some room for the item
- Nobody else currently has that chest open

Items will not be stored in empty chests or chests that do not already contain that item.

If a chest only has room for part of a stack (e.g. topping off an existing stack of 35/50), that partial amount is deposited and the rest stays in your inventory, rather than skipping the item entirely.

When triggered via the keyboard shortcut, a message is shown summarizing what happened (the **Q** button doesn't show this, since you can already see the result in the inventory UI):

- `All items deposited` - everything that could be moved was moved.
- `x/y items deposited` - only part of what you carried was moved. `y` is every item you held that was worth considering, including ones that don't yet exist in any nearby chest; `x` is how many of those actually got deposited.
- `No room for items` - some of your items already exist in a nearby chest, but none of those chests had space.
- `No matching containers` - none of your items exist in any nearby chest yet. Quick Stack only tops off an item type a chest already holds; it never seeds a chest with a type it doesn't have.
- `No available containers` - no accessible chest was found within the search radius.
- Nothing is shown when there's nothing in your inventory worth considering at all (e.g. it's empty, or everything left is equipped or locked).

### Sort

Click the **S** button to sort your inventory.

If a chest is currently open, the chest will also be sorted, and any player-built chest you have access to is also sorted automatically the moment you open it.

The player's hotbar, equipped items, and locked items will not be moved.

Partial stacks of the same item type and quality are combined into as few stacks as possible first (e.g. two stacks of 10 Wood become one stack of 20), freeing up slots before the remaining items are placed. Equipped items, hotbar items, and locked item types are never topped up or drained by this - they stay exactly as they are.

### Lock Items

Hold **Alt** and **Right Click** an item to mark that item type as locked.

Locked items will have a blue border around them.

Locked items are:

- Skipped by Quick Stack, so they will never be automatically moved into a chest
- Left in place when sorting, just like equipped and hotbar items

Alt + Right Click the item again to remove the lock.

Once an item type is marked as locked, newly picked up items of that same type will also be locked.

Lock selections are saved per character (stored in that character's own save data, alongside things like skills and known recipes), so they follow that character into any world but are not shared with your other characters.

### Build & Craft From Containers

While building pieces or crafting at a workbench/station, materials missing from your own inventory are automatically pulled from nearby chests you have access to - the same range and access rules as Quick Stack apply (see [Container Access](#container-access)).

Feeding fuel, ore, ingredients, or ammo also pulls from nearby chests when your own inventory doesn't have any:

- Fireplaces (campfires, hearths, bonfires) - fuel
- Smelters, charcoal kilns, and blast furnaces - ore and fuel
- Cooking stations (cooking station, iron cooking station, stone oven, and similar) - fuel and the food being cooked
- Turrets - ammunition

The crafting/building requirement panel always shows the combined inventory + nearby-container amount (e.g. `200/4` if you need 4 wood and have 200 spread across your inventory and nearby chests combined), instead of vanilla's plain required-amount display. It's highlighted when nearby containers are what's making up the difference.

Holding the fill-all modifier key (default **Left Shift**) while interacting with a fireplace/light, a smelter/kiln, or a cooking station's fuel switch fills it to capacity in one go - fuel or ore is pulled from your inventory first, then nearby containers, instead of adding one unit per interaction. A hover-text hint appears on these objects showing the key when using it would do something.

## Container Access

Quick Stack and chest sorting work with any player-built container you currently have access to, not only ones you personally built.

A container inside another player's ward that you aren't permitted in is skipped, the same way opening it by hand would be blocked. A container in your own ward, in a ward you're permitted in, or with no ward at all, is fair game. A chest someone has explicitly set to Private is also skipped unless you're its creator, again matching normal chest rules.

A chest another player currently has open is left alone entirely for that pass, rather than claiming it out from under them.

Naturally generated/world containers (e.g. dungeon loot) are always ignored.

## Multiplayer

- Client-side mod
- Does not have server sync
- Access to a container is checked using the same ward and privacy rules the base game uses
- Writing to a container claims network ownership of it first (mirroring how the game's own "Take All" does it), so items don't get silently lost when writing into a chest another peer currently owns

## Configuration

Configuration options include:

- Search radius (shared by Quick Stack and Build & Craft From Containers)
- Quick Stack keyboard shortcut
- Fill-all modifier key (default Left Shift)
