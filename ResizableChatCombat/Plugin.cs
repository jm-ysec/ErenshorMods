using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace ResizableChatCombat
{
    [BepInPlugin("com.noone.resizablechatcombat", "Resizable Chat Combat", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        // Chat window config
        public static ConfigEntry<float> ChatWidth;
        public static ConfigEntry<float> ChatHeight;

        // Combat window config
        public static ConfigEntry<float> CombatWidth;
        public static ConfigEntry<float> CombatHeight;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("Resizable Chat Combat: Initializing...");

            // Bind config entries
            ChatWidth = Config.Bind("Chat Window", "Width", 350f,
                new ConfigDescription("Width of the chat window", new AcceptableValueRange<float>(200f, 800f)));
            ChatHeight = Config.Bind("Chat Window", "Height", 200f,
                new ConfigDescription("Height of the chat window", new AcceptableValueRange<float>(100f, 600f)));

            CombatWidth = Config.Bind("Combat Window", "Width", 350f,
                new ConfigDescription("Width of the combat window", new AcceptableValueRange<float>(200f, 800f)));
            CombatHeight = Config.Bind("Combat Window", "Height", 150f,
                new ConfigDescription("Height of the combat window", new AcceptableValueRange<float>(80f, 400f)));

            // Create controller using proven pattern
            GameObject controllerGO = new GameObject("ResizableChatCombatController");
            controllerGO.transform.SetParent(null);
            controllerGO.hideFlags = HideFlags.HideAndDontSave;
            controllerGO.AddComponent<ResizableChatCombatController>();
            Object.DontDestroyOnLoad(controllerGO);

            Logger.LogInfo("Resizable Chat Combat: Initialized successfully.");
        }

        public void SaveConfig()
        {
            Config.Save();
        }
    }
}

