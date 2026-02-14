using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealWorldClock
{
    /// <summary>
    /// MonoBehaviour controller for the real-world clock display.
    /// Uses IMGUI (OnGUI) for rendering - simple and effective for HUD elements.
    /// Integrates with the game's "Toggle UI Movement" system (GameData.EditUIMode).
    /// </summary>
    public class RealWorldClockController : MonoBehaviour
    {
        private GUIStyle clockStyle;
        private GUIStyle handleStyle;
        private Rect clockRect;
        private Rect handleRect;
        private bool dragging;
        private Vector2 dragOffset;
        private bool styleInitialized;

        // Handle size matching game's blue squares
        private const float HandleSize = 16f;

        // Blue color matching game's UI handles
        private static readonly Color HandleColor = new Color(0.2f, 0.4f, 0.8f, 1f);
        private static readonly Color HandleHoverColor = new Color(0.3f, 0.5f, 0.9f, 1f);

        private Texture2D handleTexture;
        private Texture2D handleHoverTexture;

        private void Awake()
        {
            Debug.Log("RealWorldClockController: Initialized");

            // Initialize clock position from config
            clockRect = new Rect(
                Plugin.PosX.Value,
                Plugin.PosY.Value,
                100f,
                30f
            );

            // Create handle textures
            handleTexture = MakeColorTexture(HandleColor);
            handleHoverTexture = MakeColorTexture(HandleHoverColor);

            // Subscribe to scene changes to clamp position
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (handleTexture != null) Destroy(handleTexture);
            if (handleHoverTexture != null) Destroy(handleHoverTexture);
        }

        private Texture2D MakeColorTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClampRect();
        }

        private void InitStyle()
        {
            if (styleInitialized) return;

            clockStyle = new GUIStyle
            {
                fontSize = Plugin.FontSize.Value
            };
            clockStyle.normal.textColor = Color.white;

            handleStyle = new GUIStyle();
            handleStyle.normal.background = handleTexture;

            styleInitialized = true;
        }

        private void OnGUI()
        {
            if (!Plugin.ShowClock.Value) return;

            // Hide clock when not logged into a character
            if (GameData.InCharSelect || GameData.PlayerControl == null) return;

            // Initialize style lazily (must be done in OnGUI context)
            InitStyle();

            // Update font size if changed
            clockStyle.fontSize = Plugin.FontSize.Value;

            // Get current time string
            string timeText = DateTime.Now.ToString(Plugin.TimeFormat.Value);

            // Calculate size based on text
            Vector2 textSize = clockStyle.CalcSize(new GUIContent(timeText));
            clockRect.width = textSize.x + 8f;
            clockRect.height = textSize.y + 4f;

            // Update handle position (bottom center of clock)
            handleRect = new Rect(
                clockRect.x + (clockRect.width - HandleSize) / 2f,
                clockRect.y + clockRect.height + 2f,
                HandleSize,
                HandleSize
            );

            // Check if game is in UI edit mode
            bool editMode = GameData.EditUIMode;

            // Handle dragging in edit mode (left-click on handle)
            if (editMode)
            {
                Vector2 mousePos = Event.current.mousePosition;
                bool hoveringHandle = handleRect.Contains(mousePos);

                // Use Input.GetMouseButton for reliable drag tracking
                if (Input.GetMouseButtonDown(0) && hoveringHandle)
                {
                    dragging = true;
                    dragOffset = mousePos - new Vector2(clockRect.x, clockRect.y);
                    // Block game from receiving this click (same as native DragUI)
                    GameData.DraggingUIElement = true;
                }

                if (dragging && Input.GetMouseButton(0))
                {
                    clockRect.position = mousePos - dragOffset;
                }

                if (Input.GetMouseButtonUp(0) && dragging)
                {
                    dragging = false;
                    GameData.DraggingUIElement = false;
                    ClampRect();
                    SavePosition();
                }

                // Draw the blue handle square
                handleStyle.normal.background = hoveringHandle || dragging ? handleHoverTexture : handleTexture;
                GUI.Box(handleRect, GUIContent.none, handleStyle);
            }
            else
            {
                // Stop dragging if edit mode is turned off mid-drag
                if (dragging)
                {
                    dragging = false;
                    GameData.DraggingUIElement = false;
                    ClampRect();
                    SavePosition();
                }
            }

            // Ensure position is valid
            ClampRect();

            // Draw background box and time label
            GUI.Box(clockRect, GUIContent.none);
            GUI.Label(
                new Rect(clockRect.x + 4f, clockRect.y + 2f, textSize.x, textSize.y),
                timeText,
                clockStyle
            );
        }

        private void SavePosition()
        {
            Plugin.PosX.Value = clockRect.x;
            Plugin.PosY.Value = clockRect.y;
            Plugin.Instance.SaveConfig();
        }

        private void ClampRect()
        {
            clockRect.x = Mathf.Clamp(clockRect.x, 0f, Screen.width - clockRect.width);
            clockRect.y = Mathf.Clamp(clockRect.y, 0f, Screen.height - clockRect.height);
        }
    }
}

