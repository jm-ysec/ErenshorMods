using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace __MOD_NAMESPACE__
{
    /// <summary>
    /// MonoBehaviour component for the mod.
    /// This is attached to a GameObject and persists across scenes.
    /// </summary>
    public class __MOD_CLASS_NAME__Controller : MonoBehaviour
    {
        private static __MOD_CLASS_NAME__Controller _instance;
        
        public static __MOD_CLASS_NAME__Controller Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("__MOD_CLASS_NAME__Controller");
                    _instance = go.AddComponent<__MOD_CLASS_NAME__Controller>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("__MOD_CLASS_NAME__Controller: Initialized");
        }
        
        private void Update()
        {
            // Called every frame
            // Add your update logic here
        }
        
        private void OnGUI()
        {
            // Called for rendering and handling GUI events
            // Example: Draw a simple UI
            // GUI.Label(new Rect(10, 10, 200, 20), "__Mod Display Name__");
        }
    }
    
    /// <summary>
    /// Contains Harmony patches for the mod.
    /// </summary>
    public static class __MOD_CLASS_NAME__Patches
    {
        // Example: Initialize the MonoBehaviour controller when the game starts
        // [HarmonyPatch(typeof(GameManager), "Start")]
        // public class GameManager_Start_Patch
        // {
        //     private static void Postfix()
        //     {
        //         // Ensure the controller is created
        //         var controller = __MOD_CLASS_NAME__Controller.Instance;
        //     }
        // }
    }
}

