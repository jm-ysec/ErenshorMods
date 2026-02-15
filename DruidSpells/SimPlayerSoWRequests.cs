using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace DruidSpells
{
    /// <summary>
    /// Adds behavior for SimPlayers to request Spirit of the Wolf buffs.
    /// - Lone players in large outdoor zones are most likely to ask
    /// - Grouped players and those in dungeons are less likely
    /// - Requests come via whisper (to player) or shout (to zone)
    /// </summary>
    public static class SimPlayerSoWRequests
    {
        private static ManualLogSource Logger => Plugin.Instance?.Logger;

        // Track cooldowns per SimPlayer to avoid spam
        private static Dictionary<int, float> _whisperCooldowns = new Dictionary<int, float>();
        private static Dictionary<int, float> _shoutCooldowns = new Dictionary<int, float>();

        // Debug logging - track last log time to avoid spam
        private static float _lastDebugLogTime = 0f;
        private const float DebugLogInterval = 30f; // Log summary every 30 seconds

        // Cooldown times (in seconds)
        private const float WhisperCooldown = 300f; // 5 minutes between whispers from same sim
        private const float ShoutCooldown = 600f;   // 10 minutes between shouts from same sim

        // Stats for debug logging
        private static int _checksPerformed = 0;
        private static int _skippedHasBuff = 0;
        private static int _skippedCooldown = 0;
        private static int _skippedGrouped = 0;
        private static int _rollsFailed = 0;
        private static int _whispersSent = 0;
        private static int _shoutsSent = 0;

        // SoW request messages
        private static readonly string[] WhisperMessages = new string[]
        {
            "Hey, any chance you could cast SoW on me?",
            "Could I get a SoW please?",
            "Spirit of the Wolf? Would really help me out.",
            "SoW would be amazing if you have time!",
            "Running slow out here, got SoW?",
            "Mind buffing me with SoW?",
            "Could use some wolf speed if you're not busy!"
        };

        private static readonly string[] ShoutMessages = new string[]
        {
            "Anyone have SoW? Running slow out here!",
            "LF SoW please!",
            "Could really use Spirit of the Wolf!",
            "Any druids around? Need SoW!",
            "SoW would be appreciated!",
            "Looking for movement speed buff!"
        };

        /// <summary>
        /// Check if a SimPlayer has a movement speed buff active.
        /// </summary>
        public static bool HasMovementSpeedBuff(Stats stats)
        {
            if (stats == null || stats.StatusEffects == null)
                return false;

            for (int i = 0; i < stats.StatusEffects.Length; i++)
            {
                var se = stats.StatusEffects[i];
                if (se != null && se.Effect != null && se.Effect.MovementSpeed > 0f)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Calculate the chance modifier based on zone and group status.
        /// Returns a multiplier (0.0 to 1.0) to apply to base chance.
        /// </summary>
        public static float GetChanceModifier(SimPlayer sim)
        {
            float modifier = 1.0f;

            // Dungeon penalty - much less likely to ask in dungeons
            if (GameData.InDungeon)
                modifier *= 0.1f;

            // Grouped penalty - grouped players rarely ask
            if (sim.InGroup)
                modifier *= 0.05f;
            else if (GameData.SimMngr != null && GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[sim.myIndex]))
                modifier *= 0.1f; // Grouped with other sims, not player

            // Distance from player - closer = more likely to whisper
            float distToPlayer = Vector3.Distance(sim.transform.position, GameData.PlayerControl.transform.position);
            if (distToPlayer > 100f)
                modifier *= 0.1f; // Far away, unlikely to whisper
            else if (distToPlayer > 50f)
                modifier *= 0.5f;

            return modifier;
        }

        /// <summary>
        /// Try to make a SimPlayer request SoW via whisper.
        /// Called periodically from the patch.
        /// </summary>
        public static void TryWhisperRequest(SimPlayer sim)
        {
            _checksPerformed++;

            if (!Plugin.EnableSoWRequests?.Value ?? true)
                return;

            float baseChance = Plugin.SoWWhisperBaseChance?.Value ?? 0.5f;
            if (baseChance <= 0f)
                return;

            // Check cooldown
            if (_whisperCooldowns.TryGetValue(sim.myIndex, out float cooldown) && cooldown > 0f)
            {
                _skippedCooldown++;
                return;
            }

            // Don't ask if already has movement buff
            if (HasMovementSpeedBuff(sim.MyStats))
            {
                _skippedHasBuff++;
                return;
            }

            // Calculate actual chance
            float chance = baseChance * GetChanceModifier(sim);

            // Roll the dice (chance is per tick, so very low)
            float roll = Random.Range(0f, 100f);
            if (roll < chance)
            {
                // Send whisper
                string message = WhisperMessages[Random.Range(0, WhisperMessages.Length)];
                string npcName = sim.GetComponent<NPC>()?.NPCName ?? sim.transform.name;

                Logger?.LogInfo($"[SoW Request] {npcName} whispers for SoW (roll: {roll:F2} < chance: {chance:F4})");

                GameData.SimMngr.LoadResponse(
                    "[WHISPER FROM] " + npcName + ": " + GameData.SimMngr.PersonalizeString(message, sim),
                    sim.transform.name);

                // Set cooldown
                _whisperCooldowns[sim.myIndex] = WhisperCooldown;
                _whispersSent++;
            }
            else
            {
                _rollsFailed++;
            }
        }

        /// <summary>
        /// Try to make a SimPlayer shout for SoW.
        /// </summary>
        public static void TryShoutRequest(SimPlayer sim)
        {
            if (!Plugin.EnableSoWRequests?.Value ?? true)
                return;

            float baseChance = Plugin.SoWShoutBaseChance?.Value ?? 0.2f;
            if (baseChance <= 0f)
                return;

            // Check cooldown
            if (_shoutCooldowns.TryGetValue(sim.myIndex, out float cooldown) && cooldown > 0f)
                return;

            // Don't shout if already has movement buff
            if (HasMovementSpeedBuff(sim.MyStats))
                return;

            // Shouts are for lone players only
            if (sim.InGroup || (GameData.SimMngr != null && GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[sim.myIndex])))
            {
                _skippedGrouped++;
                return;
            }

            // Calculate actual chance (shouts are rarer)
            float chance = baseChance * GetChanceModifier(sim) * 0.5f;

            float roll = Random.Range(0f, 100f);
            if (roll < chance)
            {
                string message = ShoutMessages[Random.Range(0, ShoutMessages.Length)];
                string npcName = sim.GetComponent<NPC>()?.NPCName ?? sim.transform.name;

                Logger?.LogInfo($"[SoW Request] {npcName} shouts for SoW (roll: {roll:F2} < chance: {chance:F4})");

                UpdateSocialLog.LogAdd(npcName + " shouts: " + GameData.SimMngr.PersonalizeString(message, sim), "#FF9000");

                _shoutCooldowns[sim.myIndex] = ShoutCooldown;
                _shoutsSent++;
            }
        }

        /// <summary>
        /// Log debug summary periodically.
        /// </summary>
        public static void LogDebugSummary()
        {
            if (Time.time - _lastDebugLogTime < DebugLogInterval)
                return;

            _lastDebugLogTime = Time.time;

            if (_checksPerformed > 0)
            {
                Logger?.LogInfo($"[SoW Request Debug] Last {DebugLogInterval}s: " +
                    $"Checks={_checksPerformed}, HasBuff={_skippedHasBuff}, Cooldown={_skippedCooldown}, " +
                    $"Grouped={_skippedGrouped}, RollFailed={_rollsFailed}, " +
                    $"Whispers={_whispersSent}, Shouts={_shoutsSent}");

                // Reset counters
                _checksPerformed = 0;
                _skippedHasBuff = 0;
                _skippedCooldown = 0;
                _skippedGrouped = 0;
                _rollsFailed = 0;
                _whispersSent = 0;
                _shoutsSent = 0;
            }
        }

        /// <summary>
        /// Update cooldowns - called each frame from the patch.
        /// </summary>
        public static void UpdateCooldowns(float deltaTime)
        {
            // Log debug summary periodically
            LogDebugSummary();

            // Update whisper cooldowns
            var whisperKeys = new List<int>(_whisperCooldowns.Keys);
            foreach (var key in whisperKeys)
            {
                _whisperCooldowns[key] -= deltaTime;
                if (_whisperCooldowns[key] <= 0f)
                    _whisperCooldowns.Remove(key);
            }

            // Update shout cooldowns
            var shoutKeys = new List<int>(_shoutCooldowns.Keys);
            foreach (var key in shoutKeys)
            {
                _shoutCooldowns[key] -= deltaTime;
                if (_shoutCooldowns[key] <= 0f)
                    _shoutCooldowns.Remove(key);
            }
        }
    }

    /// <summary>
    /// TEMPORARY: Disables SimUsable on movement speed buff spells so SimPlayers won't auto-cast them.
    /// This allows testing of the SoW request feature.
    /// Remove this patch when testing is complete!
    /// </summary>
    [HarmonyPatch(typeof(SpellDB), "Start")]
    public class SpellDB_DisableSimMovementBuffs_Patch
    {
        private static void Postfix(SpellDB __instance)
        {
            int disabled = 0;
            foreach (var spell in __instance.SpellDatabase)
            {
                if (spell != null &&
                    spell.Type == Spell.SpellType.Beneficial &&
                    spell.MovementSpeed > 0f &&
                    spell.SimUsable)
                {
                    spell.SimUsable = false;
                    disabled++;
                    Debug.Log($"[SoW Testing] Disabled SimUsable on '{spell.SpellName}'");
                }
            }
            Debug.Log($"[SoW Testing] Disabled {disabled} movement buff spells for SimPlayers");
        }
    }

    /// <summary>
    /// Harmony patch to hook into SimPlayer behavior and trigger SoW requests.
    /// </summary>
    public static class SimPlayerSoWRequestPatches
    {
        /// <summary>
        /// Patch SimPlayer.Update to periodically check for SoW requests.
        /// </summary>
        [HarmonyPatch(typeof(SimPlayer), "Update")]
        public class SimPlayer_Update_Patch
        {
            private static void Postfix(SimPlayer __instance)
            {
                // Only process if feature is enabled
                if (!Plugin.EnableSoWRequests?.Value ?? true)
                    return;

                // Skip if not alive or is GM character
                if (__instance.MyStats == null || !__instance.MyStats.Myself.Alive || __instance.IsGMCharacter)
                    return;

                // Skip if player isn't loaded
                if (GameData.PlayerControl == null || GameData.PlayerStats == null)
                    return;

                // Use instance-based timing via a simple random check to spread out processing
                // This avoids all SimPlayers checking at once
                // ~2% of frames pass through (was 0.1%) - increased for testing
                if (Random.Range(0f, 100f) > 2f)
                    return;

                // Try whisper request (to player)
                SimPlayerSoWRequests.TryWhisperRequest(__instance);

                // Try shout request (to zone)
                SimPlayerSoWRequests.TryShoutRequest(__instance);
            }
        }

        /// <summary>
        /// Patch to update cooldowns each frame.
        /// We hook into GameManager.Update for a single update point.
        /// </summary>
        [HarmonyPatch(typeof(GameManager), "Update")]
        public class GameManager_Update_Patch
        {
            private static void Postfix()
            {
                SimPlayerSoWRequests.UpdateCooldowns(Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Handles SimPlayer "thank you" responses when player buffs them with movement speed.
    /// Also creates a memory of the interaction.
    /// </summary>
    public static class SimPlayerSoWThankYou
    {
        private static readonly string[] ThankYouMessages = new string[]
        {
            "Thanks for the SoW!",
            "Appreciate the speed buff!",
            "Nice, thanks for the wolf!",
            "Sweet, SoW! Thanks!",
            "You're awesome, thanks for the buff!",
            "Much appreciated!",
            "Thanks! Gotta go fast now!",
            "Woohoo, SoW! Thank you!",
            "Perfect timing, thanks!"
        };

        /// <summary>
        /// Check if a spell is a movement speed buff (our SoW spells).
        /// </summary>
        public static bool IsMovementSpeedBuff(Spell spell)
        {
            if (spell == null)
                return false;

            return spell.Type == Spell.SpellType.Beneficial && spell.MovementSpeed > 0f;
        }

        /// <summary>
        /// Send a thank you whisper from the SimPlayer to the player.
        /// </summary>
        public static void SendThankYou(SimPlayer sim, Spell spell)
        {
            if (sim == null || spell == null)
                return;

            string message = ThankYouMessages[Random.Range(0, ThankYouMessages.Length)];
            string npcName = sim.GetComponent<NPC>()?.NPCName ?? sim.transform.name;

            // Send whisper
            GameData.SimMngr.LoadResponse(
                "[WHISPER FROM] " + npcName + ": " + GameData.SimMngr.PersonalizeString(message, sim),
                sim.transform.name);

            Plugin.Instance?.Logger?.LogInfo($"[SoW Thank You] {npcName} thanks player for {spell.SpellName}");

            // Update the SimPlayer's memory - record they grouped with player
            if (GameData.SimMngr?.Sims != null && sim.myIndex >= 0 && sim.myIndex < GameData.SimMngr.Sims.Count)
            {
                var tracking = GameData.SimMngr.Sims[sim.myIndex];
                if (tracking?.MyCurrentMemory != null)
                {
                    // Record player character name in memory
                    tracking.MyCurrentMemory.NameOfPlayerCharacter = GameData.PlayerControl?.transform.name ?? "";
                    tracking.MyCurrentMemory.GroupedLastDay = System.DateTime.Now.DayOfYear;
                    tracking.MyCurrentMemory.GroupedLastYear = System.DateTime.Now.Year;
                }

                // Increase friend level slightly
                tracking.FriendLevel += 0.1f;
            }
        }
    }

    /// <summary>
    /// Harmony patch to detect when player casts movement speed buff on SimPlayer.
    /// </summary>
    [HarmonyPatch(typeof(Stats), "AddStatusEffect", new System.Type[] { typeof(Spell), typeof(bool), typeof(int), typeof(Character) })]
    public class Stats_AddStatusEffect_Patch
    {
        private static void Postfix(Stats __instance, Spell spell, bool _fromPlayer, int _dmgBonus, Character _specificCaster)
        {
            // Only process if player cast the spell
            if (!_fromPlayer || _specificCaster == null)
                return;

            // Only process movement speed buffs
            if (!SimPlayerSoWThankYou.IsMovementSpeedBuff(spell))
                return;

            // Check if target is a SimPlayer
            if (__instance.Myself == null || !__instance.Myself.isNPC)
                return;

            var simPlayer = __instance.Myself.GetComponent<SimPlayer>();
            if (simPlayer == null || simPlayer.IsGMCharacter)
                return;

            // Check if THIS spell is now active on the target (meaning it took hold)
            // This handles both fresh buffs and upgrades (where lower buff was removed)
            bool spellTookHold = false;
            for (int i = 0; i < __instance.StatusEffects.Length; i++)
            {
                var se = __instance.StatusEffects[i];
                if (se != null && se.Effect == spell)
                {
                    spellTookHold = true;
                    break;
                }
            }

            // Only thank if the spell actually took hold
            if (spellTookHold)
            {
                SimPlayerSoWThankYou.SendThankYou(simPlayer, spell);
            }
        }
    }
}

