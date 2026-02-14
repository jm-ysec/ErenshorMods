using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace MovementSpeedManager
{
    [BepInPlugin("com.noone.movementspeedmanager", "Movement Speed Manager", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        // Configuration entries
        public static ConfigEntry<float> GlobalSpeed;
        public static ConfigEntry<float> PlayerSpeed;
        public static ConfigEntry<float> NPCSpeed;
        public static ConfigEntry<float> SimPlayerSpeed;
        public static ConfigEntry<float> GroupedSimPlayerSpeed;

        private void Awake()
        {
            Logger.LogInfo("Movement Speed Manager: Initializing...");

            // Initialize configuration
            GlobalSpeed = Config.Bind("Movement Speed", "GlobalSpeed", 6f,
                "Global movement speed for all entities (default: 6)");

            PlayerSpeed = Config.Bind("Movement Speed", "PlayerSpeed", -1f,
                "Player movement speed. Set to -1 to use GlobalSpeed (default: -1)");

            NPCSpeed = Config.Bind("Movement Speed", "NPCSpeed", -1f,
                "NPC and monster movement speed. Set to -1 to use GlobalSpeed (default: -1)");

            SimPlayerSpeed = Config.Bind("Movement Speed", "SimPlayerSpeed", -1f,
                "SimPlayer movement speed. Set to -1 to use GlobalSpeed (default: -1)");

            GroupedSimPlayerSpeed = Config.Bind("Movement Speed", "GroupedSimPlayerSpeed", -1f,
                "SimPlayer movement speed when grouped with player. Set to -1 to use SimPlayerSpeed or GlobalSpeed (default: -1)");

            Logger.LogInfo("Movement Speed Manager: Configuration loaded.");

            // Apply Harmony patches
            Harmony harmony = new Harmony("com.noone.movementspeedmanager");
            harmony.PatchAll();
            Logger.LogInfo("Movement Speed Manager: Harmony patches applied.");
        }
    }
}

