using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using Anvil.Common;

namespace Anvil.EditorEmojis
{
    internal class SymbolsView : VisualElement
    {
        internal readonly ToolbarSearchField SearchField;
        internal readonly ListView SymbolListView;

        internal readonly Button EmojiTabButton;
        internal readonly Button IconTabButton;
        internal readonly Button SettingsButton;

        internal readonly Button RandomSymbolButton;
        internal readonly Button ColorPickerButton;
        internal readonly VisualElement ColorPickerButtonIcon;
        internal readonly VisualElement _colorPickerPopup;
        internal readonly List<Button> ColorSelectButtons = new(10);

        // Settings
        internal readonly VisualElement settingsContainer;
        internal readonly Toggle ShowHierarchyToggle;
        internal readonly Toggle ChangeHierarchyToggle;

        private TabState _currentTabState;

        internal SymbolsView()
        {
            Util.Log($"Initializing Symbols View", LogFilter.View);

            var uxmlPath = SymbolDataUtility.GetFilePath(Constants.SYMBOL_POPUP_UXML);
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            VisualElement template = uxml.CloneTree();
            template.style.width = Length.Percent(100);
            template.style.height = Length.Percent(100);

            // Tabs
            EmojiTabButton = template.Q<Button>("emoji-tab");
            IconTabButton = template.Q<Button>("icon-tab");
            SettingsButton = template.Q<Button>("settings-button");

            // Search Field Row
            SearchField = template.Q<ToolbarSearchField>("search-field");
            RandomSymbolButton = template.Q<Button>("random-button");
            ColorPickerButton = template.Q<Button>("color-picker-button");
            ColorPickerButtonIcon = ColorPickerButton.Q<VisualElement>("color-picker-button-icon");
            _colorPickerPopup = template.Q<VisualElement>("color-picker-container");

            for (int i = 0; i < 10; i++)
            {
                Button colorButton = _colorPickerPopup.Q<Button>($"color-select-button-{i}");
                ColorSelectButtons.Add(colorButton);
            }

            settingsContainer = template.Q<VisualElement>("settings-container");
            ShowHierarchyToggle = template.Q<Toggle>("show-icons-toggle");
            ChangeHierarchyToggle = template.Q<Toggle>("change-icons-toggle");

            // List View
            SymbolListView = template.Q<ListView>();
            SymbolListView.selectionType = SelectionType.None;
            SymbolListView.showBorder = false;
            SymbolListView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;

            HideColorPicker();

            var templateFileName = EditorGUIUtility.isProSkin ? Constants.SYMBOL_POPUP_STYLE_DARK : Constants.SYMBOL_POPUP_STYLE_LIGHT;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SymbolDataUtility.GetFilePath(templateFileName));
            template.styleSheets.Add(styleSheet);
            Add(template);
        }

        internal void ChangeTabState(TabState tabState)
        {
            _currentTabState = tabState;
            // Remove active class from all buttons
            EmojiTabButton.RemoveFromClassList("active-tab");
            IconTabButton.RemoveFromClassList("active-tab");
            SettingsButton.RemoveFromClassList("active-tab");

            // Add active class to the selected tab button
            switch (_currentTabState)
            {
                case TabState.Emoji:
                    EmojiTabButton.AddToClassList("active-tab");
                    ColorPickerButton.style.display = DisplayStyle.None;
                    HideSettings();
                    HideColorPicker();
                    break;
                case TabState.Icon:
                    IconTabButton.AddToClassList("active-tab");
                    ColorPickerButton.style.display = DisplayStyle.Flex;
                    HideSettings();
                    HideColorPicker();
                    break;
                case TabState.Settings:
                    SettingsButton.AddToClassList("active-tab");
                    ShowSettings();
                    HideColorPicker();
                    ColorPickerButton.style.display = DisplayStyle.None;
                    break;
            }
        }

        internal void ToggleColorPicker()
        {
            _colorPickerPopup.style.visibility = _colorPickerPopup.style.visibility == Visibility.Hidden
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        internal void HideColorPicker() => _colorPickerPopup.style.visibility = Visibility.Hidden;
        internal void ShowColorPicker() => _colorPickerPopup.style.visibility = Visibility.Visible;

        internal void ToggleSettings()
        {
            settingsContainer.style.display = settingsContainer.style.display == DisplayStyle.Flex
                ? DisplayStyle.None
                : DisplayStyle.Flex;
        }

        internal void HideSettings() => settingsContainer.style.display = DisplayStyle.None;
        internal void ShowSettings() => settingsContainer.style.display = DisplayStyle.Flex;
    }

}