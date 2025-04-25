using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.Common;
using UnityEditor;

namespace Anvil.EditorEmojis
{
    /// <summary>
    /// Represents emoji popup data.
    /// </summary>
    internal class SymbolsModel
    {
        internal static readonly SymbolsModel Instance = new();
        // This is pretty ugly but better than injected the editor window.
        internal Action<Symbol> OnSymbolClicked;

        internal readonly SymbolsData EmojiData;
        internal readonly SymbolsData IconData;
        // There is only ever one 'visible' filtered list.
        internal readonly ObservableList<SymbolGroup> CurrentFilteredSymbolGroups = new();

        internal bool DarkMode => EditorGUIUtility.isProSkin;
        internal readonly Observable<TabState> CurrentTabState = new(TabState.Emoji);
        internal readonly Observable<string> SearchFilter = new(string.Empty);
        internal readonly Observable<IconColor> CurrentIconColor = new();
        internal readonly ObservableList<SymbolRowItem> RowItems = new();

        // Settings Data
        internal readonly Observable<bool> ShowIconsInHierarchy = new(true);
        internal readonly Observable<bool> ChangeIconsInHierarchy = new(true);
        internal readonly string SettingsFilePath = "";

        internal SymbolsModel()
        {
            Util.Log("Initializing Symbols Model", LogFilter.Model);
            CurrentTabState.Value = TabState.Emoji;
            SearchFilter.Value = "";
            CurrentIconColor.Value = DarkMode ? IconColor.White : IconColor.Black;
            SearchFilter.Value = "";

            Util.Log("Loading Emoji Data", LogFilter.Model);
            EmojiData = new SymbolsData(Constants.EMOJI_DATA_FILENAME, Constants.EMOJI_IMAGE_DIRNAME);
            Util.Log("Loading Icon Data", LogFilter.Model);
            IconData = new SymbolsData(Constants.ICON_DATA_FILENAME, Constants.ICON_IMAGE_DIRNAME, CurrentIconColor);

            SetUpReactionChains();
            SetupDebug();

            // Create the initial row items
            UpdateCurrentFilteredSymbolsGroup(SearchFilter);

            // Set up the settings
            SettingsFilePath = SymbolDataUtility.GetDirPath(Constants.MAIN_DIRNAME) + "/settings.json";
            LoadSettingsData();
            ShowIconsInHierarchy.OnValueChanged += (bool showIconsInHierarchy) => SaveSettingsData();
            ChangeIconsInHierarchy.OnValueChanged += (bool changeIconsInHierarchy) => SaveSettingsData();
        }
        /// <summary>
        /// Defines how the models data relates to eachother.
        /// </summary>
        private void SetUpReactionChains()
        {
            // When the tab changes, we update filtered symbols but don't immediately trigger row updates
            CurrentTabState.OnValueChanged += _ =>
            {
                UpdateCurrentFilteredSymbolsGroup(SearchFilter);
            };

            // When the search filter changes, update the filtered symbols but don't trigger row updates
            SearchFilter.OnValueChanged += searchFilter =>
            {
                UpdateCurrentFilteredSymbolsGroup(searchFilter);
            };

            // When filtered symbols actually change, update the row items
            CurrentFilteredSymbolGroups.OnValueChanged += _ =>
            {
                UpdateRowItems(CurrentFilteredSymbolGroups);
            };

            // When the icon color changes, update icons and only trigger row updates if the tab is 'Icon'
            CurrentIconColor.OnValueChanged += color =>
            {
                IconData.ChangeSymbolsColor(color);
                if (CurrentTabState == TabState.Icon)
                {
                    UpdateCurrentFilteredSymbolsGroup(SearchFilter); // Update symbols before updating row items
                }
            };

            // if we can't see icons, we can't change them!
            ShowIconsInHierarchy.OnValueChanged += showIconsInHierarchy =>
            {
                if (!showIconsInHierarchy)
                    ChangeIconsInHierarchy.Value = false;
            };
        }

        private void SetupDebug()
        {
            SearchFilter.OnValueChanged += (string searchTerm) => Util.Log($"Search Filter: {searchTerm}", LogFilter.Model);
            CurrentTabState.OnValueChanged += (TabState tabState) => Util.Log($"Current Tab State: {tabState}", LogFilter.Model);
            CurrentIconColor.OnValueChanged += (IconColor color) => Util.Log($"Current Icon Color: {color}", LogFilter.Model);
            CurrentFilteredSymbolGroups.OnValueChanged += (List<SymbolGroup> symbolGroups) => Util.Log($"Current Filtered Symbol Groups: {symbolGroups.Count}", LogFilter.Model);
            RowItems.OnValueChanged += (List<SymbolRowItem> rowItems) => Util.Log($"Current Row Items: {rowItems.Count}", LogFilter.Model);
            ShowIconsInHierarchy.OnValueChanged += (bool showIconsInHierarchy) => Util.Log($"Show Icons In Hierarchy: {showIconsInHierarchy}", LogFilter.Model);
            ChangeIconsInHierarchy.OnValueChanged += (bool changeIconsInHierarchy) => Util.Log($"Change Icons In Hierarchy: {changeIconsInHierarchy}", LogFilter.Model);
        }

        internal void AddRecentlyUsedEmoji(Symbol symbol)
        {
            Util.Log("Adding recently used symbol to Emoji Data", LogFilter.Model);
            EmojiData.AddRecentlyUsedSymbol(symbol);
            UpdateCurrentFilteredSymbolsGroup(SearchFilter);
        }

        internal void AddRecentlyUsedIcon(Symbol symbol)
        {
            Util.Log("Adding recently used symbol to Icon Data", LogFilter.Model);
            IconData.AddRecentlyUsedSymbol(symbol);
            UpdateCurrentFilteredSymbolsGroup(SearchFilter);
        }


        // When the search filter changes, update the filtered symbols based on the current tab state.
        private void UpdateCurrentFilteredSymbolsGroup(string searchFilter)
        {
            if (CurrentTabState == TabState.Emoji)
            {
                EmojiData.SetFilteredSymbolGroups(FilterSymbols(searchFilter, EmojiData.AllSymbolGroups));
                CurrentFilteredSymbolGroups.Value = EmojiData.FilteredSymbolGroups;
            }
            else if (CurrentTabState == TabState.Icon)
            {
                IconData.SetFilteredSymbolGroups(FilterSymbols(searchFilter, IconData.AllSymbolGroups));
                CurrentFilteredSymbolGroups.Value = IconData.FilteredSymbolGroups;
            }
        }

        private List<SymbolGroup> FilterSymbols(string searchTerm, List<SymbolGroup> allSymbolGroups)
        {
            var filteredGroups = new List<SymbolGroup>();

            // Process the search term
            string lowerSearchTerm = searchTerm?.Trim().ToLower() ?? string.Empty;

            // If the search term is empty, return all groups with "Recent" at the top
            if (string.IsNullOrEmpty(lowerSearchTerm))
            {
                filteredGroups.AddRange(allSymbolGroups);
                return filteredGroups;
            }

            // Filter through all other groupsz
            foreach (var group in allSymbolGroups)
            {
                bool isGroupMatch = group.GroupName.ToLower().Contains(lowerSearchTerm);

                if (isGroupMatch)
                {
                    // If group name matches, include all icons in this group
                    filteredGroups.Add(new SymbolGroup
                    {
                        GroupName = group.GroupName,
                        Icons = new List<Symbol>(group.Icons) // Clone the list to prevent unintended modifications
                    });
                }
                else
                {
                    // Otherwise, filter icons within the group
                    var matchedIcons = group.Icons
                        .Where(icon => icon.Name.ToLower().Contains(lowerSearchTerm))
                        .ToList();

                    if (matchedIcons.Count > 0)
                    {
                        filteredGroups.Add(new SymbolGroup
                        {
                            GroupName = group.GroupName,
                            Icons = matchedIcons
                        });
                    }
                }
            }
            return filteredGroups;
        }

        public void UpdateRowItems(List<SymbolGroup> filteredSymbols)
        {
            Util.Log($"Updating Row Items", LogFilter.Model);
            RowItems.Value = BuildRowItems(filteredSymbols);
        }

        private List<SymbolRowItem> BuildRowItems(List<SymbolGroup> symbolGroups)
        {
            List<SymbolRowItem> rowItems = new();

            foreach (var group in symbolGroups)
            {
                // Skip groups with no symbols
                if (group.Icons == null || group.Icons.Count == 0)
                    continue;

                // Add subtitle row
                rowItems.Add(new SymbolRowItem(group.GroupName));

                // Break symbols into rows of 10
                var symbolsInGroup = group.Icons;
                int totalSymbols = symbolsInGroup.Count;
                int symbolsPerRow = 10;

                for (int i = 0; i < totalSymbols; i += symbolsPerRow)
                {
                    var symbolRow = symbolsInGroup.GetRange(i, Math.Min(symbolsPerRow, totalSymbols - i));
                    rowItems.Add(new SymbolRowItem(symbolRow));
                }
            }
            return rowItems;
        }

        private void SaveSettingsData()
        {
            Util.Log("Saving Settings Data", LogFilter.Model);
            var saveData = new SymbolModelSaveData
            {
                ShowIconsInHierarchy = ShowIconsInHierarchy,
                ChangeIconsInHierarchy = ChangeIconsInHierarchy
            };
            var json = UnityEditor.EditorJsonUtility.ToJson(saveData);
            System.IO.File.WriteAllText(SettingsFilePath, json);
        }

        private void LoadSettingsData()
        {
            Util.Log("Loading Settings Data", LogFilter.Model);
            if (System.IO.File.Exists(SettingsFilePath))
            {
                var json = System.IO.File.ReadAllText(SettingsFilePath);
                var loadedData = new SymbolModelSaveData();
                EditorJsonUtility.FromJsonOverwrite(json, loadedData);
                if (loadedData != null)
                {
                    ShowIconsInHierarchy.Value = loadedData.ShowIconsInHierarchy;
                    ChangeIconsInHierarchy.Value = loadedData.ChangeIconsInHierarchy;
                }
            }
            else
            {
                Util.Log("Settings file not found. Using default values.", LogFilter.Model);
            }

        }
    }

    [Serializable]
    public class SymbolModelSaveData
    {
        public bool ShowIconsInHierarchy;
        public bool ChangeIconsInHierarchy;
    }

}