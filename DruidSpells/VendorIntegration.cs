using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DruidSpells
{
    /// <summary>
    /// Adds custom SoW spell scrolls to the Druid vendor Tiver Banes in Port Azure.
    /// Spell scrolls are Items with TeachSpell set to our custom spells.
    /// </summary>
    public static class VendorIntegration
    {
        private static List<Item> _spellScrolls = new List<Item>();
        private static bool _initialized = false;

        /// <summary>
        /// Creates spell scroll Items for each custom spell.
        /// Must be called after SpellRegistry.Initialize().
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            foreach (var spell in SpellRegistry.GetCreatedSpells())
            {
                var scroll = CreateSpellScroll(spell);
                if (scroll != null)
                {
                    _spellScrolls.Add(scroll);
                    Debug.Log($"[DruidSpells] Created spell scroll: '{scroll.ItemName}' for spell '{spell.SpellName}'");
                }
            }

            Debug.Log($"[DruidSpells] VendorIntegration initialized with {_spellScrolls.Count} spell scrolls");
        }

        // Cached reference to an existing spell scroll icon
        private static Sprite _spellScrollIcon;

        /// <summary>
        /// Finds the icon used by existing druid spell scrolls in the game.
        /// The most reliable method is to copy the sprite directly from an existing scroll item
        /// since that's exactly what the game uses.
        /// </summary>
        private static Sprite GetSpellScrollIcon()
        {
            if (_spellScrollIcon != null) return _spellScrollIcon;

            // Best approach: Find an existing druid spell scroll and copy its sprite
            // This guarantees we get the exact same sprite the game uses
            if (GameData.ItemDB?.ItemDB != null)
            {
                // Look for known druid spell scrolls by name pattern
                foreach (var item in GameData.ItemDB.ItemDB)
                {
                    if (item != null && item.TeachSpell != null && item.ItemIcon != null)
                    {
                        // Druid scrolls have Object.name like "SPELL SCROLL - DruDD ..." or "SPELL SCROLL - Biting Vines"
                        // We want the green druid scroll icon (sprite "8", 512x512)
                        if (item.name.Contains("Biting Vines") ||
                            item.name.Contains("DruDD") ||
                            item.name.Contains("Summon Forest"))
                        {
                            _spellScrollIcon = item.ItemIcon;
                            Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Copied sprite from existing druid scroll: {item.ItemName}");
                            Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Sprite: name={_spellScrollIcon.name}, " +
                                $"size={_spellScrollIcon.rect.width}x{_spellScrollIcon.rect.height}");
                            return _spellScrollIcon;
                        }
                    }
                }

                // Fallback: any spell scroll with the "8" icon that's 512x512
                foreach (var item in GameData.ItemDB.ItemDB)
                {
                    if (item != null && item.TeachSpell != null && item.ItemIcon != null)
                    {
                        if (item.ItemIcon.name == "8" &&
                            item.ItemIcon.rect.width == 512 &&
                            item.ItemIcon.rect.height == 512)
                        {
                            _spellScrollIcon = item.ItemIcon;
                            Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Copied 512x512 sprite from: {item.ItemName}");
                            return _spellScrollIcon;
                        }
                    }
                }
            }

            Plugin.Instance.Logger.LogWarning("[VendorIntegration] Could not find existing druid spell scroll icon");
            return null;
        }

        /// <summary>
        /// Creates a spell scroll Item that teaches the given spell.
        /// </summary>
        private static Item CreateSpellScroll(Spell spell)
        {
            if (spell == null) return null;

            var scroll = ScriptableObject.CreateInstance<Item>();
            scroll.name = $"SPELL SCROLL - {spell.SpellName}";  // Unity Object.name
            scroll.Id = $"druidspells_scroll_{spell.Id}";
            scroll.ItemName = $"Spell Scroll: {spell.SpellName}";
            scroll.RequiredSlot = Item.SlotType.General;
            scroll.TeachSpell = spell;

            // Price based on spell level (1000 gold per level, minimum 1000)
            scroll.ItemValue = Mathf.Max(1000, spell.RequiredLevel * 1000);

            // Initialize required collections to avoid NullReferenceException
            scroll.Classes = new System.Collections.Generic.List<Class>();

            // Set lore/description
            scroll.Lore = $"This scroll teaches the spell: {spell.SpellName}\n\n{spell.SpellDesc}";

            // Use icon from an existing spell scroll in the game
            scroll.ItemIcon = GetSpellScrollIcon();

            // Make it unique so you can only have one at a time
            scroll.Unique = false;
            scroll.Stackable = false;

            return scroll;
        }

        /// <summary>
        /// Gets all created spell scrolls.
        /// </summary>
        public static List<Item> GetSpellScrolls()
        {
            return _spellScrolls;
        }
    }

    /// <summary>
    /// Harmony patch to add spell scrolls to Tiver Banes' vendor inventory.
    /// </summary>
    [HarmonyPatch(typeof(VendorInventory), "Start")]
    public class VendorInventory_Start_Patch
    {
        private static void Postfix(VendorInventory __instance)
        {
            // Check if this is Tiver Banes (Druid spell vendor in Port Azure)
            string npcName = __instance.transform.name;
            
            // Vendors can have various name formats, so check for "Tiver Banes" anywhere
            if (!npcName.Contains("Tiver Banes"))
                return;

            // Ensure vendor integration is initialized
            VendorIntegration.Initialize();

            // Add our spell scrolls to this vendor
            var scrolls = VendorIntegration.GetSpellScrolls();
            if (scrolls.Count == 0)
            {
                Debug.Log($"[DruidSpells] Found Tiver Banes but no spell scrolls to add");
                return;
            }

            foreach (var scroll in scrolls)
            {
                if (!__instance.ItemsForSale.Contains(scroll))
                {
                    __instance.ItemsForSale.Add(scroll);
                    Debug.Log($"[DruidSpells] Added '{scroll.ItemName}' to Tiver Banes' inventory");
                }
            }

            Debug.Log($"[DruidSpells] Added {scrolls.Count} spell scrolls to Tiver Banes");
        }
    }
}
