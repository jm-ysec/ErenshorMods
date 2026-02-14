using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace RealWorldClock
{
    [BepInPlugin("com.noone.realworldclock", "Real World Clock", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        // Configuration entries
        public static ConfigEntry<bool> ShowClock;
        public static ConfigEntry<string> TimeFormat;
        public static ConfigEntry<int> FontSize;
        public static ConfigEntry<float> PosX;
        public static ConfigEntry<float> PosY;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("Real World Clock: Initializing...");

            // Initialize configuration
            ShowClock = Config.Bind("Clock", "ShowClock", true,
                "Show the real-world clock");
            TimeFormat = Config.Bind("Clock", "TimeFormat", "HH:mm:ss",
                "C# DateTime format string");
            FontSize = Config.Bind("Clock", "FontSize", 24,
                "Clock font size");
            PosX = Config.Bind("Clock", "PositionX", 10f,
                "Saved X position");
            PosY = Config.Bind("Clock", "PositionY", 10f,
                "Saved Y position");

            // Create controller GameObject using proven pattern
            GameObject controllerGO = new GameObject("RealWorldClockController");
            controllerGO.transform.SetParent(null);
            controllerGO.hideFlags = HideFlags.HideAndDontSave;
            controllerGO.AddComponent<RealWorldClockController>();
            Object.DontDestroyOnLoad(controllerGO);

            Logger.LogInfo("Real World Clock: Initialized successfully.");
        }

        public void SaveConfig()
        {
            Config.Save();
        }
    }
}

