using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace ErenshorAreaMaps
{
    public class AreaMapsLogic : MonoBehaviour
    {
        private string mapAssetsPath;

        private List<string> mapAreas = new List<string>
        {
            "Azure", "Azynthi", "AzynthiClear", "Blight", "Brake", "Braxonian",
            "Duskenlight", "FernallaField", "Hidden", "Loomingwood",
            "Malaroth", "Ripper", "SaltedStrand", "ShiveringStep", "ShiveringTomb",
            "Silkengrass", "Soluna", "Stowaway", "Tutorial", "Vitheo",
            "Windwashed", "Abyssal", "Bonepits", "Braxonia", "DuskenPortal",
            "Elderstone", "FernallaPortal", "Jaws", "Krakengard", "PrielPlateau",
            "RipperPortal", "Rockshade", "Rottenfoot", "Undercity", "Underspine",
            "VitheosEnd"
        };

        private Dictionary<string, Texture2D> mapCache = new Dictionary<string, Texture2D>();
        private string sceneName;
        private GameObject mapCanvas;
        private string lastLoadedMap = "";
        private Button toggleButton;
        private bool showingWorldMap = false;
        private Text toggleButtonText;

        private void Awake()
        {
            mapAssetsPath = Path.Combine(Paths.PluginPath, "AreaMaps", "Assets");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneName = scene.name;
            if (sceneName == "Menu" || sceneName == "LoadScene")
                return;

            if (mapCanvas == null)
            {
                RectTransform[] allRects = Resources.FindObjectsOfTypeAll<RectTransform>();
                foreach (RectTransform rt in allRects)
                {
                    if (rt.name == "Map" && rt.GetComponent<Image>() != null)
                    {
                        mapCanvas = rt.gameObject;
                        break;
                    }
                }
            }

            if (mapCanvas == null)
                return;

            CreateToggleButton();

            Image component = mapCanvas.GetComponent<Image>();
            if (component == null || lastLoadedMap == sceneName)
                return;

            if (!mapAreas.Contains(sceneName) || mapAreas.IndexOf(sceneName) >= 21)
            {
                ApplyMapTexture("MapRoutes", true);
                ButtonSetter(-2, false);
            }
            else
            {
                ApplyMapTexture(sceneName, false);
                ButtonSetter(mapAreas.IndexOf(sceneName));
            }
        }

        private void CreateToggleButton()
        {
            if (mapCanvas == null || toggleButton != null)
                return;

            GameObject buttonObj = new GameObject("MapToggleButton", new System.Type[]
            {
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button)
            });
            buttonObj.transform.SetParent(mapCanvas.transform, false);

            RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1f, 0f);
            btnRect.anchorMax = new Vector2(1f, 0f);
            btnRect.anchoredPosition = new Vector2(-60f, 0f);
            btnRect.sizeDelta = new Vector2(100f, 40f);

            Image btnImage = buttonObj.GetComponent<Image>();
            btnImage.color = new Color(1f / 85f, 18f / 85f, 24f / 85f, 1f);

            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = new Color(0.2784314f, 35f / 51f, 0.7058824f, 1f);
            outline.effectDistance = new Vector2(1f, 1f);

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(ToggleMapView);
            toggleButton = btn;

            GameObject textObj = new GameObject("Text", new System.Type[]
            {
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text)
            });
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            toggleButtonText = textObj.GetComponent<Text>();
            toggleButtonText.text = showingWorldMap ? "Area Map" : "World Map";
            toggleButtonText.alignment = TextAnchor.MiddleCenter;
            toggleButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            toggleButtonText.color = Color.white;
            toggleButtonText.fontSize = 18;
        }

        private void ToggleMapView()
        {
            if (mapCanvas == null)
                return;

            Image component = mapCanvas.GetComponent<Image>();
            if (component == null)
                return;

            if (showingWorldMap)
            {
                if (mapAreas.Contains(sceneName) && mapAreas.IndexOf(sceneName) < 21)
                {
                    ApplyMapTexture(sceneName, false);
                    ButtonSetter(mapAreas.IndexOf(sceneName));
                }
            }
            else
            {
                ApplyMapTexture("MapRoutes", true);
                ButtonSetter(-1);
            }
        }

        private Texture2D GetMapTexture(string mapName)
        {
            if (mapCache.ContainsKey(mapName))
                return mapCache[mapName];

            string path = Path.Combine(mapAssetsPath, mapName + ".png");

            try
            {
                byte[] data = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(tex, data);
                mapCache[mapName] = tex;
                return tex;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[AreaMaps] Failed to load map image " + mapName + ": " + ex.Message);
                return null;
            }
        }

        private void ApplyMapTexture(string mapKey, bool isWorldMap)
        {
            Image mapImage = mapCanvas != null ? mapCanvas.GetComponent<Image>() : null;
            if (mapImage == null)
                return;

            Texture2D tex = GetMapTexture(mapKey);
            if (tex == null)
                return;

            mapImage.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            showingWorldMap = isWorldMap;
            lastLoadedMap = sceneName;
        }

        private void ButtonSetter(int index, bool hasAreaMap = true)
        {
            if (!hasAreaMap && index == -2)
            {
                if (toggleButton != null)
                    toggleButton.gameObject.SetActive(false);
            }
            else if (index == -1 || index >= 21)
            {
                if (toggleButtonText != null)
                    toggleButtonText.text = "Area Map";
                if (toggleButton != null)
                    toggleButton.gameObject.SetActive(true);
            }
            else
            {
                if (toggleButtonText != null)
                    toggleButtonText.text = "World Map";
                if (toggleButton != null)
                    toggleButton.gameObject.SetActive(true);
            }
        }
    }
}