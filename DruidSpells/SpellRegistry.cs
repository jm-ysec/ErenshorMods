using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DruidSpells
{
    /// <summary>
    /// Manages registration and creation of custom Druid spells.
    /// Add new spells by calling RegisterSpell() with a SpellDefinition.
    /// </summary>
    public static class SpellRegistry
    {
        private static readonly List<SpellDefinition> _spellDefinitions = new List<SpellDefinition>();
        private static readonly List<Spell> _createdSpells = new List<Spell>();
        private static bool _initialized = false;
        private static string _modPath;

        /// <summary>
        /// Definition for a custom spell - add new spells by creating these
        /// </summary>
        public class SpellDefinition
        {
            public string Id { get; set; }
            public string SpellName { get; set; }
            public string Description { get; set; }
            public string IconFileName { get; set; } // e.g., "sow_64x64.png"
            public Spell.SpellType Type { get; set; } = Spell.SpellType.Beneficial;
            public Spell.SpellLine Line { get; set; } = Spell.SpellLine.Generic;
            public int RequiredLevel { get; set; } = 1;
            public int ManaCost { get; set; } = 10;
            public float CastTime { get; set; } = 3f;
            public int DurationTicks { get; set; } = 600; // ~1 minute at 10 ticks/sec
            public float MovementSpeed { get; set; } = 0f;
            public bool SelfOnly { get; set; } = false;
            public float SpellRange { get; set; } = 100f;
            public int ChargeFXIndex { get; set; } = 0;
            public int ResolveFXIndex { get; set; } = 0;
            public string StatusMessagePlayer { get; set; } = "";
            public string StatusMessageNPC { get; set; } = "";
            public Func<Class> GetClass { get; set; } // e.g., () => GameData.ClassDB.Druid
        }

        /// <summary>
        /// Register a new spell definition. Call this before Initialize().
        /// </summary>
        public static void RegisterSpell(SpellDefinition definition)
        {
            _spellDefinitions.Add(definition);
            Debug.Log($"SpellRegistry: Registered spell definition: {definition.SpellName}");
        }

        /// <summary>
        /// Initialize and create all registered spells. Call after databases are loaded.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            
            _modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            foreach (var def in _spellDefinitions)
            {
                try
                {
                    var spell = CreateSpell(def);
                    if (spell != null)
                    {
                        _createdSpells.Add(spell);
                        Debug.Log($"SpellRegistry: Created spell: {spell.SpellName}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"SpellRegistry: Failed to create spell {def.SpellName}: {ex}");
                }
            }
            
            _initialized = true;
            Debug.Log($"SpellRegistry: Initialized with {_createdSpells.Count} spells");
        }

        /// <summary>
        /// Get all created spells for injection into SpellDB
        /// </summary>
        public static List<Spell> GetCreatedSpells() => _createdSpells;

        private static Spell CreateSpell(SpellDefinition def)
        {
            var spell = ScriptableObject.CreateInstance<Spell>();
            
            // Use reflection to set the Id property (inherited from BaseScriptableObject)
            var idField = typeof(Spell).BaseType?.GetField("Id", BindingFlags.Public | BindingFlags.Instance);
            if (idField != null)
                idField.SetValue(spell, def.Id);
            else
                spell.name = def.Id; // Fallback to Unity name
            
            spell.SpellName = def.SpellName;
            spell.SpellDesc = def.Description;
            spell.Type = def.Type;
            spell.Line = def.Line;
            spell.RequiredLevel = def.RequiredLevel;
            spell.ManaCost = def.ManaCost;
            spell.SpellChargeTime = def.CastTime;
            spell.SpellDurationInTicks = def.DurationTicks;
            spell.MovementSpeed = def.MovementSpeed;
            spell.SelfOnly = def.SelfOnly;
            spell.SpellRange = def.SpellRange;
            spell.SpellChargeFXIndex = def.ChargeFXIndex;
            spell.SpellResolveFXIndex = def.ResolveFXIndex;
            spell.StatusEffectMessageOnPlayer = def.StatusMessagePlayer;
            spell.StatusEffectMessageOnNPC = def.StatusMessageNPC;
            
            // Set class restriction
            spell.UsedBy = new List<Class>();
            if (def.GetClass != null)
            {
                var classRef = def.GetClass();
                if (classRef != null)
                    spell.UsedBy.Add(classRef);
            }
            
            // Load icon from assets folder
            spell.SpellIcon = LoadIcon(def.IconFileName);

            // Initialize audio clip lists (required to avoid NullReferenceException)
            // The game checks .Count without null checking in CastSpell.StartSpell()
            spell.ChargeVariations = new List<AudioClip>();
            spell.CompleteVariations = new List<AudioClip>();

            return spell;
        }

        private static Sprite LoadIcon(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            
            string iconPath = Path.Combine(_modPath, "Assets", fileName);
            if (!File.Exists(iconPath))
            {
                Debug.LogWarning($"SpellRegistry: Icon not found: {iconPath}");
                return null;
            }
            
            byte[] data = File.ReadAllBytes(iconPath);
            var texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, data);
            
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f), 100f);
        }
    }
}

