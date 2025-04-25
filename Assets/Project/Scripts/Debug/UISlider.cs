using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


namespace Project.Utils
{
    [RequireComponent(typeof(Slider))]
    [AddComponentMenu("Project/Utils/UI Slider")]
    public class UISlider : MonoBehaviour
    {
        [Header("Property Settings")]
        [SerializeField]
        private Text showValueText;
        [SerializeField]
        private MonoBehaviour propertyHolder;
        [SerializeField]
        private string propertyName = string.Empty;

        [Header("Slider Settings")]
        [SerializeField]
        private float minValue = 0f;
        [SerializeField]
        private float maxValue = 1f;

        private string Key => $"{transform.root.gameObject.name}-{propertyName}";

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        private Slider slider;

        void Awake()
        {
            slider = GetComponent<Slider>();
            slider.minValue = minValue;
            slider.maxValue = maxValue;

            float value;
            if (PlayerPrefs.HasKey(Key)) value = PlayerPrefs.GetFloat(Key);
            else value = GetValue();

            slider.onValueChanged.AddListener(SetValue);
            slider.value = Mathf.Clamp(value, minValue, maxValue);
        }

        void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(SetValue);
        }

        private float GetValue()
        {
            try {
                FieldInfo fieldInfo = propertyHolder.GetType().GetField(propertyName, flags);
                return (float)fieldInfo.GetValue(propertyHolder);
            } catch { return Mathf.Lerp(minValue, maxValue, 0.5f); }
        }

        private void SetValue(float value)
        {
            try {
                PlayerPrefs.SetFloat(Key, value);
                PlayerPrefs.Save();
                FieldInfo fieldInfo = propertyHolder.GetType().GetField(propertyName, flags);
                fieldInfo.SetValue(propertyHolder, value);
            } catch { }
            showValueText.text = value.ToString();
        }
    }
}