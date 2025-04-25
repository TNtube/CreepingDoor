using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    /// <summary>
    /// Add the Symbol popup to the GameObject header.
    /// </summary>
    //[CustomEditor(typeof(GameObject))]
    [CanEditMultipleObjects]
    public class IconGameObjectInspector : Editor
    {
        [SerializeField] private Texture2D m_Icon;
        [SerializeField] private VisualTreeAsset m_emojiPopupAsset;

        private Editor m_GameObjectInspector;
        private MethodInfo m_OnHeaderGUI;
#if PROJECT_FIXES
        private FieldInfo m_mPreviewCache; // Dictionnary of Textures, DestroyImmediate
#endif

        void OnEnable()
        {
            System.Type gameObjectInspectorType = typeof(Editor).Assembly.GetType("UnityEditor.GameObjectInspector");
            m_OnHeaderGUI = gameObjectInspectorType.GetMethod("OnHeaderGUI", BindingFlags.NonPublic | BindingFlags.Instance);
#if PROJECT_FIXES
            m_mPreviewCache = gameObjectInspectorType.GetField("m_PreviewCache", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
            m_GameObjectInspector = CreateEditor(target, gameObjectInspectorType);
        }

        void OnDisable()
        {
            if (m_GameObjectInspector != null && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
#if PROJECT_FIXES
                System.Collections.Generic.Dictionary<int, Texture> previewCache = (System.Collections.Generic.Dictionary<int, Texture>)m_mPreviewCache.GetValue(m_GameObjectInspector);
                System.Collections.Generic.List<int> keys = new System.Collections.Generic.List<int>(previewCache.Keys);
                foreach (int key in keys) if (previewCache[key] == null) previewCache.Remove(key);
                m_mPreviewCache.SetValue(m_GameObjectInspector, previewCache);
#endif
                DestroyImmediate(m_GameObjectInspector);
                m_GameObjectInspector = null;
            }
        }

        protected override void OnHeaderGUI()
        {
            if (m_OnHeaderGUI != null)
            {
                m_OnHeaderGUI.Invoke(m_GameObjectInspector, null);
            }
            DrawEmojiAddButton();
        }

        private void DrawEmojiAddButton()
        {
            // Get the last rect used by the GUI (which is the header in this context)
            Rect iconRect = GUILayoutUtility.GetLastRect();

            // Adjust the rect to cover the icon dropdown area
            iconRect.x += 0;  // Adjust X to align over the icon area
            iconRect.y += 37; // Adjust Y to overlap the icon area
            iconRect.width = 40; // Set the rect width
            iconRect.height = 20; // Set the rect height

            // Create a GUIContent with the icon and label
            GUIContent buttonContent = new()
            {
                text = "    +"
            };

            // Create a custom GUIStyle for the button
            GUIStyle buttonStyle = new(GUI.skin.button)
                    {
                        padding = new RectOffset(2, 2, 2, 5),
                        border = new RectOffset(1, 1, 1, 1),
                        stretchWidth = true,
                        stretchHeight = true,
                    };
            // Change the button's background colors

            var textColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.8f);
            var backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.24f, 0.24f, 0.24f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);
            var hoverColor = EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);
            var activeColor = EditorGUIUtility.isProSkin ? new Color(0.45f, 0.45f, 0.45f, 1f) : new Color(0.6f, 0.6f, 0.6f, 1f);

            buttonStyle.normal.textColor = textColor; // Text color in normal state
            buttonStyle.normal.background = MakeTex(2, 2, backgroundColor); // Normal background color
            buttonStyle.hover.background = MakeTex(2, 2, hoverColor); // Background color on hover
            buttonStyle.active.background = MakeTex(2, 2, activeColor); // Background color when clicked

            // Draw the button
            if (GUI.Button(iconRect, buttonContent, buttonStyle))
            {
                ShowEmojiPopup();
            }
            if (m_Icon != null)
            {
                Rect iconPartRect = new Rect(iconRect.x + 9, iconRect.y + 2, 12, 12); // Position and size for the icon
                GUI.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0,0,0,0.5f); // Set transparency to 50%
                GUI.DrawTexture(iconPartRect, m_Icon);
                GUI.color = Color.white; // Reset GUI color to avoid affecting other elements
            }
        }
        private void ShowEmojiPopup()
        {
            // Create and show the popup window with the emoji selector
            UnityEditor.PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero), new SymbolsWindow(serializedObject));
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default GameObject inspector
            m_GameObjectInspector.OnInspectorGUI();
        }

        public override bool HasPreviewGUI()
        {
            return m_GameObjectInspector.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            m_GameObjectInspector.OnPreviewGUI(r, background);
        }

        public override string GetInfoString()
        {
            return m_GameObjectInspector.GetInfoString();
        }

        public override GUIContent GetPreviewTitle()
        {
            return m_GameObjectInspector.GetPreviewTitle();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            m_GameObjectInspector.OnInteractivePreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            m_GameObjectInspector.OnPreviewSettings();
        }

        public override void ReloadPreviewInstances()
        {
            m_GameObjectInspector.ReloadPreviewInstances();
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            return m_GameObjectInspector.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override bool RequiresConstantRepaint()
        {
            return m_GameObjectInspector.RequiresConstantRepaint();
        }

        public override bool UseDefaultMargins()
        {
            return m_GameObjectInspector.UseDefaultMargins();
        }
    }
}