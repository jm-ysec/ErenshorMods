using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace __MOD_NAMESPACE__
{
    /// <summary>
    /// Configuration manager for the mod.
    /// </summary>
    public static class __MOD_CLASS_NAME__Config
    {
        // Configuration entries
        public static ConfigEntry<bool> EnableFeature;
        public static ConfigEntry<int> SomeValue;
        public static ConfigEntry<float> SomeMultiplier;
        public static ConfigEntry<string> SomeText;
        
        public static void Initialize(ConfigFile config)
        {
            // Boolean setting
            EnableFeature = config.Bind(
                "__Mod Display Name__",
                "EnableFeature",
                true,
                "Enable or disable the main feature"
            );
            
            // Integer setting
            SomeValue = config.Bind(
                "__Mod Display Name__",
                "SomeValue",
                100,
                new ConfigDescription(
                    "Some configurable value",
                    new AcceptableValueRange<int>(1, 1000)
                )
            );
            
            // Float setting
            SomeMultiplier = config.Bind(
                "__Mod Display Name__",
                "SomeMultiplier",
                1.5f,
                new ConfigDescription(
                    "Multiplier for some calculation",
                    new AcceptableValueRange<float>(0.1f, 10f)
                )
            );
            
            // String setting
            SomeText = config.Bind(
                "__Mod Display Name__",
                "SomeText",
                "Default text",
                "Some configurable text"
            );
        }
    }
    
    /// <summary>
    /// Contains all Harmony patches for the mod.
    /// </summary>
    public static class __MOD_CLASS_NAME__Patches
    {
        // Example: Use configuration in a patch
        // [HarmonyPatch(typeof(SomeClass), "SomeMethod")]
        // public class SomeClass_SomeMethod_Patch
        // {
        //     private static void Prefix(ref int value)
        //     {
        //         if (__MOD_CLASS_NAME__Config.EnableFeature.Value)
        //         {
        //             value = (int)(value * __MOD_CLASS_NAME__Config.SomeMultiplier.Value);
        //         }
        //     }
        // }
    }
}

