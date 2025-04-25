using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.Common
{
    public static class UIExtensions
    {
        #region Visual Element Bindings
        /// <summary>
        /// Bind a container's visibility to an observable bool.
        /// </summary>
        internal static void BindDisplay(this VisualElement container, ObservableBool observable, bool invert = false)
        {
            observable.OnValueChanged += Value => container.style.display = Value ^ invert ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Bind a status icon to an observable bool.
        /// </summary>
        internal static void BindBackgroundColour(this VisualElement icon, ObservableBool observable, Color trueColor = default, Color falseColor = default)
        {
            if (trueColor == default) trueColor = Color.green;
            if (falseColor == default) falseColor = Color.red;
            observable.OnValueChanged += Value => icon.style.backgroundColor = new StyleColor(Value ? trueColor : falseColor);
        }

        /// <summary>
        /// Binds the tooltip of a VisualElement to an ObservableString.
        /// </summary>
        internal static void BindTooltip(this VisualElement element, ObservableString observableString)
        {
            element.tooltip = observableString.Value;
            observableString.OnValueChanged += Value => element.tooltip = Value;
        }

        /// <summary>
        /// Binds the tooltip of a VisualElement to an ObservableBool with specific messages for true and false states.
        /// </summary>
        internal static void BindTooltip(this VisualElement element, ObservableBool observableString, string trueMessage, string falseMessage)
        {
            element.tooltip = observableString.Value ? trueMessage : falseMessage;
            observableString.OnValueChanged += Value => element.tooltip = Value ? trueMessage : falseMessage;
        }

        /// <summary>
        /// Binds the tooltip of a VisualElement to an ObservableBool with dynamic messages for true and false states.
        /// </summary>
        internal static void BindTooltip(this VisualElement element, ObservableBool observableString, Func<string> trueMessage, Func<string> falseMessage)
        {
            element.tooltip = observableString.Value ? trueMessage() : falseMessage();
            observableString.OnValueChanged += Value => element.tooltip = Value ? trueMessage() : falseMessage();
        }

        # endregion

        # region Toggle

        /// <summary>
        /// Two way bind a toggle to an observable bool.
        /// </summary>
        internal static void BindToggle(this Toggle toggle, ObservableBool observable)
        {
            toggle.value = observable.Value;
            toggle.userData = observable;
            toggle.RegisterValueChangedCallback(ToggleValueChanged);
            observable.OnValueChanged += value => toggle.SetValueWithoutNotify(value);
        }

        /// <summary>
        /// Unbind the toggle from an obserrvable bool. Observables should be unbind manually.
        /// </summary>
        internal static void UnBind(this Toggle toggle, ObservableBool observable)
        {
            toggle.UnregisterValueChangedCallback(ToggleValueChanged);
        }

        // Static method to handle the toggle's value change
        private static void ToggleValueChanged(ChangeEvent<bool> evt)
        {
            if (evt.target is Toggle toggle && toggle.userData is ObservableBool observable)
            {
                observable.Value = evt.newValue;
            }
        }

        # endregion

        # region Label

        /// <summary>
        /// Bind a label to an observable string.
        /// </summary>
        internal static void BindLabel(this Label label, ObservableString observable)
        {
            label.userData = observable;
            observable.OnValueChanged += Value => label.text = Value;
        }

        /// <summary>
        /// Bind a label to an observable bool, with custom messages for true and false.
        /// </summary>
        internal static void BindLabel(this Label label, ObservableBool observable, string trueMessage, string falseMessage)
        {
            observable.OnValueChanged += Value => label.text = Value ? trueMessage : falseMessage;
        }

        /// <summary>
        /// Bind a label to an observable bool, with custom Func<string> for true and false. This allows us to use $"{variable}" in the message.
        /// </summary>
        internal static void BindLabel(this Label label, ObservableBool observable, Func<string> trueMessage, Func<string> falseMessage)
        {
            observable.OnValueChanged += Value => label.text = Value ? trueMessage() : falseMessage();
        }

        /// <summary>
        /// Bind a label to an observable int, with custom Func<string> for the message.
        /// </summary>
        internal static void BindLabel(this Label label, ObservableInt observable, Func<string> message)
        {
            observable.OnValueChanged += Value => label.text = message();
        }
        # endregion




        /// <summary>
        /// Bind a button's enabled state to an observable bool.
        /// </summary>
        internal static void BindButtonState(this Button button, ObservableBool observable, bool invert = false)
        {
            observable.OnValueChanged += value => button.SetEnabled(value ^ invert);
        }



        /// <summary>
        /// Bind a button's enabled state to an observable bool and customize the text.
        /// </summary>
        internal static void BindButtonState(this Button button, ObservableBool observable, string enableText, string disableText, bool invert = false)
        {
            observable.OnValueChanged += value => button.SetEnabled(value ^ invert);
            observable.OnValueChanged += Value => button.text = Value ? enableText : disableText;
        }

        /// <summary>
        /// Bind a button's text to an observable string.
        /// </summary>
        internal static void BindButtonText(this Button button, ObservableString observable)
        {
            observable.OnValueChanged += Value => button.text = Value;
        }

        /// <summary>
        /// Bind a button's text to an observable bool, with custom messages for true and false.
        /// </summary>
        internal static void BindButtonText(this Button button, ObservableBool observable, string trueText, string falseText)
        {
            observable.OnValueChanged += Value => button.text = Value ? trueText : falseText;
        }
    }
}