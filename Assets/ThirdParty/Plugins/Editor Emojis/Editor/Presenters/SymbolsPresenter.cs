using Anvil.Common;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    /// <summary>
    /// Acts as a config for the other presetners.
    /// </summary>
    internal sealed class SymbolsPresenter : Presenter
    {
        // Nav presenters
        private readonly TabsPresenter _tabsPresenter;

        // Tool bar presenters
        private readonly SearchFilterPresenter _searchFilterPresenter;
        private readonly SelectRandomPresenter _selectRandomPresenter;
        private readonly ColorPickerPresenter _colorPickerPresenter;

        private readonly ListViewPresenter _listViewPresenter;

        private readonly SettingsPresenter _settingsPresenter;

        public SymbolsPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            _tabsPresenter = new TabsPresenter(model, view);

            _searchFilterPresenter = new SearchFilterPresenter(model, view);
            _selectRandomPresenter = new SelectRandomPresenter(model, view);
            _colorPickerPresenter = new ColorPickerPresenter(model, view);

            _listViewPresenter = new ListViewPresenter(model, view);

            _settingsPresenter = new SettingsPresenter(model, view);
        }

        internal override void OnOpen()
        {
            _tabsPresenter.OnOpen();
            _searchFilterPresenter.OnOpen();
            _selectRandomPresenter.OnOpen();
            _colorPickerPresenter.OnOpen();
            _listViewPresenter.OnOpen();
            _settingsPresenter.OnOpen();
        }

        public override void Dispose()
        {
            _tabsPresenter.Dispose();

            _searchFilterPresenter.Dispose();
            _selectRandomPresenter.Dispose();
            _colorPickerPresenter.Dispose();

            _listViewPresenter.Dispose();

            _settingsPresenter.Dispose();
        }
    }
}