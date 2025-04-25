using System;
using Anvil.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    internal sealed class SettingsPresenter : Presenter
    {
        public SettingsPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            Util.Log("Initializing Settings Presenter", LogFilter.Controller);
            view.ShowHierarchyToggle.RegisterValueChangedCallback(OnShowHierarchyToggleChanged);
            view.ChangeHierarchyToggle.RegisterValueChangedCallback(OnChangeHierarchyToggleChanged);

            view.ShowHierarchyToggle.SetValueWithoutNotify(model.ShowIconsInHierarchy.Value);
            view.ChangeHierarchyToggle.SetValueWithoutNotify(model.ChangeIconsInHierarchy.Value);
        }

        private void OnChangeHierarchyToggleChanged(ChangeEvent<bool> evt)
        {
            Model.ChangeIconsInHierarchy.Value = evt.newValue;
        }

        private void OnShowHierarchyToggleChanged(ChangeEvent<bool> evt)
        {
            // Change toggle should only be visible when show icons is enabled
            View.ChangeHierarchyToggle.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            Model.ShowIconsInHierarchy.Value = evt.newValue;
        }

        public override void Dispose()
        {
            View.ShowHierarchyToggle.UnregisterValueChangedCallback(OnShowHierarchyToggleChanged);
            View.ChangeHierarchyToggle.UnregisterValueChangedCallback(OnChangeHierarchyToggleChanged);
        }
    }
}
