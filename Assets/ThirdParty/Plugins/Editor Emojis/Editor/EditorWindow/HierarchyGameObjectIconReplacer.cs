using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Anvil.EditorEmojis
{
    [InitializeOnLoad]
    public static class HierarchyGameObjectIconReplacer
    {
        static HierarchyGameObjectIconReplacer()
        {
            additionalSelectedInstanceIDs = new HashSet<int>();
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.projectChanged -= ClearCache;
            EditorApplication.projectChanged += ClearCache;
            EditorApplication.hierarchyChanged -= ClearCache;
            EditorApplication.hierarchyChanged += ClearCache;
        }

        private static bool hierarchyHasFocus;
        private static EditorWindow hierarchyEditorWindow;
        private static readonly HashSet<int> additionalSelectedInstanceIDs;
        private static Dictionary<int, Texture2D> iconCache = new Dictionary<int, Texture2D>();

        private static void OnEditorUpdate()
        {


            if (!hierarchyEditorWindow && IsHierarchyWindowFocused())
            {
                hierarchyEditorWindow = EditorWindow.GetWindow(
                    Type.GetType($"{nameof(UnityEditor)}.SceneHierarchyWindow,{nameof(UnityEditor)}"));
            }

            hierarchyHasFocus = EditorWindow.focusedWindow
                && EditorWindow.focusedWindow == hierarchyEditorWindow;

            additionalSelectedInstanceIDs.Clear();
        }

        private static bool IsHierarchyWindowFocused()
        {
            EditorWindow focusedWindow = EditorWindow.focusedWindow;
            return focusedWindow != null && focusedWindow.GetType().Name == "SceneHierarchyWindow";
        }

        private static void ClearCache()
        {
            iconCache.Clear();
        }
        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (!SymbolsModel.Instance.ShowIconsInHierarchy) return;

            // Get the GameObject associated with the instanceID
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            // Attempt to get the custom icon from the cache
            Texture2D customIcon = GetCustomIconFromCache(instanceID, obj);
            // If it's not in cache, try to get it from the GameObject
            Texture2D iconToDraw = customIcon != null ? customIcon : EditorGUIUtility.ObjectContent(obj, obj.GetType()).image as Texture2D;

            if (iconToDraw == null) return;

            // Determine if the GameObject is active
            bool isActive = obj.activeInHierarchy;

            Rect iconRect;
            if (iconToDraw.name == "d_GameObject Icon" || iconToDraw.name == "d_Prefab Icon")
            {
                // Draw a see-through rect that can still be selected
                iconRect = new Rect(selectionRect.x, selectionRect.y, 14, 14);
                EditorGUI.DrawRect(iconRect, new Color(0, 0, 0, 0));
            }
            else
            {
                HierarchyObjectState objectState = GetHierarchyObjectState(selectionRect, instanceID);
                // Clear the original icon area
                CoverOriginalIcon(objectState, selectionRect);
                // Calculate the position of the icon (same as default icon position)
                iconRect = new Rect(selectionRect.x + 0, selectionRect.y, 14, 14);

                // Apply a gray color tint if the GameObject is not active
                Color originalColor = GUI.color;
                if (!isActive)
                {
                    GUI.color = Color.gray;
                }

                // Draw the icon
                GUI.DrawTexture(iconRect, iconToDraw);

                // Restore the original GUI color
                GUI.color = originalColor;
            }

            //if (!SymbolsModel.Instance.ChangeIconsInHierarchy) return;
            HandleIconClick(iconRect, obj);
        }


        private static HierarchyObjectState GetHierarchyObjectState(Rect selectionRect, int instanceID)
        {
            HierarchyObjectState objectStatus = new();

            Rect entireRowRect = selectionRect;
            entireRowRect.x = 0;
            entireRowRect.width = short.MaxValue;
            Rect expandChildrenIconRect = selectionRect;
            expandChildrenIconRect.x -= 14f;
            expandChildrenIconRect.width = 11f;

            objectStatus.IsSelected = Selection.instanceIDs.Contains(instanceID);
            objectStatus.IsHovered = entireRowRect.Contains(Event.current.mousePosition);
            objectStatus.IsDropDownHovered = expandChildrenIconRect.Contains(Event.current.mousePosition);

            return objectStatus;
        }

        private static void HandleIconClick(Rect iconRect, GameObject obj)
        {
            Event currentEvent = Event.current;

            if (!iconRect.Contains(currentEvent.mousePosition)) return;
            if (currentEvent.type != EventType.MouseDown) return;
            if (currentEvent.button != 0) return;
            if (!currentEvent.control) return;

            // Prevent the event from propagating further
            currentEvent.Use();
            ShowEmojiPopup(obj);
        }

        private static void ShowEmojiPopup(GameObject obj)
        {
            // Create and show the popup window with the emoji selector
            Rect popupRect = new(Event.current.mousePosition, Vector2.zero);
            SymbolsWindow popupWindow = new(obj);
            PopupWindow.Show(popupRect, popupWindow);
        }

        private static void CoverOriginalIcon(HierarchyObjectState hierarchyObjectStatus, Rect selectionRect)
        {
            // Define the area of the default icon
            Rect iconRect = new(selectionRect.x + 0, selectionRect.y, 16, 16);
            int selectedAmount = Selection.instanceIDs.Length > 1 ? Selection.instanceIDs.Length : additionalSelectedInstanceIDs.Count;

            // Set the background color based on the hierarchy object status to match what it would usually be
            Color backgroundColor = UnityEditorBackgroundColor.Get(hierarchyObjectStatus, hierarchyHasFocus, selectedAmount);

            // Draw over the icon area with the background color to "erase" the default icon
            EditorGUI.DrawRect(iconRect, backgroundColor);
        }

        private static Texture2D GetCustomIconFromCache(int instanceID, GameObject obj)
        {
            if (iconCache.TryGetValue(instanceID, out Texture2D customIcon)) return customIcon;

            // Use SerializedObject to access the 'm_Icon' property
            SerializedObject serializedObject = new SerializedObject(obj);
            SerializedProperty iconProperty = serializedObject.FindProperty("m_Icon");

            // Attempt to get the icon from the property as a Texture 2D
            customIcon = iconProperty != null && iconProperty.objectReferenceValue != null
            ? iconProperty.objectReferenceValue as Texture2D
            : null;

            // Store the result in the cache (even if it's null to avoid repeated checks)
            iconCache[instanceID] = customIcon;

            return customIcon;
        }
    }
}