using System.Collections.Generic;
using UnityEngine;

namespace Anvil.EditorEmojis
{
    internal static class Constants
    {
        internal const string MAIN_DIRNAME = "Editor Emojis";
        /// <summary>
        /// The filename of the JSON file containing emoji data grouped by category.
        /// </summary>
        internal const string EMOJI_DATA_FILENAME = "emoji-data-by-group.json";

        /// <summary>
        /// The name of the directory containing emoji image files.
        /// </summary>
        internal const string EMOJI_IMAGE_DIRNAME = "Emoji Images";

        /// <summary>
        /// The filename of the JSON file containing icon data (to be added).
        /// </summary>
        internal const string ICON_DATA_FILENAME = "icon-data-by-group.json";

        /// <summary>
        /// The name of the directory containing icon image files (to be added).
        /// </summary>
        internal const string ICON_IMAGE_DIRNAME = "Icon Images";

        /// <summary>
        /// The filename of the UXML file for the icon popup UI.
        /// </summary>
        internal const string SYMBOL_POPUP_UXML = "SymbolPopup.uxml";

        /// <summary>
        /// The filename of the USS file for styling the icon popup UI.
        /// </summary>
        internal const string SYMBOL_POPUP_STYLE = "SymbolPopup.uss";
        internal const string SYMBOL_POPUP_STYLE_DARK = "SymbolPopupDark.uss";
        internal const string SYMBOL_POPUP_STYLE_LIGHT = "SymbolPopupLight.uss";

        internal static readonly Dictionary<IconColor, Color> IconColors = new()
            {
                { IconColor.White, new Color(0.95f, 0.95f, 0.95f) },
                { IconColor.Gray, new Color(0.5f, 0.5f, 0.5f) },
                { IconColor.Black, new Color(0.05f, 0.05f, 0.05f) },
                { IconColor.PrefabBlue, new Color(0.498f, 0.839f, 0.992f) },
                { IconColor.UnityGreen, new Color(0.694f, 0.988f, 0.349f) },
                { IconColor.Green, new Color(0.290f, 0.690f, 0.492f) },
                { IconColor.Blue, new Color(0.290f, 0.583f, 0.895f) },
                { IconColor.Purple, new Color(0.645f, 0.459f, 0.838f) },
                { IconColor.Pink, new Color(0.866f, 0.360f, 0.570f) },
                { IconColor.Red, new Color(0.883f, 0.394f, 0.377f) }
            };
    }
}