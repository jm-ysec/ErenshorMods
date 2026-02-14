using BepInEx;
using HarmonyLib;

namespace __MOD_NAMESPACE__
{
    [BepInPlugin("com.noone.__mod_id__", "__Mod Display Name__", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("__Mod Display Name__: Initializing...");
            
            // Initialize the MonoBehaviour controller
            var controller = __MOD_CLASS_NAME__Controller.Instance;
            
            // Apply Harmony patches
            Harmony harmony = new Harmony("com.noone.__mod_id__");
            harmony.PatchAll();
            
            Logger.LogInfo("__Mod Display Name__: Initialized with MonoBehaviour controller.");
        }
    }
}

