using System;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    /// <summary>
    /// Updates the list view any time RowItems changes.
    /// </summary>
    internal sealed class SelectRandomPresenter : Presenter
    {
        private readonly Button _selectRandomButton;

        public SelectRandomPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            _selectRandomButton = View.RandomSymbolButton;
            _selectRandomButton.clicked += SelectRandom;
        }

        private void SelectRandom()
        {
            if(Model.CurrentTabState == TabState.Emoji)
            {
                var randomEmoji = Model.EmojiData.GetRandomSymbol();
                Model.EmojiData.AddRecentlyUsedSymbol(randomEmoji);
                Model.OnSymbolClicked?.Invoke(randomEmoji);
                return;
            }
            if(Model.CurrentTabState == TabState.Icon)
            {
                var randomIcon = Model.IconData.GetRandomSymbol();
                Model.IconData.AddRecentlyUsedSymbol(randomIcon);
                Model.OnSymbolClicked?.Invoke(randomIcon);
                return;
            }
        }

        public override void Dispose()
        {
            _selectRandomButton.clicked -= SelectRandom;
        }
    }
}