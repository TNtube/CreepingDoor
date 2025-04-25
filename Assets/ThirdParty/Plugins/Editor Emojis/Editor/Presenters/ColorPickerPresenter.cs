using System;
using System.Collections.Generic;
using Anvil.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.EditorEmojis
{
    /// <summary>
        /// Handles switching of tab states.
        /// </summary>
    internal sealed class ColorPickerPresenter : Presenter
    {
        private readonly Button _colorPickerButton;
        private readonly VisualElement _colorPickerButtonIcon;
        private readonly List<Button> _colorSelectButtons = new(10);
        private readonly List<Action> _colorSelectActions = new(10);

        public ColorPickerPresenter(SymbolsModel model, SymbolsView view) : base(model, view)
        {
            _colorPickerButton = view.ColorPickerButton;
            _colorPickerButtonIcon = view.ColorPickerButtonIcon;
            _colorSelectButtons = view.ColorSelectButtons;

            
            Util.Log($"Color select button count: {_colorSelectButtons.Count}", LogFilter.View);
            

            for(int i = 0; i < _colorSelectButtons.Count; i++)
            {
                var button = _colorSelectButtons[i];
                var x = i;
                void selectAction() => OnColorSelected(x);
                button.clicked += selectAction;
                _colorSelectActions.Add(selectAction);

                button.clicked += () => Util.Log($"Color selected direct: {(IconColor)x}", LogFilter.View);
            }

            _colorPickerButton.clicked += OnColorPickerButtonClicked;
            OnCurrentIconColorChanged(Model.CurrentIconColor);
            Model.CurrentIconColor.OnValueChanged += OnCurrentIconColorChanged;
            OnCurrentIconColorChanged(Model.CurrentIconColor);
        }

        internal override void OnOpen()
        {
            OnCurrentIconColorChanged(Model.CurrentIconColor);
        }

        private void OnColorSelected(int x)
        {
            if(x < 0 || x >= _colorSelectButtons.Count)
            {
                Debug.LogError($"Invalid index: {x} for color select buttons");
                return;
            }

            Util.Log($"Color selected: {(IconColor)x}", LogFilter.View);

            // For example, x=4 will be IconColor.UnityGreen which equates to Color(0.694f, 0.988f, 0.349f) },
            Model.CurrentIconColor.Value = (IconColor)x;

            View.HideColorPicker();
        }

        private void OnCurrentIconColorChanged(IconColor iconColor)
        {
            Util.Log($"Color changed: {iconColor}", LogFilter.View);
            _colorPickerButtonIcon.style.backgroundColor = Constants.IconColors[iconColor];
        }

        private void OnColorPickerButtonClicked()
        {
            Util.Log($"Color picker button clicked", LogFilter.View);
            View.ToggleColorPicker();
        }

        public override void Dispose()
        {
            Model.CurrentIconColor.OnValueChanged -= OnCurrentIconColorChanged;
            _colorPickerButton.clicked -= OnColorPickerButtonClicked;

            for (int i = 0; i < _colorSelectButtons.Count; i++)
            {
                var button = _colorSelectButtons[i];
                var selectAction = _colorSelectActions[i];
                button.clicked -= selectAction;
            }

            _colorSelectActions.Clear();
        }
    }
}