using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Anvil.Common;

namespace Anvil.EditorEmojis
{
    /// <summary>
    /// Updates the list view any time RowItems changes.
    /// </summary>
    internal sealed class ListViewPresenter : Presenter
    {
        public ListViewPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            // Bind List View
            View.SymbolListView.itemsSource = Model.RowItems.Value;
            View.SymbolListView.makeItem = MakeListViewItem;
            View.SymbolListView.bindItem = BindListViewItem;

            Model.RowItems.OnValueChanged += UpdateListView;

            UpdateListView(Model.RowItems.Value);
        }

        internal override void OnOpen()
        {
            UpdateListView(Model.RowItems.Value);
        }

        private void UpdateListView(List<SymbolRowItem> list)
        {
            Util.Log("Updating List View", LogFilter.Controller);
            View.SymbolListView.itemsSource = null;
            View.SymbolListView.itemsSource = list;
            View.SymbolListView.Rebuild();
        }

        private void BindListViewItem(VisualElement element, int index)
        {
            var item = Model.RowItems.Value[index];
            element.Clear();

            if (item.IsSubtitle)
            {
                // Create and add subtitle label
                Label subtitleLabel = new(item.Subtitle);
                subtitleLabel.AddToClassList("subtitle-label");
                element.Add(subtitleLabel);
                return;
            }

            // Create a row container
            VisualElement rowContainer = new VisualElement();
            rowContainer.AddToClassList("symbol-row-container");

            foreach (var symbol in item.Emojis)
            {
                // We shouldn't check for null here as if the texture is null the icon should not even be added to data.
                Texture2D symbolTexture = symbol.PreviewTexture;

                Image symbolImage = new Image { image = symbolTexture };
                symbolImage.AddToClassList("symbol-image");
                symbolImage.tooltip = symbol.Name;

                // Register the event handler, passing the specific symbol
                if (Model.CurrentTabState == TabState.Emoji)
                {
                    symbolImage.RegisterCallback((ClickEvent evt) => EmojiSelected(symbol));
                }
                else if (Model.CurrentTabState == TabState.Icon)
                {
                    symbolImage.RegisterCallback((ClickEvent evt) => IconSelected(symbol));
                }

                rowContainer.Add(symbolImage);
            }
            element.Add(rowContainer);
        }

        private void EmojiSelected(Symbol symbol)
        {
            Model.AddRecentlyUsedEmoji(symbol);
            Model.OnSymbolClicked?.Invoke(symbol);
        }


        private void IconSelected(Symbol symbol)
        {
            Model.AddRecentlyUsedIcon(symbol);
            Model.OnSymbolClicked?.Invoke(symbol);
        }
        
        public VisualElement MakeListViewItem()
        {
            // Create a container for each item
            VisualElement container = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    alignItems = Align.FlexStart,
                    marginBottom = 5
                }
            };
            return container;
        }

        public override void Dispose()
        {
           View.SymbolListView.itemsSource = null;
           View.SymbolListView.makeItem = null;
           View.SymbolListView.bindItem = null;
           Model.RowItems.OnValueChanged -= UpdateListView;
        }
    }
}