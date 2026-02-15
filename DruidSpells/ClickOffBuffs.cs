using HarmonyLib;
using UnityEngine;

namespace DruidSpells
{
    /// <summary>
    /// Allows players to click off beneficial buffs they've cast on themselves.
    /// Single-clicking a buff icon removes the buff and all its effects.
    /// 
    /// Restrictions:
    /// - Only works on Beneficial spells (not debuffs)
    /// - Only works on buffs cast by the player on themselves
    /// - Cannot click off buffs on other characters
    /// </summary>
    public static class ClickOffBuffs
    {
        /// <summary>
        /// Patch StatusEffectIcon to handle click events for buff removal.
        /// We use a Postfix on Update to check for clicks since we can't add interfaces via Harmony.
        /// </summary>
        [HarmonyPatch(typeof(StatusEffectIcon), "Update")]
        public class StatusEffectIcon_Update_Patch
        {
            // Track which icon was clicked to prevent double-processing
            private static StatusEffectIcon _lastClickedIcon = null;
            private static float _clickCooldown = 0f;

            private static void Postfix(StatusEffectIcon __instance, bool ___mouseOver)
            {
                // Update cooldown
                if (_clickCooldown > 0f)
                    _clickCooldown -= Time.deltaTime;

                // Only process if mouse is over this icon and left mouse button clicked
                if (!___mouseOver || !Input.GetMouseButtonDown(0))
                    return;

                // Prevent double-clicks
                if (_clickCooldown > 0f && _lastClickedIcon == __instance)
                    return;

                // Check if the icon is actually showing a buff
                if (!__instance.Icon.enabled)
                    return;

                // Get the Stats component this icon reads from
                Stats stats = __instance.ReadStats;
                if (stats == null)
                    return;

                // Only allow clicking off buffs on the PLAYER
                if (stats != GameData.PlayerStats)
                    return;

                int slotIndex = __instance.SlotIndex;
                if (slotIndex < 0 || slotIndex >= stats.StatusEffects.Length)
                    return;

                StatusEffect statusEffect = stats.StatusEffects[slotIndex];
                if (statusEffect == null || statusEffect.Effect == null)
                    return;

                // Only allow clicking off BENEFICIAL spells
                if (statusEffect.Effect.Type != Spell.SpellType.Beneficial)
                {
                    Debug.Log($"[ClickOffBuffs] Cannot click off {statusEffect.Effect.SpellName} - not a beneficial spell");
                    return;
                }

                // Only allow clicking off buffs cast by the PLAYER on THEMSELVES
                // Check if the owner is the player character
                if (statusEffect.Owner != GameData.PlayerControl.Myself)
                {
                    Debug.Log($"[ClickOffBuffs] Cannot click off {statusEffect.Effect.SpellName} - not cast by player");
                    UpdateSocialLog.LogAdd("You can only click off buffs you cast on yourself.", "yellow");
                    return;
                }

                // All checks passed - remove the buff
                string spellName = statusEffect.Effect.SpellName;
                stats.RemoveStatusEffect(slotIndex);

                Debug.Log($"[ClickOffBuffs] Player clicked off {spellName}");
                UpdateSocialLog.LogAdd($"You clicked off {spellName}.", "lightblue");

                // Set cooldown to prevent rapid double-clicks
                _lastClickedIcon = __instance;
                _clickCooldown = 0.3f;
            }
        }
    }
}

