using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DruidSpells
{
    [BepInPlugin("com.noone.druidspells", "Druid Spells", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        /// <summary>
        /// Expose logger for other classes to use.
        /// </summary>
        public new ManualLogSource Logger => base.Logger;

        // =======================================================================
        // Optional Features - Non-spell gameplay modifications
        // =======================================================================

        /// <summary>
        /// Percentage of movement speed buff that applies to backward movement cap.
        /// Base game caps backward speed at 7, regardless of buffs.
        /// At 50%, a +5 movement buff increases the backward cap from 7 to 9.5.
        /// </summary>
        public static ConfigEntry<float> BackwardSpeedBuffPercent;

        // =======================================================================
        // SimPlayer SoW Request Behavior
        // =======================================================================

        /// <summary>
        /// Enable SimPlayers to request Spirit of the Wolf buffs via whisper/shout.
        /// </summary>
        public static ConfigEntry<bool> EnableSoWRequests;

        /// <summary>
        /// Base chance per tick (6 seconds) for a SimPlayer to request SoW via whisper.
        /// Modified by zone size, dungeon status, and group status.
        /// </summary>
        public static ConfigEntry<float> SoWWhisperBaseChance;

        /// <summary>
        /// Base chance per tick for a SimPlayer to shout for SoW in zone chat.
        /// Modified by zone size, dungeon status, and group status.
        /// </summary>
        public static ConfigEntry<float> SoWShoutBaseChance;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("DruidSpells: Initializing...");

            // Bind config entries - Optional Features
            BackwardSpeedBuffPercent = Config.Bind(
                "Optional Features",
                "BackwardSpeedBuffPercent",
                50f,
                new ConfigDescription(
                    "Percentage of movement speed buff that applies to backward movement speed cap. " +
                    "Base game caps backward speed at 7. At 50%, a +5 buff increases cap to 9.5. " +
                    "Set to 0 to disable, 100 for full buff application.",
                    new AcceptableValueRange<float>(0f, 100f)));

            // Bind config entries - SimPlayer SoW Requests
            EnableSoWRequests = Config.Bind(
                "SimPlayer SoW Requests",
                "EnableSoWRequests",
                true,
                "Enable SimPlayers to request Spirit of the Wolf buffs via whisper and shout.");

            SoWWhisperBaseChance = Config.Bind(
                "SimPlayer SoW Requests",
                "WhisperBaseChance",
                0.5f,
                new ConfigDescription(
                    "Base chance (%) per tick for a lone SimPlayer to whisper you for SoW. " +
                    "Reduced in dungeons and when grouped. Set to 0 to disable whispers.",
                    new AcceptableValueRange<float>(0f, 10f)));

            SoWShoutBaseChance = Config.Bind(
                "SimPlayer SoW Requests",
                "ShoutBaseChance",
                0.2f,
                new ConfigDescription(
                    "Base chance (%) per tick for a lone SimPlayer to shout for SoW in zone chat. " +
                    "Reduced in dungeons and when grouped. Set to 0 to disable shouts.",
                    new AcceptableValueRange<float>(0f, 10f)));

            // Register all spell definitions (before SpellDB.Start runs)
            DruidSpellsMod.RegisterAllSpells();

            // Apply Harmony patches:
            // - SpellDB_Start_Patch: injects custom spells into SpellDB
            // - SpellMessageFixer: fixes "did not take hold" message display
            // - BackwardSpeedCapFixer: applies movement buff to backward speed cap
            Harmony harmony = new Harmony("com.noone.druidspells");
            harmony.PatchAll();

            Logger.LogInfo("DruidSpells: Plugin loaded.");
        }

        private void Update()
        {
            // F9 = Debug dump of status effects and movement speed
            if (Input.GetKeyDown(KeyCode.F9))
            {
                DumpStatusEffects();
            }
        }

        private void DumpStatusEffects()
        {
            if (GameData.PlayerStats == null)
            {
                Logger.LogInfo("[DruidSpells Debug] PlayerStats is null");
                return;
            }

            var stats = GameData.PlayerStats;
            Logger.LogInfo($"[DruidSpells Debug] === Status Effect Dump ===");
            Logger.LogInfo($"[DruidSpells Debug] RunSpeed (base): {stats.RunSpeed}");
            Logger.LogInfo($"[DruidSpells Debug] actualRunSpeed: {stats.actualRunSpeed}");

            var statusEffects = stats.StatusEffects;
            int count = 0;
            float totalMovementSpeed = 0f;

            for (int i = 0; i < statusEffects.Length; i++)
            {
                if (statusEffects[i] != null && statusEffects[i].Effect != null)
                {
                    var effect = statusEffects[i].Effect;
                    count++;
                    Logger.LogInfo($"[DruidSpells Debug] Slot {i}: {effect.SpellName} (Line: {effect.Line}, MovementSpeed: {effect.MovementSpeed}, Duration: {statusEffects[i].Duration})");
                    totalMovementSpeed += effect.MovementSpeed;
                }
            }

            Logger.LogInfo($"[DruidSpells Debug] Total status effects: {count}");
            Logger.LogInfo($"[DruidSpells Debug] Total MovementSpeed from effects: {totalMovementSpeed}");
            Logger.LogInfo($"[DruidSpells Debug] Expected actualRunSpeed: {stats.RunSpeed + totalMovementSpeed}");
            Logger.LogInfo($"[DruidSpells Debug] === End Dump ===");
        }
    }
}

