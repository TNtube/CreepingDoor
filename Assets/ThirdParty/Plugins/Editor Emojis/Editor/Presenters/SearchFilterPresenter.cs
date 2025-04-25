using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    internal sealed class SearchFilterPresenter : Presenter
    {
        private readonly ToolbarSearchField _searchField;

        internal SearchFilterPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            _searchField = view.SearchField;
            _searchField.SetValueWithoutNotify(Model.SearchFilter);
            _searchField.RegisterValueChangedCallback(OnSearchFieldChanged);
            _searchField.value = Model.SearchFilter;
        }

        private void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            Model.SearchFilter.Value = evt.newValue.Trim().ToLower();
        }

        public override void Dispose()
        {
            _searchField.UnregisterValueChangedCallback(OnSearchFieldChanged);
        }
    }
}