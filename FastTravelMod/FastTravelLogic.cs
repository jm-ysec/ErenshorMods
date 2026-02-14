using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ErenshorFastTravel
{
    /// <summary>
    /// Contains all Harmony patches for the Fast Travel mod.
    /// Patches are discovered and applied by Plugin.cs via Harmony.PatchAll().
    /// </summary>
    public static class FastTravelLogic
    {
        /// <summary>
        /// Prefix patch on all CastSpell.StartSpell overloads — fixes portal spell range
        /// at cast time. The game resets SpellRange after PlayerControl.Start, so modifying
        /// it there is ineffective. By patching right before the range check executes, the
        /// fix can't be overwritten.
        /// </summary>
        [HarmonyPatch]
        public class CastSpell_StartSpell_Patch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                var methods = AccessTools.GetDeclaredMethods(typeof(CastSpell))
                    .Where(m => m.Name == "StartSpell");
                foreach (var method in methods)
                {
                    yield return method;
                }
            }

            private static void Prefix(Spell _spell)
            {
                if (_spell != null && _spell.SpellName.Contains("Portal"))
                {
                    _spell.SpellRange = 9999f;
                }
            }
        }

        /// <summary>
        /// Postfix patch on PlayerControl.Start — teaches all portal spells to the player.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControl), "Start")]
        public class PlayerControl_Start_Patch
        {
            private static void Postfix()
            {
                TeachSpell("Portal to Hidden");
                TeachSpell("Portal to Braxonian");
                TeachSpell("Portal to Ripper's Keep");
                TeachSpell("Portal to Silkengrass");
                TeachSpell("Portal to Soluna's Landing");
                TeachSpell("Portal to Reliquary");
            }

            private static void TeachSpell(string spellName)
            {
                Spell val = GameData.SpellDatabase.SpellDatabase.FirstOrDefault(s => s.SpellName == spellName);

                if (val == null)
                {
                    Debug.LogError("Spell '" + spellName + "' not found!");
                    return;
                }

                CastSpell mySpells = GameData.PlayerControl.Myself.MySpells;

                if (!mySpells.KnownSpells.Contains(val))
                {
                    mySpells.KnownSpells.Add(val);
                }
            }
        }
    }
}

