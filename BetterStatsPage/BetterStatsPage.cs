using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BetterStatsPage
{
    /// <summary>
    /// MonoBehaviour controller for the Better Stats Page UI.
    /// Creates and manages a tabbed UI for player stats and reputation.
    /// Persistence is handled by Plugin.cs - this component should NOT call DontDestroyOnLoad.
    /// </summary>
    public class BetterStatsPageController : MonoBehaviour
    {
        // UI Components
        private Canvas statsCanvas;
        private GameObject canvasGO;
        private RectTransform panelRect;
        private GameObject windowPanel;
        private GameObject headerObject;
        private GameObject tabContainer;
        private GameObject contentObject;
        private GameObject dragHandle;
        private Button statsTab;
        private Button repTab;
        private Image statsTabImage;
        private Image repTabImage;
        private CanvasGroup panelGroup;

        // State
        private bool isUIInitialized = false;
        private bool dragging = false;
        private Vector2 dragOffset;

        // Stats caching for change detection
        private float updateInterval = 0.1f;
        private float nextUpdateTime = 0f;
        private int lastLevel, lastStr, lastDex, lastEnd, lastAgi, lastInt, lastWis, lastCha;
        private float lastAttackAbility, lastLifeSteal, lastRunSpeed;
        private int lastDS;
        private int lastItemStr, lastItemDex, lastItemEnd, lastItemAgi, lastItemInt, lastItemWis, lastItemCha;

        private void Awake()
        {
            Debug.Log("BetterStatsPageController: Initialized");
        }

        private void Update()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            string sceneName = activeScene.name;

            // Don't show UI in menus
            if (sceneName == "Menu" || sceneName == "LoadScene")
            {
                if (windowPanel != null && windowPanel.activeSelf)
                {
                    panelGroup.alpha = 0f;
                    panelGroup.interactable = false;
                    panelGroup.blocksRaycasts = false;
                    windowPanel.SetActive(false);
                }
                return;
            }

            // Initialize UI when game is ready
            if (!isUIInitialized && GameData.PlayerControl != null && GameData.PlayerStats != null)
            {
                isUIInitialized = true;
                InitializeUI();
                CacheCurrentStats();
            }

            if (isUIInitialized)
            {
                // Handle toggle key
                if (Input.GetKeyDown(Plugin.ToggleKey.Value) && !GameData.PlayerTyping && !GameData.InCharSelect)
                {
                    TogglePanel();
                }

                // Update stats periodically when panel is visible
                if (windowPanel.activeSelf && Time.time >= nextUpdateTime)
                {
                    CheckForStatsChanges();
                    nextUpdateTime = Time.time + updateInterval;
                }
            }
        }

        private void InitializeUI()
        {
            // Create canvas as a root object - ScreenSpaceOverlay requires root canvas
            // We mark it with HideAndDontSave so it persists like the controller
            canvasGO = new GameObject("BetterStatsCanvas");
            canvasGO.transform.SetParent(null); // Must be root for ScreenSpaceOverlay
            canvasGO.hideFlags = HideFlags.HideAndDontSave;
            statsCanvas = canvasGO.AddComponent<Canvas>();
            statsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            statsCanvas.sortingOrder = 100; // Ensure it renders on top
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f; // Balance between width and height scaling
            canvasGO.AddComponent<GraphicRaycaster>();
            UnityEngine.Object.DontDestroyOnLoad(canvasGO); // Persist across scenes

            // Create window panel - scaled down to ~40% of original size
            windowPanel = new GameObject("WindowPanel");
            windowPanel.transform.SetParent(canvasGO.transform, false);
            panelRect = windowPanel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(320f, 450f); // Reduced from 420x620
            panelRect.anchoredPosition = new Vector2(200f, 0f); // Offset to the right so it doesn't cover center
            Image panelImage = windowPanel.AddComponent<Image>();
            panelImage.color = new Color32(20, 30, 45, 230);
            windowPanel.AddComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, 0.5f);
            panelGroup = windowPanel.AddComponent<CanvasGroup>();
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;

            // Create header
            headerObject = new GameObject("Header");
            headerObject.transform.SetParent(windowPanel.transform, false);
            RectTransform headerRect = headerObject.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.sizeDelta = new Vector2(0f, 30f); // Reduced from 40f
            Image headerImage = headerObject.AddComponent<Image>();
            headerImage.color = new Color32(18, 30, 45, 255);
            Text headerText = CreateText(headerObject.transform, "Player Stats & Reputation", 14, TextAnchor.MiddleCenter); // Reduced from 22
            headerText.rectTransform.anchorMin = Vector2.zero;
            headerText.rectTransform.anchorMax = Vector2.one;

            // Create drag handle
            dragHandle = new GameObject("DragHandle");
            dragHandle.transform.SetParent(headerObject.transform, false);
            RectTransform dragRect = dragHandle.AddComponent<RectTransform>();
            dragRect.anchorMin = new Vector2(1f, 1f);
            dragRect.anchorMax = new Vector2(1f, 1f);
            dragRect.pivot = new Vector2(1f, 1f);
            dragRect.anchoredPosition = new Vector2(-20f, -10f);
            dragRect.sizeDelta = new Vector2(16f, 16f);
            Image dragImage = dragHandle.AddComponent<Image>();
            dragImage.color = new Color32(108, 194, 255, 255);
            dragHandle.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
            AddDragEvents(dragHandle.AddComponent<EventTrigger>());

            // Create tab container
            tabContainer = new GameObject("Tabs");
            tabContainer.transform.SetParent(windowPanel.transform, false);
            RectTransform tabRect = tabContainer.AddComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0f, 1f);
            tabRect.anchorMax = new Vector2(1f, 1f);
            tabRect.pivot = new Vector2(0.5f, 1f);
            tabRect.anchoredPosition = new Vector2(0f, -30f); // Adjusted for smaller header
            tabRect.sizeDelta = new Vector2(0f, 24f); // Reduced from 32f
            HorizontalLayoutGroup tabLayout = tabContainer.AddComponent<HorizontalLayoutGroup>();
            tabLayout.childAlignment = TextAnchor.MiddleCenter;
            tabLayout.spacing = 12f;

            // Create tabs
            statsTab = CreateTabButton("Stats");
            statsTab.onClick.AddListener(() => ShowStats(true));
            statsTabImage = statsTab.GetComponent<Image>();

            repTab = CreateTabButton("Reputation");
            repTab.onClick.AddListener(() => ShowStats(false));
            repTabImage = repTab.GetComponent<Image>();

            // Create content area
            contentObject = new GameObject("Content");
            contentObject.transform.SetParent(windowPanel.transform, false);
            RectTransform contentRect = contentObject.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.offsetMin = new Vector2(8f, 8f); // Reduced padding
            contentRect.offsetMax = new Vector2(-8f, -60f); // Adjusted for smaller header/tabs (was -84)
            VerticalLayoutGroup contentLayout = contentObject.AddComponent<VerticalLayoutGroup>();
            contentLayout.childAlignment = TextAnchor.UpperLeft;
            contentLayout.spacing = 3f; // Reduced from 6f
            contentLayout.padding = new RectOffset(4, 4, 4, 4); // Reduced from 8,8,8,8
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            // Initialize with stats tab
            ShowStats(true);
            HighlightTab(true);
            windowPanel.SetActive(false);

            Debug.Log("BetterStatsPageController: UI Initialized");
        }

        private Button CreateTabButton(string label)
        {
            GameObject tabGO = new GameObject(label + "Tab");
            tabGO.transform.SetParent(tabContainer.transform, false);
            RectTransform tabBtnRect = tabGO.AddComponent<RectTransform>();
            tabBtnRect.sizeDelta = new Vector2(90f, 24f); // Reduced from 120x32
            Image tabBtnImage = tabGO.AddComponent<Image>();
            tabBtnImage.color = Plugin.InactiveTabColor.Value;
            Button btn = tabGO.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = new ColorBlock();
            colors.normalColor = Plugin.InactiveTabColor.Value;
            colors.highlightedColor = Plugin.InactiveTabColor.Value * 1.2f;
            colors.pressedColor = Plugin.InactiveTabColor.Value * 0.9f;
            colors.selectedColor = Plugin.InactiveTabColor.Value;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            btn.colors = colors;
            CreateText(tabGO.transform, label, 12, TextAnchor.MiddleCenter); // Reduced from 18
            return btn;
        }

        private Text CreateText(Transform parent, string text, int size, TextAnchor align)
        {
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(parent, false);
            Text textComp = textGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComp.fontSize = size;
            textComp.alignment = align;
            textComp.color = Color.white;
            textGO.AddComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, 0.5f);
            return textComp;
        }

        private IEnumerator FadeIn()
        {
            windowPanel.SetActive(true);
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                panelGroup.alpha = t;
                yield return null;
            }
            panelGroup.alpha = 1f;
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;
        }

        private IEnumerator FadeOut()
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * 2f;
                panelGroup.alpha = t;
                yield return null;
            }
            panelGroup.alpha = 0f;
            windowPanel.SetActive(false);
        }

        private void TogglePanel()
        {
            if (!windowPanel.activeSelf)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                StartCoroutine(FadeOut());
            }
        }

        private void ShowStats(bool showStats)
        {
            foreach (Transform child in contentObject.transform)
            {
                Destroy(child.gameObject);
            }
            if (showStats)
            {
                RefreshStats();
            }
            else
            {
                RefreshReputation();
            }
            HighlightTab(showStats);
        }

        private void HighlightTab(bool statsActive)
        {
            statsTabImage.color = statsActive ? Plugin.ActiveTabColor.Value : Plugin.InactiveTabColor.Value;
            repTabImage.color = statsActive ? Plugin.InactiveTabColor.Value : Plugin.ActiveTabColor.Value;
        }

        private void AddDragEvents(EventTrigger et)
        {
            // Begin drag
            EventTrigger.Entry beginDrag = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            beginDrag.callback.AddListener((data) =>
            {
                dragging = true;
                PointerEventData pData = (PointerEventData)data;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, pData.position, null, out dragOffset);
            });
            et.triggers.Add(beginDrag);

            // Drag
            EventTrigger.Entry drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            drag.callback.AddListener((data) =>
            {
                if (dragging)
                {
                    PointerEventData pData = (PointerEventData)data;
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)statsCanvas.transform, pData.position, null, out localPoint);
                    panelRect.anchoredPosition = localPoint - dragOffset;
                }
            });
            et.triggers.Add(drag);

            // End drag
            EventTrigger.Entry endDrag = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDrag.callback.AddListener((data) => { dragging = false; });
            et.triggers.Add(endDrag);
        }

        private void AddLine(string text)
        {
            GameObject lineGO = new GameObject("Line");
            lineGO.transform.SetParent(contentObject.transform, false);
            RectTransform lineRect = lineGO.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0f, 1f);
            lineRect.anchorMax = new Vector2(1f, 1f);
            lineRect.pivot = new Vector2(0.5f, 1f);
            lineRect.sizeDelta = new Vector2(0f, 18f); // Reduced from 24f
            Text lineText = lineGO.AddComponent<Text>();
            lineText.text = text;
            lineText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            lineText.fontSize = 12; // Reduced from 16
            lineText.alignment = TextAnchor.MiddleLeft;
            lineText.color = Color.white;
            lineGO.AddComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, 0.5f);
        }

        private void RefreshStats()
        {
            Stats playerStats = GameData.PlayerStats;
            AddLine(string.Format("{0,-20}{1,8}", "Level:", playerStats.Level));
            AddLine(string.Format("{0,-20}{1,8}", "Ascension Level:", playerStats.Myself.MySkills.GetPointsSpent()));
            AddLine(string.Format("{0,-20}{1,8} / {2,-8}", "Exp:", playerStats.CurrentExperience, playerStats.ExperienceToLevelUp));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Strength:", playerStats.GetCurrentStr(), playerStats.MyInv.ItemStr));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Dexterity:", playerStats.GetCurrentDex(), playerStats.MyInv.ItemDex));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Endurance:", playerStats.GetCurrentEnd(), playerStats.MyInv.ItemEnd));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Agility:", playerStats.GetCurrentAgi(), playerStats.MyInv.ItemAgi));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Intelligence:", playerStats.GetCurrentInt(), playerStats.MyInv.ItemInt));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Wisdom:", playerStats.GetCurrentWis(), playerStats.MyInv.ItemWis));
            AddLine(string.Format("{0,-20}{1,4}  (+{2,3})", "Charisma:", playerStats.GetCurrentCha(), playerStats.MyInv.ItemCha));

            int currentDex = playerStats.GetCurrentDex();
            int level = playerStats.Level;
            int dexBenefit = playerStats.CharacterClass.DexBenefit;
            float num = (float)dexBenefit / 100f;
            float num2 = (float)level / (100f - (float)dexBenefit);
            float num3 = (float)currentDex * (num + num2);
            float num4 = Mathf.Min(1f, num3 / 100f);
            float critChance = num4 * (float)level;
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Crit Chance:", critChance));

            int duelistBonus = playerStats.CharacterClass.ClassName == "Duelist" ? 1 : 0;
            float dodgeBase = 5f - (float)duelistBonus;
            float dodgeChance = dodgeBase / 20f * 100f;
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Dodge Chance:", dodgeChance));

            AddLine(string.Format("{0,-20}{1,8:F1}", "Physical Attack:", playerStats.AttackAbility));
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Spell Dmg Bonus:", (float)playerStats.GetCurrentInt() * ((float)playerStats.CharacterClass.IntBenefit / 100f)));
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Healing Bonus:", (float)playerStats.GetCurrentWis() * ((float)playerStats.CharacterClass.WisBenefit / 100f)));
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Life Steal:", playerStats.PercentLifesteal));
            AddLine(string.Format("{0,-20}{1,8}", "Damage Shield:", playerStats.GetCurrentDS()));
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Attack Speed Bonus:", (float)playerStats.GetCurrentDex() * ((float)playerStats.CharacterClass.DexBenefit / 100f)));
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Move Speed Bonus:", (playerStats.actualRunSpeed - playerStats.RunSpeed) / playerStats.RunSpeed * 100f));
            AddLine(string.Format("{0,-20}{1,8:F1}%", "Resonance Proc Chance:", (float)playerStats.GetCurrentDex() * ((float)playerStats.CharacterClass.DexBenefit / 100f)));
        }

        private void RefreshReputation()
        {
            Debug.Log("BetterStatsPage: RefreshReputation called");

            // Try to load factions if not already loaded
            if (GlobalFactionManager.AllFactions == null || GlobalFactionManager.AllFactions.Count == 0)
            {
                Debug.Log("BetterStatsPage: AllFactions is null or empty, attempting to load...");
                try
                {
                    GlobalFactionManager.LoadFactions();
                    Debug.Log($"BetterStatsPage: LoadFactions completed. Count: {GlobalFactionManager.AllFactions?.Count ?? 0}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"BetterStatsPage: Error loading factions: {ex.Message}");
                    AddLine("Error loading faction data");
                    return;
                }
            }

            // Check again after attempting to load
            if (GlobalFactionManager.AllFactions == null || GlobalFactionManager.AllFactions.Count == 0)
            {
                AddLine("No faction data available");
                AddLine("(Factions load after character selection)");
                return;
            }

            Debug.Log($"BetterStatsPage: Displaying {GlobalFactionManager.AllFactions.Count} factions");
            foreach (NPCFaction faction in GlobalFactionManager.AllFactions)
            {
                if (faction != null)
                {
                    AddLine($"{faction.Desc}: {faction.Value:F1}");
                }
            }
        }

        private void CacheCurrentStats()
        {
            Stats playerStats = GameData.PlayerStats;
            lastLevel = playerStats.Level;
            lastStr = playerStats.GetCurrentStr();
            lastDex = playerStats.GetCurrentDex();
            lastEnd = playerStats.GetCurrentEnd();
            lastAgi = playerStats.GetCurrentAgi();
            lastInt = playerStats.GetCurrentInt();
            lastWis = playerStats.GetCurrentWis();
            lastCha = playerStats.GetCurrentCha();
            lastAttackAbility = playerStats.AttackAbility;
            lastLifeSteal = playerStats.PercentLifesteal;
            lastDS = playerStats.GetCurrentDS();
            lastRunSpeed = playerStats.actualRunSpeed;
            lastItemStr = playerStats.MyInv.ItemStr;
            lastItemDex = playerStats.MyInv.ItemDex;
            lastItemEnd = playerStats.MyInv.ItemEnd;
            lastItemAgi = playerStats.MyInv.ItemAgi;
            lastItemInt = playerStats.MyInv.ItemInt;
            lastItemWis = playerStats.MyInv.ItemWis;
            lastItemCha = playerStats.MyInv.ItemCha;
        }

        private void CheckForStatsChanges()
        {
            Stats playerStats = GameData.PlayerStats;
            bool isDirty = false;

            if (lastLevel != playerStats.Level ||
                lastStr != playerStats.GetCurrentStr() ||
                lastDex != playerStats.GetCurrentDex() ||
                lastEnd != playerStats.GetCurrentEnd() ||
                lastAgi != playerStats.GetCurrentAgi() ||
                lastInt != playerStats.GetCurrentInt() ||
                lastWis != playerStats.GetCurrentWis() ||
                lastCha != playerStats.GetCurrentCha() ||
                lastAttackAbility != playerStats.AttackAbility ||
                lastLifeSteal != playerStats.PercentLifesteal ||
                lastDS != playerStats.GetCurrentDS() ||
                lastRunSpeed != playerStats.actualRunSpeed ||
                lastItemStr != playerStats.MyInv.ItemStr ||
                lastItemDex != playerStats.MyInv.ItemDex ||
                lastItemEnd != playerStats.MyInv.ItemEnd ||
                lastItemAgi != playerStats.MyInv.ItemAgi ||
                lastItemInt != playerStats.MyInv.ItemInt ||
                lastItemWis != playerStats.MyInv.ItemWis ||
                lastItemCha != playerStats.MyInv.ItemCha)
            {
                isDirty = true;
            }

            if (isDirty)
            {
                RefreshCurrentTab();
                CacheCurrentStats();
            }
        }

        private void RefreshCurrentTab()
        {
            bool showStats = statsTabImage.color == Plugin.ActiveTabColor.Value;
            ShowStats(showStats);
        }
    }
}