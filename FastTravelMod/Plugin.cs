using BepInEx;
using HarmonyLib;

namespace ErenshorFastTravel
{
    [BepInPlugin("com.noone.fasttravelspellbook", "Fixed Fast Travel Spellbook", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Fast Travel Spellbook Mod: Initializing...");
            Harmony harmony = new Harmony("com.noone.fasttravelspellbook");
            harmony.PatchAll();
            Logger.LogInfo("Fast Travel Spellbook Mod: Harmony patches applied.");
        }
    }
}