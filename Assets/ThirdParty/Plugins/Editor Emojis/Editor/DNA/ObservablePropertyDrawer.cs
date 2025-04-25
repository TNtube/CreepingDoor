using UnityEditor;
using UnityEngine;

namespace Anvil.Common
{
    [CustomPropertyDrawer(typeof(ObservableBool))]
    internal sealed class ObservableBoolDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var value = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ObservableString))]
    internal sealed class ObservableStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var value = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ObservableInt))]
    internal sealed class ObservableIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var value = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }
    
}