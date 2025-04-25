using System;
using Anvil.Common;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    /// <summary>
        /// Handles switching of tab states.
        /// </summary>
    internal sealed class TabsPresenter : Presenter
    {
        private readonly Button _emojiTabButton;
        private readonly Button _iconTabButton;
        private readonly Button _settingButton;

        public TabsPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            _emojiTabButton = view.EmojiTabButton;
            _iconTabButton = view.IconTabButton;
            _settingButton = view.SettingsButton;

            _emojiTabButton.clicked += OnEmojiTabButtonClicked;
            _iconTabButton.clicked += OnIconTabButtonClicked;
            _settingButton.clicked += OnSettingButtonClicked;

            Model.CurrentTabState.OnValueChanged += View.ChangeTabState;
            View.ChangeTabState(Model.CurrentTabState);
        }

        private void OnSettingButtonClicked()
        {
            Model.CurrentTabState.Value = TabState.Settings;
        }

        internal override void OnOpen()
        {
            View.ChangeTabState(Model.CurrentTabState);
        }

        private void OnEmojiTabButtonClicked()
        {
            Util.Log("Emoji Tab Clicked", LogFilter.Controller);
            Model.CurrentTabState.Value = TabState.Emoji;
        }

        private void OnIconTabButtonClicked()
        {
            Util.Log("Icon Tab Clicked", LogFilter.Controller);
            Model.CurrentTabState.Value = TabState.Icon;
        }

        public override void Dispose()
        {
            Model.CurrentTabState.OnValueChanged -= View.ChangeTabState;
            _emojiTabButton.clicked -= OnEmojiTabButtonClicked;
            _iconTabButton.clicked -= OnIconTabButtonClicked;
        }
    }
}