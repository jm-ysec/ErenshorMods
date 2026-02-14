using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace __MOD_NAMESPACE__
{
    /// <summary>
    /// Contains all Harmony patches for the mod.
    /// Patches are discovered and applied by Plugin.cs via Harmony.PatchAll().
    /// </summary>
    public static class __MOD_CLASS_NAME__
    {
        // ---------------------------------------------------------------
        // Example 1: Patch a single method (no overloads)
        // ---------------------------------------------------------------
        // [HarmonyPatch(typeof(TargetClass), "MethodName")]
        // public class TargetClass_MethodName_Patch
        // {
        //     private static void Prefix()
        //     {
        //         // Runs BEFORE the original method.
        //     }
        //
        //     private static void Postfix()
        //     {
        //         // Runs AFTER the original method.
        //     }
        // }

        // ---------------------------------------------------------------
        // Example 2: Patch a method with a specific signature
        // ---------------------------------------------------------------
        // [HarmonyPatch(typeof(TargetClass), "MethodName", new[] { typeof(string), typeof(int) })]
        // public class TargetClass_MethodName_Patch
        // {
        //     private static void Prefix(string param1, int param2)
        //     {
        //     }
        // }

        // ---------------------------------------------------------------
        // Example 3: Patch ALL overloads of a method using TargetMethods
        // Use this when the target method has multiple overloads.
        // ---------------------------------------------------------------
        // [HarmonyPatch]
        // public class TargetClass_MethodName_Patch
        // {
        //     static IEnumerable<MethodBase> TargetMethods()
        //     {
        //         return AccessTools.GetDeclaredMethods(typeof(TargetClass))
        //             .Where(m => m.Name == "MethodName");
        //     }
        //
        //     private static void Prefix(/* first param shared by all overloads */)
        //     {
        //     }
        // }
    }
}

