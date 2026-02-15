using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ResizableChatCombat
{
    /// <summary>
    /// Controller that manages resizable chat and combat windows.
    /// </summary>
    public class ResizableChatCombatController : MonoBehaviour
    {
        private static ResizableChatCombatController _instance;

        // References to the window RectTransforms
        private RectTransform chatWindowRT;
        private RectTransform combatWindowRT;

        // Resize handles
        private ResizeHandle chatResizeHandle;
        private ResizeHandle combatResizeHandle;

        // Track initialization
        private bool initialized = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void Update()
        {
            // Wait for game to be ready
            if (GameData.InCharSelect || GameData.PlayerControl == null)
            {
                initialized = false;
                return;
            }

            // Initialize once when in game
            if (!initialized)
            {
                TryInitialize();
            }

            // Update resize handles visibility based on edit mode
            if (initialized)
            {
                UpdateHandleVisibility();
            }
        }

        // References to the visual backdrop containers (from WindowScaler)
        private RectTransform chatBackdropRT;
        private RectTransform combatBackdropRT;

        private void TryInitialize()
        {
            // Find chat log component
            if (GameData.ChatLog == null)
                return;

            // Get the chat window's ScrollRect
            var chatScrollRect = GameData.ChatLog.scrollRect;
            if (chatScrollRect == null)
                return;

            // Use the ScrollRect's RectTransform directly - this is what we'll attach handles to
            chatWindowRT = chatScrollRect.GetComponent<RectTransform>();
            if (chatWindowRT == null)
                return;

            // Find WindowScaler component to get the ChatBox (visual backdrop) reference
            var chatWindowScaler = chatScrollRect.GetComponentInParent<WindowScaler>();
            if (chatWindowScaler != null && chatWindowScaler.ChatBox != null)
            {
                chatBackdropRT = chatWindowScaler.ChatBox;
            }
            else
            {
                // Look for sibling named "ChatWindow" - this is the visual backdrop
                chatBackdropRT = FindSiblingByName(chatWindowRT, "ChatWindow");
                if (chatBackdropRT == null)
                {
                    chatBackdropRT = chatWindowRT.parent?.GetComponent<RectTransform>();
                }
            }

            // Combat content is separate - find its ScrollRect
            var combatContent = GameData.ChatLog.combatContent;
            if (combatContent != null)
            {
                var combatParent = combatContent.parent;
                if (combatParent != null)
                {
                    var scrollRect = combatParent.GetComponentInParent<ScrollRect>();
                    if (scrollRect != null && scrollRect != chatScrollRect)
                    {
                        combatWindowRT = scrollRect.GetComponent<RectTransform>();

                        // Find WindowScaler for combat window
                        var combatWindowScaler = scrollRect.GetComponentInParent<WindowScaler>();
                        if (combatWindowScaler != null && combatWindowScaler.ChatBox != null)
                        {
                            combatBackdropRT = combatWindowScaler.ChatBox;
                        }
                        else
                        {
                            combatBackdropRT = combatWindowRT?.parent?.GetComponent<RectTransform>();
                        }
                    }
                }
            }

            // Create resize handles
            CreateResizeHandles();

            initialized = true;
        }

        /// <summary>
        /// Find a sibling RectTransform by name
        /// </summary>
        private RectTransform FindSiblingByName(RectTransform rt, string name)
        {
            if (rt.parent == null) return null;

            for (int i = 0; i < rt.parent.childCount; i++)
            {
                var sibling = rt.parent.GetChild(i);
                if (sibling.name == name)
                {
                    return sibling.GetComponent<RectTransform>();
                }
            }
            return null;
        }

        private void CreateResizeHandles()
        {
            // Create chat resize handle (top-right, expands right and up)
            if (chatWindowRT != null && chatResizeHandle == null)
            {
                chatResizeHandle = CreateResizeHandle(chatWindowRT, chatBackdropRT, "ChatResizeHandle",
                    Plugin.ChatWidth, Plugin.ChatHeight, false);
            }

            // Create combat resize handle (top-left, expands left and up - mirror of chat)
            if (combatWindowRT != null && combatResizeHandle == null)
            {
                combatResizeHandle = CreateResizeHandle(combatWindowRT, combatBackdropRT, "CombatResizeHandle",
                    Plugin.CombatWidth, Plugin.CombatHeight, true);
            }
        }

        private ResizeHandle CreateResizeHandle(RectTransform targetWindow, RectTransform backdropContainer, string name,
            BepInEx.Configuration.ConfigEntry<float> widthConfig,
            BepInEx.Configuration.ConfigEntry<float> heightConfig,
            bool isCombat)
        {
            // Create handle GameObject as child of target window
            GameObject handleGO = new GameObject(name);
            handleGO.transform.SetParent(targetWindow, false);

            // Add RectTransform - position handle OUTSIDE the window corner
            RectTransform handleRT = handleGO.AddComponent<RectTransform>();
            handleRT.sizeDelta = new Vector2(20f, 20f);

            if (isCombat)
            {
                // Combat: handle at top-left, OUTSIDE the window
                // Anchor to top-left of parent, pivot at bottom-right so handle sits outside
                handleRT.anchorMin = new Vector2(0f, 1f);
                handleRT.anchorMax = new Vector2(0f, 1f);
                handleRT.pivot = new Vector2(1f, 0f); // Bottom-right of handle touches top-left of window
                handleRT.anchoredPosition = Vector2.zero;
            }
            else
            {
                // Chat: handle at top-right, OUTSIDE the window
                // Anchor to top-right of parent, pivot at bottom-left so handle sits outside
                handleRT.anchorMin = new Vector2(1f, 1f);
                handleRT.anchorMax = new Vector2(1f, 1f);
                handleRT.pivot = new Vector2(0f, 0f); // Bottom-left of handle touches top-right of window
                handleRT.anchoredPosition = Vector2.zero;
            }

            // Add Image for visual handle
            Image handleImage = handleGO.AddComponent<Image>();
            handleImage.color = new Color(0.3f, 0.5f, 0.8f, 0.8f); // Blue like DragUI
            handleImage.enabled = false; // Hidden by default

            // Add resize handle component
            ResizeHandle handle = handleGO.AddComponent<ResizeHandle>();
            handle.Initialize(targetWindow, backdropContainer, handleImage, widthConfig, heightConfig, isCombat);

            return handle;
        }

        private void UpdateHandleVisibility()
        {
            bool showHandles = GameData.EditUIMode;

            if (chatResizeHandle != null)
                chatResizeHandle.SetVisible(showHandles);

            if (combatResizeHandle != null)
                combatResizeHandle.SetVisible(showHandles);
        }

        private void ApplySavedSizes()
        {
            if (chatWindowRT != null)
            {
                chatWindowRT.sizeDelta = new Vector2(Plugin.ChatWidth.Value, Plugin.ChatHeight.Value);
            }

            if (combatWindowRT != null)
            {
                combatWindowRT.sizeDelta = new Vector2(Plugin.CombatWidth.Value, Plugin.CombatHeight.Value);
            }
        }
    }

    /// <summary>
    /// Component that handles resizing a UI element via drag.
    /// </summary>
    public class ResizeHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private RectTransform targetWindow;
        private RectTransform backdropContainer; // The visual backdrop container
        private RectTransform bottomBar; // Optional bottom bar (BottomBG for combat, InputBox for chat)
        private RectTransform topBar; // Optional top bar (Image for chat)
        private List<RectTransform> combatBottomElements = new List<RectTransform>(); // Elements near bottom that need to stay fixed
        private Image handleImage;
        private BepInEx.Configuration.ConfigEntry<float> widthConfig;
        private BepInEx.Configuration.ConfigEntry<float> heightConfig;
        private float minWidth = 650f;  // Minimum to prevent text overflow
        private float maxWidth = 2000f;  // Allow full width
        private float minHeight = 80f;
        private float maxHeight = 2000f; // Allow full height

        private bool dragging = false;
        private bool isCombatWindow = false; // Combat expands left, chat expands right
        private Vector2 startMousePos;
        private Vector2 startSize;
        private Vector2 startBackdropSize;
        private float startBottomBarWidth;
        private float startTopBarWidth;

        // World positions of fixed corners (bottom-left for chat, bottom-right for combat)
        private Vector3 targetFixedCornerWorld;
        private Vector3 backdropFixedCornerWorld;
        private Vector3 bottomBarFixedCornerWorld;
        private Vector3 topBarFixedCornerWorld;
        private List<Vector3> combatBottomElementsFixedCorners = new List<Vector3>();

        public void Initialize(RectTransform target, RectTransform backdrop, Image image,
            BepInEx.Configuration.ConfigEntry<float> width,
            BepInEx.Configuration.ConfigEntry<float> height,
            bool isCombat)
        {
            targetWindow = target;
            backdropContainer = backdrop;
            handleImage = image;
            widthConfig = width;
            heightConfig = height;
            isCombatWindow = isCombat;

            // Find bars that need to resize with the window
            if (target.parent != null)
            {
                for (int i = 0; i < target.parent.childCount; i++)
                {
                    var sibling = target.parent.GetChild(i);

                    if (isCombatWindow)
                    {
                        // Combat window: find BottomBG and other bottom elements
                        if (sibling.name == "BottomBG")
                        {
                            bottomBar = sibling.GetComponent<RectTransform>();
                        }
                        // Also track elements that need to stay fixed with the bottom bar
                        else if (sibling.name == "AutomateAttack" || sibling.name == "CombatBG" ||
                                 sibling.name == "ShieldBonus" || sibling.name == "DragCombat")
                        {
                            var rt = sibling.GetComponent<RectTransform>();
                            if (rt != null)
                            {
                                combatBottomElements.Add(rt);
                            }
                        }
                    }
                    else
                    {
                        // Chat window: find InputBox (bottom) and first Image sibling (top bar)
                        if (sibling.name == "InputBox")
                        {
                            bottomBar = sibling.GetComponent<RectTransform>();
                        }
                        else if (sibling.name == "Image" && topBar == null)
                        {
                            topBar = sibling.GetComponent<RectTransform>();
                        }
                    }
                }
            }
        }

        public void SetVisible(bool visible)
        {
            if (handleImage != null)
                handleImage.enabled = visible;

            // Stop dragging if hidden while dragging
            if (!visible && dragging)
            {
                dragging = false;
                GameData.DraggingUIElement = false;
                SaveSize();
            }
        }

        private Vector3 GetFixedCorner(RectTransform rt)
        {
            // Get world corners: [0]=bottom-left, [1]=top-left, [2]=top-right, [3]=bottom-right
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            // Chat: bottom-left fixed (index 0), Combat: bottom-right fixed (index 3)
            return isCombatWindow ? corners[3] : corners[0];
        }

        private Vector3 GetBarFixedCorner(RectTransform rt)
        {
            // Bars: Chat keeps left edge (bottom-left), Combat keeps right edge (bottom-right)
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return isCombatWindow ? corners[3] : corners[0];
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!GameData.EditUIMode) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;

            dragging = true;
            GameData.DraggingUIElement = true;
            startMousePos = eventData.position;

            // Capture starting sizes
            startSize = targetWindow.rect.size;
            if (backdropContainer != null)
                startBackdropSize = backdropContainer.rect.size;
            if (bottomBar != null)
                startBottomBarWidth = bottomBar.rect.width;
            if (topBar != null)
                startTopBarWidth = topBar.rect.width;

            // Capture world positions of fixed corners
            targetFixedCornerWorld = GetFixedCorner(targetWindow);
            if (backdropContainer != null)
                backdropFixedCornerWorld = GetFixedCorner(backdropContainer);
            if (bottomBar != null)
                bottomBarFixedCornerWorld = GetBarFixedCorner(bottomBar);
            if (topBar != null)
                topBarFixedCornerWorld = GetBarFixedCorner(topBar);

            // Capture fixed corners for combat bottom elements
            combatBottomElementsFixedCorners.Clear();
            foreach (var elem in combatBottomElements)
            {
                combatBottomElementsFixedCorners.Add(GetBarFixedCorner(elem));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!dragging) return;

            dragging = false;
            GameData.DraggingUIElement = false;
            SaveSize();
        }

        private void ResizeAndFixCorner(RectTransform rt, float newWidth, float newHeight, Vector3 fixedCornerWorld)
        {
            // Resize the element
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            // After resize, get the current position of the fixed corner
            Vector3 currentFixedCorner = GetFixedCorner(rt);

            // Calculate the world-space offset needed to restore the corner position
            Vector3 worldOffset = fixedCornerWorld - currentFixedCorner;

            // Convert world offset to local offset and apply to position
            rt.position += worldOffset;
        }

        private void ResizeBarAndFixCorner(RectTransform rt, float newWidth, Vector3 fixedCornerWorld)
        {
            // Resize width only (bars don't change height)
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

            // After resize, get the current position of the fixed corner
            Vector3 currentFixedCorner = GetBarFixedCorner(rt);

            // Calculate the world-space offset needed to restore the corner position
            Vector3 worldOffset = fixedCornerWorld - currentFixedCorner;

            // Apply full offset (parent may have moved the bar, need to compensate Y too)
            rt.position += worldOffset;
        }

        private void FixElementPosition(RectTransform rt, Vector3 fixedCornerWorld)
        {
            // Fix element position without resizing (just restore corner position)
            Vector3 currentFixedCorner = GetBarFixedCorner(rt);
            Vector3 worldOffset = fixedCornerWorld - currentFixedCorner;
            rt.position += worldOffset;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;

            Vector2 delta = eventData.position - startMousePos;

            // Calculate size change based on window type:
            // In Unity screen space, Y increases UPWARD, so delta.y is positive when dragging up
            // Chat: handle at top-right, drag right/up = expand right/up
            // Combat: handle at top-left, drag left/up = expand left/up
            float deltaWidth = isCombatWindow ? -delta.x : delta.x;
            float deltaHeight = delta.y; // Drag up = positive delta.y = taller

            // Calculate new sizes
            float newWidth = Mathf.Clamp(startSize.x + deltaWidth, minWidth, maxWidth);
            float newHeight = Mathf.Clamp(startSize.y + deltaHeight, minHeight, maxHeight);

            // Resize target (ScrollRect) and fix the corner position
            ResizeAndFixCorner(targetWindow, newWidth, newHeight, targetFixedCornerWorld);

            // Resize backdrop container
            if (backdropContainer != null)
            {
                float backdropNewWidth = Mathf.Clamp(startBackdropSize.x + deltaWidth, minWidth, maxWidth);
                float backdropNewHeight = Mathf.Clamp(startBackdropSize.y + deltaHeight, minHeight, maxHeight);
                ResizeAndFixCorner(backdropContainer, backdropNewWidth, backdropNewHeight, backdropFixedCornerWorld);
            }

            // Resize bottom bar width (keeps fixed edge)
            if (bottomBar != null)
            {
                float barNewWidth = Mathf.Clamp(startBottomBarWidth + deltaWidth, minWidth, maxWidth);
                ResizeBarAndFixCorner(bottomBar, barNewWidth, bottomBarFixedCornerWorld);
            }

            // Resize top bar width (chat only, keeps left edge fixed)
            if (topBar != null)
            {
                float barNewWidth = Mathf.Clamp(startTopBarWidth + deltaWidth, minWidth, maxWidth);
                ResizeBarAndFixCorner(topBar, barNewWidth, topBarFixedCornerWorld);
            }

            // Fix positions of combat bottom elements (they don't resize, just need to stay in place)
            for (int i = 0; i < combatBottomElements.Count; i++)
            {
                FixElementPosition(combatBottomElements[i], combatBottomElementsFixedCorners[i]);
            }
        }

        private void SaveSize()
        {
            if (targetWindow == null) return;

            widthConfig.Value = targetWindow.sizeDelta.x;
            heightConfig.Value = targetWindow.sizeDelta.y;
            Plugin.Instance.SaveConfig();
        }
    }
}

