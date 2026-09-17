using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace NSimpleDeposit
{
    internal static class InputService
    {
        // BepInEx's own KeyboardShortcut.IsPressed()/IsDown() is meant to be polled once per Update
        // frame and can behave unreliably when checked from inside a Harmony patch instead, so this
        // checks the raw key state directly via the game's own input wrapper (same approach other
        // Valheim mods use for this reason; the game reads input through ZInput, not UnityEngine.Input).
        internal static bool IsHeld(KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None
                && ZInput.GetKey(shortcut.MainKey, false)
                && shortcut.Modifiers.All(key => ZInput.GetKey(key, false));
        }
    }
}
