using System.Collections.Generic;

namespace Anvil.EditorEmojis
{
    // Updated EmojiRowItem class
    internal class SymbolRowItem
    {
        internal bool IsSubtitle { get; private set; }
        internal string Subtitle { get; private set; }
        internal List<Symbol> Emojis { get; private set; }

        // Constructor for subtitle
        internal SymbolRowItem(string subtitle)
        {
            IsSubtitle = true;
            Subtitle = subtitle;
            Emojis = null;
        }

        // Constructor for emoji row
        internal SymbolRowItem(List<Symbol> emojis)
        {
            IsSubtitle = false;
            Emojis = emojis;
            Subtitle = null;
        }
    }
}
