using HarmonyLib;
using UnityEngine;

namespace MovementSpeedManager
{
    /// <summary>
    /// Contains all Harmony patches for the mod.
    /// Patches are discovered and applied by Plugin.cs via Harmony.PatchAll().
    /// </summary>
    public static class MovementSpeedManagerPatches
    {
        /// <summary>
        /// Patch for Stats.CalcStats to apply custom movement speeds based on entity type.
        /// </summary>
        [HarmonyPatch(typeof(Stats), "CalcStats")]
        public class Stats_CalcStats_Patch
        {
            private static void Postfix(Stats __instance)
            {
                // Determine the appropriate speed based on entity type
                float speed = DetermineSpeed(__instance);

                // Apply the speed
                __instance.RunSpeed = speed;
            }

            /// <summary>
            /// Determines the appropriate movement speed for the given Stats instance.
            /// Priority: Specific entity type > Global
            /// </summary>
            private static float DetermineSpeed(Stats stats)
            {
                // Check if this is the player (has PlayerControl component)
                if (stats.GetComponent<PlayerControl>() != null)
                {
                    float playerSpeed = Plugin.PlayerSpeed.Value;
                    return playerSpeed >= 0 ? playerSpeed : Plugin.GlobalSpeed.Value;
                }

                // Check if this is a SimPlayer
                SimPlayer simPlayer = stats.GetComponent<SimPlayer>();
                if (simPlayer != null)
                {
                    // Check if grouped with player
                    NPC npc = stats.GetComponent<NPC>();
                    bool isGrouped = false;

                    if (npc != null && npc.ThisSim != null)
                    {
                        isGrouped = npc.ThisSim.InGroup;
                    }

                    if (isGrouped)
                    {
                        // Grouped SimPlayer - check GroupedSimPlayerSpeed first
                        float groupedSpeed = Plugin.GroupedSimPlayerSpeed.Value;
                        if (groupedSpeed >= 0)
                            return groupedSpeed;

                        // Fall back to SimPlayerSpeed if GroupedSimPlayerSpeed is not set
                        float simPlayerSpeed = Plugin.SimPlayerSpeed.Value;
                        return simPlayerSpeed >= 0 ? simPlayerSpeed : Plugin.GlobalSpeed.Value;
                    }
                    else
                    {
                        // Not grouped, use SimPlayerSpeed
                        float simPlayerSpeed = Plugin.SimPlayerSpeed.Value;
                        return simPlayerSpeed >= 0 ? simPlayerSpeed : Plugin.GlobalSpeed.Value;
                    }
                }

                // Check if this is an NPC or monster (has NPC component but not SimPlayer)
                NPC npcComponent = stats.GetComponent<NPC>();
                if (npcComponent != null && !npcComponent.SimPlayer)
                {
                    float npcSpeed = Plugin.NPCSpeed.Value;
                    return npcSpeed >= 0 ? npcSpeed : Plugin.GlobalSpeed.Value;
                }

                // Default to global speed for any other entity
                return Plugin.GlobalSpeed.Value;
            }
        }
    }
}

