using BepInEx;
using UnityEngine;

namespace ErenshorAreaMaps
{
    [BepInPlugin("com.noone.area_maps_mod", "Fixed Erenshor Area Maps Mod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Area Maps Mod: Initializing...");
            GameObject mapHandler = new GameObject("AreaMapsController");
            mapHandler.transform.SetParent(null); // Ensure root object
            mapHandler.hideFlags = HideFlags.HideAndDontSave;
            mapHandler.AddComponent<AreaMapsLogic>();
            Object.DontDestroyOnLoad(mapHandler);
        }
    }
}

