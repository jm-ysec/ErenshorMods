using HarmonyLib;
using UnityEngine;

namespace DruidSpells
{
    /// <summary>
    /// Main mod class - registers spell definitions.
    /// To add a new spell, add a new RegisterSpell call in RegisterAllSpells().
    /// </summary>
    public static class DruidSpellsMod
    {
        /// <summary>
        /// Register all custom Druid spells here.
        /// Called at plugin startup before databases are loaded.
        /// </summary>
        public static void RegisterAllSpells()
        {
            // Lesser Spirit of the Wolf - Entry level movement speed buff
            SpellRegistry.RegisterSpell(new SpellRegistry.SpellDefinition
            {
                Id = "druidspells_lesser_spirit_of_the_wolf",
                SpellName = "Lesser Spirit of the Wolf",
                Description = "Calls upon a lesser wolf spirit to increase movement speed.",
                IconFileName = "sow_64x64.png",
                Type = Spell.SpellType.Beneficial,
                Line = Spell.SpellLine.Global_Move,
                RequiredLevel = 5,
                ManaCost = 50,
                CastTime = 360f, // 6 seconds (60 ticks = 1 second)
                DurationTicks = 150, // 15 minutes (1 tick = 6 seconds)
                MovementSpeed = 1.5f,
                SelfOnly = false,
                SpellRange = 100f,
                ChargeFXIndex = 7,
                ResolveFXIndex = 7,
                StatusMessagePlayer = "You feel a lesser wolf spirit guiding your steps.",
                StatusMessageNPC = " feels a lesser wolf spirit.",
                GetClass = () => GameData.ClassDB?.Druid
            });

            // Spirit of the Wolf - Standard movement speed buff
            SpellRegistry.RegisterSpell(new SpellRegistry.SpellDefinition
            {
                Id = "druidspells_spirit_of_the_wolf",
                SpellName = "Spirit of the Wolf",
                Description = "Calls upon the spirit of the wolf to increase movement speed.",
                IconFileName = "sow_64x64.png",
                Type = Spell.SpellType.Beneficial,
                Line = Spell.SpellLine.Global_Move,
                RequiredLevel = 15,
                ManaCost = 100,
                CastTime = 600f, // 10 seconds (60 ticks = 1 second)
                DurationTicks = 300, // 30 minutes (1 tick = 6 seconds)
                MovementSpeed = 2.5f,
                SelfOnly = false,
                SpellRange = 100f,
                ChargeFXIndex = 7,
                ResolveFXIndex = 7,
                StatusMessagePlayer = "You feel the spirit of the wolf coursing through you.",
                StatusMessageNPC = " feels the spirit of the wolf.",
                GetClass = () => GameData.ClassDB?.Druid
            });

            // Spirit of the Greater Wolf - Enhanced movement speed buff
            SpellRegistry.RegisterSpell(new SpellRegistry.SpellDefinition
            {
                Id = "druidspells_spirit_of_the_greater_wolf",
                SpellName = "Spirit of the Greater Wolf",
                Description = "Calls upon a greater wolf spirit to greatly increase movement speed.",
                IconFileName = "sow_64x64.png",
                Type = Spell.SpellType.Beneficial,
                Line = Spell.SpellLine.Global_Move,
                RequiredLevel = 25,
                ManaCost = 150,
                CastTime = 600f, // 10 seconds (60 ticks = 1 second)
                DurationTicks = 450, // 45 minutes (1 tick = 6 seconds)
                MovementSpeed = 3.5f,
                SelfOnly = false,
                SpellRange = 100f,
                ChargeFXIndex = 7,
                ResolveFXIndex = 7,
                StatusMessagePlayer = "You feel a greater wolf spirit empowering your stride.",
                StatusMessageNPC = " feels a greater wolf spirit.",
                GetClass = () => GameData.ClassDB?.Druid
            });

            // Spirit of the Elder Wolf - Ultimate movement speed buff
            SpellRegistry.RegisterSpell(new SpellRegistry.SpellDefinition
            {
                Id = "druidspells_spirit_of_the_elder_wolf",
                SpellName = "Spirit of the Elder Wolf",
                Description = "Calls upon an ancient elder wolf spirit for maximum movement speed.",
                IconFileName = "sow_64x64.png",
                Type = Spell.SpellType.Beneficial,
                Line = Spell.SpellLine.Global_Move,
                RequiredLevel = 35,
                ManaCost = 200,
                CastTime = 300f, // 5 seconds (60 ticks = 1 second)
                DurationTicks = 600, // 60 minutes (1 tick = 6 seconds)
                MovementSpeed = 5.0f,
                SelfOnly = false,
                SpellRange = 100f,
                ChargeFXIndex = 7,
                ResolveFXIndex = 7,
                StatusMessagePlayer = "You feel an elder wolf spirit infusing your very being.",
                StatusMessageNPC = " feels an elder wolf spirit.",
                GetClass = () => GameData.ClassDB?.Druid
            });

            Debug.Log("DruidSpellsMod: All spell definitions registered.");
        }
    }

    /// <summary>
    /// Harmony patches to inject custom spells into the game's SpellDB
    /// and teach them to the player automatically.
    /// </summary>
    public static class DruidSpellsPatches
    {
        /// <summary>
        /// Patch PlayerControl.Start() to teach custom spells to the player automatically.
        /// This runs when the player loads into the game world.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControl), "Start")]
        public class PlayerControl_Start_Patch
        {
            private static void Postfix()
            {
                // Teach all custom spells to the player
                foreach (var spell in SpellRegistry.GetCreatedSpells())
                {
                    TeachSpellSilently(spell);
                }
            }

            /// <summary>
            /// Teach a spell without sound/visual effects (silent load)
            /// </summary>
            private static void TeachSpellSilently(Spell spell)
            {
                if (spell == null) return;

                CastSpell mySpells = GameData.PlayerControl.Myself.MySpells;

                if (!mySpells.KnownSpells.Contains(spell))
                {
                    mySpells.KnownSpells.Add(spell);
                    Debug.Log($"DruidSpellsPatches: Taught '{spell.SpellName}' to player");
                }
            }
        }

        /// <summary>
        /// Patch SpellDB.Start() to inject custom spells after the database loads.
        /// </summary>
        [HarmonyPatch(typeof(SpellDB), "Start")]
        public class SpellDB_Start_Patch
        {
            private static void Postfix(SpellDB __instance)
            {
                Debug.Log("DruidSpellsPatches: SpellDB.Start postfix - injecting custom spells...");

                // Initialize the spell registry (creates ScriptableObjects)
                SpellRegistry.Initialize();

                // Get our custom spells
                var customSpells = SpellRegistry.GetCreatedSpells();
                if (customSpells.Count == 0)
                {
                    Debug.Log("DruidSpellsPatches: No custom spells to inject.");
                    return;
                }

                // Expand the SpellDatabase array to include our custom spells
                var originalSpells = __instance.SpellDatabase;
                var newSpellArray = new Spell[originalSpells.Length + customSpells.Count];

                // Copy original spells
                for (int i = 0; i < originalSpells.Length; i++)
                {
                    newSpellArray[i] = originalSpells[i];
                }

                // Add custom spells
                for (int i = 0; i < customSpells.Count; i++)
                {
                    newSpellArray[originalSpells.Length + i] = customSpells[i];
                    Debug.Log($"DruidSpellsPatches: Added spell '{customSpells[i].SpellName}' to SpellDB");
                }

                // Replace the database array
                __instance.SpellDatabase = newSpellArray;

                Debug.Log($"DruidSpellsPatches: SpellDB now contains {newSpellArray.Length} spells " +
                         $"({originalSpells.Length} original + {customSpells.Count} custom)");
            }
        }
    }

    /// <summary>
    /// Fixes spell stacking - when a higher level spell on the same line is cast,
    /// removes any existing lower level spells on that line first.
    /// The base game only blocks lower-level spells, but doesn't remove them when
    /// casting a higher-level replacement.
    /// </summary>
    [HarmonyPatch(typeof(Stats), "AddStatusEffect", new System.Type[] { typeof(Spell), typeof(bool), typeof(int), typeof(Character) })]
    public class Stats_RemoveLowerLevelSpells_Patch
    {
        // Run before the main AddStatusEffect to clean up lower-level spells
        [HarmonyPriority(Priority.High)]
        private static void Prefix(Stats __instance, Spell spell)
        {
            if (spell == null || spell.Type != Spell.SpellType.Beneficial)
                return;

            // Skip if Generic line (those don't stack-check)
            if (spell.Line == Spell.SpellLine.Generic)
                return;

            // Find and remove any lower-level spells on the same line
            for (int i = 0; i < __instance.StatusEffects.Length; i++)
            {
                var se = __instance.StatusEffects[i];
                if (se == null || se.Effect == null)
                    continue;

                // Same spell line, and existing spell is LOWER level
                if (se.Effect.Line == spell.Line && se.Effect.RequiredLevel < spell.RequiredLevel)
                {
                    Debug.Log($"[SpellStacking] Removing '{se.Effect.SpellName}' (level {se.Effect.RequiredLevel}) " +
                              $"to make room for '{spell.SpellName}' (level {spell.RequiredLevel})");
                    __instance.RemoveStatusEffect(i);
                }
            }
        }
    }

    /// <summary>
    /// Fixes the bug where beneficial spell status messages appear even when the spell
    /// doesn't take hold due to a higher level spell already being active.
    /// This is a bug in the base game where SpellVessel.ResolveSpell() shows the status
    /// message BEFORE checking CheckForHigherLevelSE().
    /// </summary>
    public static class SpellMessageFixer
    {
        // State tracking for beneficial spell resolution
        private static bool _isResolvingBeneficialSpell = false;
        private static Spell _currentSpell = null;
        private static bool _spellWasBlocked = false;

        /// <summary>
        /// Prefix patch for SpellVessel.ResolveSpell() - tracks when beneficial spells
        /// are being resolved and pre-checks if they'll be blocked.
        /// </summary>
        [HarmonyPatch(typeof(SpellVessel), "ResolveSpell")]
        public class SpellVessel_ResolveSpell_Patch
        {
            private static void Prefix(Spell ___spell, Stats ___targ)
            {
                if (___spell != null && ___spell.Type == Spell.SpellType.Beneficial)
                {
                    _isResolvingBeneficialSpell = true;
                    _currentSpell = ___spell;

                    // Pre-check if spell will be blocked by a higher level spell
                    if (___targ != null && ___targ.Myself != null && !___targ.Myself.Invulnerable)
                    {
                        _spellWasBlocked = ___targ.Myself.MyStats.CheckForHigherLevelSE(___spell);
                    }
                    else
                    {
                        _spellWasBlocked = false;
                    }
                }
            }

            private static void Postfix()
            {
                // Clean up state after ResolveSpell completes
                _isResolvingBeneficialSpell = false;
                _currentSpell = null;
                _spellWasBlocked = false;
            }
        }

        /// <summary>
        /// Prefix patch for UpdateSocialLog.LogAdd() - intercepts status effect messages
        /// during beneficial spell resolution and replaces them with "did not take hold"
        /// message if the spell was blocked.
        /// </summary>
        [HarmonyPatch(typeof(UpdateSocialLog), "LogAdd", typeof(string), typeof(string))]
        public class UpdateSocialLog_LogAdd_Patch
        {
            private static bool Prefix(ref string _string, ref string _colorAsString)
            {
                // Fast path - not resolving a beneficial spell
                if (!_isResolvingBeneficialSpell || _currentSpell == null)
                    return true;

                // Check if this is the status effect message for the current spell
                // The message format is "You " + spell.StatusEffectMessageOnPlayer
                if (_string.StartsWith("You ") && _currentSpell.StatusEffectMessageOnPlayer != null &&
                    _string.Contains(_currentSpell.StatusEffectMessageOnPlayer))
                {
                    if (_spellWasBlocked)
                    {
                        // Replace with "did not take hold" message in red
                        _string = "Your spell did not take hold.";
                        _colorAsString = "red";
                    }
                }

                return true; // Let the (possibly modified) message through
            }
        }
    }

    /// <summary>
    /// Increases the backward movement speed cap when player has movement speed buffs.
    /// The base game caps backward movement at 7f regardless of buffs.
    /// This patch allows 50% of movement speed buffs to also increase the backward cap.
    /// </summary>
    public static class BackwardSpeedCapFixer
    {
        /// <summary>
        /// Postfix patch for Stats.CalcStats() - recalculates actualRunSpeed with
        /// a higher backward cap if player has movement speed buffs.
        /// </summary>
        [HarmonyPatch(typeof(Stats), "CalcStats")]
        public class Stats_CalcStats_Patch
        {
            private static void Postfix(Stats __instance, float ___seRunSpeed)
            {
                // Get config value (0-100%)
                float buffPercent = Plugin.BackwardSpeedBuffPercent?.Value ?? 50f;

                // Skip if disabled or no buff or not retreating
                if (buffPercent <= 0f || !__instance.isRetreating || ___seRunSpeed <= 0f)
                    return;

                // Calculate new cap: base 7 + configured % of movement speed buff
                float buffMultiplier = buffPercent / 100f;
                float newCap = 7f + (___seRunSpeed * buffMultiplier);

                // Recalculate speed with new cap
                float speed = __instance.RunSpeed + ___seRunSpeed;
                if (speed > newCap)
                    speed = newCap;
                if (speed < 2f)
                    speed = 2f;

                __instance.actualRunSpeed = speed;
            }
        }
    }
}

