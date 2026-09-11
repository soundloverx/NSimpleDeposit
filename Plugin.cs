using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NSimpleDeposit
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "NSimpleDeposit";
        private const string PluginName = "NSimpleDeposit";
        private const string PluginVersion = "1.0.2";

        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Log { get; private set; }

        private ConfigEntry<float> _searchRadius;
        private ConfigEntry<KeyboardShortcut> _quickStackShortcut;
        private Harmony _harmonyInstance;

        internal static float SearchRadius => Instance?._searchRadius != null ? Instance._searchRadius.Value : 25f;
        internal static KeyboardShortcut QuickStackShortcut => Instance?._quickStackShortcut != null ? Instance._quickStackShortcut.Value : new KeyboardShortcut(KeyCode.P);

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            _searchRadius = Config.Bind(
                "General",
                "SearchRadius",
                25f,
                new ConfigDescription(
                    "The radius in which to search for nearby containers when quick stacking.",
                    new AcceptableValueRange<float>(5f, 150f)
                )
            );

            _quickStackShortcut = Config.Bind(
                "General",
                "QuickStackShortcut",
                new KeyboardShortcut(KeyCode.P),
                "Keyboard shortcut used to quick stack nearby containers."
            );

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll(assembly);

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void Update()
        {
            if (IsTypingInInputField())
            {
                return;
            }

            if (!QuickStackShortcut.IsDown())
            {
                return;
            }

            Player player = Player.m_localPlayer;

            if (player == null)
            {
                return;
            }

            QuickStackResult result = QuickStackService.QuickStack(player);
            ShowQuickStackResultMessage(player, result);
        }

        private static void ShowQuickStackResultMessage(Player player, QuickStackResult result)
        {
            switch (result.Outcome)
            {
                case QuickStackOutcome.NoContainersFound:
                    player.Message(MessageHud.MessageType.Center, "No available containers");
                    break;

                case QuickStackOutcome.NothingToDeposit:
                    break;

                case QuickStackOutcome.NoMatchingItems:
                    player.Message(MessageHud.MessageType.Center, "Cannot auto-deposit new items");
                    break;

                case QuickStackOutcome.NoRoomForItems:
                    player.Message(MessageHud.MessageType.Center, "No room for items");
                    break;

                case QuickStackOutcome.ItemsDeposited:
                    if (result.Deposited >= result.TotalEligible)
                    {
                        player.Message(MessageHud.MessageType.Center, "All items deposited");
                    }
                    else
                    {
                        player.Message(MessageHud.MessageType.Center, $"{result.Deposited}/{result.TotalEligible} items deposited");
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            _harmonyInstance?.UnpatchSelf();

            Log?.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            Instance = null;
        }

        private static bool IsTypingInInputField()
        {
            if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            {
                return false;
            }

            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            return selectedObject.GetComponent<InputField>() != null || selectedObject.GetComponent<TMP_InputField>() != null;
        }
    }
}
