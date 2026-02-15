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
        /// Finds the icon used by existing spell scrolls in the game.
        /// </summary>
        private static Sprite GetSpellScrollIcon()
        {
            if (_spellScrollIcon != null) return _spellScrollIcon;

            // Try to find sprite named "8" (the druid spell scroll icon)
            // Log all sprites named "8" to help identify the correct one
            var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
            int spriteCount = 0;
            foreach (var sprite in allSprites)
            {
                if (sprite.name == "8")
                {
                    spriteCount++;
                    var tex = sprite.texture;
                    Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Found sprite '8' #{spriteCount}: " +
                        $"Texture={tex?.name ?? "null"}, Size={sprite.rect.width}x{sprite.rect.height}, " +
                        $"PixelsPerUnit={sprite.pixelsPerUnit}");

                    // Store first one found as fallback
                    if (_spellScrollIcon == null)
                    {
                        _spellScrollIcon = sprite;
                    }
                    // Prefer sprites from a texture that looks like item icons (often named "Items" or similar)
                    if (tex != null && (tex.name.Contains("Item") || tex.name.Contains("Scroll") || tex.name.Contains("Spell")))
                    {
                        _spellScrollIcon = sprite;
                        Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Selected sprite from texture: {tex.name}");
                    }
                }
            }

            if (_spellScrollIcon != null)
            {
                Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Using sprite '8' from texture: {_spellScrollIcon.texture?.name ?? "unknown"}");
                return _spellScrollIcon;
            }

            // Fallback: Search for any existing item with TeachSpell set and copy its icon
            if (GameData.ItemDB?.ItemDB != null)
            {
                Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Searching {GameData.ItemDB.ItemDB.Length} items for spell scrolls...");
                foreach (var item in GameData.ItemDB.ItemDB)
                {
                    if (item != null && item.TeachSpell != null)
                    {
                        Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Found scroll: {item.ItemName} (ID: {item.Id}, Icon: {item.ItemIcon?.name ?? "null"})");
                        if (item.ItemIcon != null && _spellScrollIcon == null)
                        {
                            _spellScrollIcon = item.ItemIcon;
                        }
                    }
                }
                if (_spellScrollIcon != null)
                {
                    Plugin.Instance.Logger.LogInfo($"[VendorIntegration] Using sprite: {_spellScrollIcon.name}");
                    return _spellScrollIcon;
                }
            }

            Plugin.Instance.Logger.LogWarning("[VendorIntegration] Could not find existing spell scroll icon");
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

