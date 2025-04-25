using System;
using System.Collections.Generic;
using Anvil.Common;
using UnityEngine;

namespace Anvil.EditorEmojis
{
    internal class SymbolsData
    {
        public event Action<List<SymbolGroup>> OnFilteredSymbolsChanged;
        // Default loaded emojis. Represent actual data.
        private readonly List<SymbolGroup> DefaultSymbolGroups;
        // Recently used emojis are not duplicates, but reference existing symbols.
        private readonly SymbolGroup RecentlyUsedSymbolGroup;
        // Represents all symbols, including duplicates.
        internal readonly List<SymbolGroup> AllSymbolGroups;
        // Represents filtered symbols.
        internal readonly List<SymbolGroup> FilteredSymbolGroups;

        internal SymbolsData(string jsonFilename, string symbolImageDir, IconColor iconColor = IconColor.None)
        {
            // Load the default emoji data.
            DefaultSymbolGroups = LoadSymbolData(jsonFilename, symbolImageDir, iconColor);
            RecentlyUsedSymbolGroup = new SymbolGroup("Recent", new List<Symbol>());
            AllSymbolGroups = new List<SymbolGroup> { RecentlyUsedSymbolGroup };
            AllSymbolGroups.AddRange(DefaultSymbolGroups);
            FilteredSymbolGroups = new List<SymbolGroup>(AllSymbolGroups);
            OnFilteredSymbolsChanged?.Invoke(FilteredSymbolGroups);
        }

        /// <summary>
        /// Load all available emoji images once.
        /// </summary>
        private List<SymbolGroup> LoadSymbolData(string jsonFileName, string imageDirName, IconColor color)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var emojiJsonPath = SymbolDataUtility.GetFilePath(jsonFileName);
            var symbolDataJson = SymbolDataUtility.LoadSymbolData(emojiJsonPath);
            var emojiImageDir = SymbolDataUtility.GetDirPath(imageDirName);

            var deserializationTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            var symbolGroups = new List<SymbolGroup>();

            Util.Log($"\tDeserialized in {deserializationTime}ms", LogFilter.Model);

            foreach (var group in symbolDataJson.groups)
            {
                var groupName = group.name;
                var groupIcons = group.icons;
                var symbolGroup = new SymbolGroup(groupName, new List<Symbol>());
                symbolGroups.Add(symbolGroup);
                foreach (var icon in groupIcons)
                {
                    var symbol = new Symbol(icon.name, icon.imagename);
                    symbol.Initialize(emojiImageDir, color);
                    symbolGroup.Icons.Add(symbol);
                }
            }

            var initializationTime = stopwatch.ElapsedMilliseconds;
            Util.Log($"\tInitialized in {initializationTime}ms", LogFilter.Model);
            stopwatch.Stop();

            return symbolGroups;
        }

        internal void AddRecentlyUsedSymbol(Symbol symbol)
        {
            if (!RecentlyUsedSymbolGroup.Icons.Contains(symbol))
            {
                RecentlyUsedSymbolGroup.Icons.Insert(0, symbol);
                if (RecentlyUsedSymbolGroup.Icons.Count > 10)
                {
                    RecentlyUsedSymbolGroup.Icons.RemoveAt(10);
                }
            }
            else
            {
                RecentlyUsedSymbolGroup.Icons.Remove(symbol);
                RecentlyUsedSymbolGroup.Icons.Insert(0, symbol);
            }
        }

        internal void ChangeSymbolsColor(IconColor color)
        {
            foreach (var group in AllSymbolGroups)
            {
                foreach (var symbol in group.Icons)
                {
                    symbol.ChangeColor(color);
                }
            }
        }

        internal void SetFilteredSymbolGroups(List<SymbolGroup> filteredGroups)
        {
            FilteredSymbolGroups.Clear();
            FilteredSymbolGroups.AddRange(filteredGroups);
            OnFilteredSymbolsChanged?.Invoke(FilteredSymbolGroups);
        }

        internal Symbol GetRandomSymbol()
        {
            var random = new System.Random();
            var randomIndex = random.Next(0, AllSymbolGroups.Count);
            var randomGroup = AllSymbolGroups[randomIndex];
            var randomIndexInGroup = random.Next(0, randomGroup.Icons.Count);
            return randomGroup.Icons[randomIndexInGroup];
        }
    }
}