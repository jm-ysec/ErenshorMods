using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace BetterStatsPage
{
    [BepInPlugin("com.noone.betterstatspage", "Better Stats Page", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        // Configuration entries
        public static ConfigEntry<KeyCode> ToggleKey;
        public static ConfigEntry<Color> ActiveTabColor;
        public static ConfigEntry<Color> InactiveTabColor;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("Better Stats Page: Initializing...");

            // Initialize configuration
            ToggleKey = Config.Bind("Controls", "ToggleKey", KeyCode.P,
                "Key to open/close the stats window");
            ActiveTabColor = Config.Bind("Appearance", "ActiveTabColor",
                new Color(0.35f, 0.59f, 1f), "Color for active tab");
            InactiveTabColor = Config.Bind("Appearance", "InactiveTabColor",
                new Color(0.7f, 0.7f, 0.7f), "Color for inactive tab");

            // Create controller GameObject following the proven pattern from AreaMapMod
            // Create it here in Plugin.Awake(), set HideFlags, then DontDestroyOnLoad
            GameObject controllerGO = new GameObject("BetterStatsPageController");
            controllerGO.transform.SetParent(null); // Ensure root object
            controllerGO.hideFlags = HideFlags.HideAndDontSave;
            controllerGO.AddComponent<BetterStatsPageController>();
            Object.DontDestroyOnLoad(controllerGO);

            Logger.LogInfo("Better Stats Page: Initialized successfully.");
        }
    }
}

