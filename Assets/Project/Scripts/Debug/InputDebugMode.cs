using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Project.Debug
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Debug/Input Debug Mode")]
    public class InputDebugMode : MonoBehaviour
    {
        [System.Serializable]
        private struct ModeContainer
        {
            public static ModeContainer Default => new ModeContainer { objects = new GameObject[0] };
            [SerializeField]
            private string name;
            public GameObject[] objects;
        }

        [SerializeField]
        private InputActionProperty toggleInput = new InputActionProperty(new InputAction(name: "Toggle", type: InputActionType.Button, binding: "<Keyboard>/f2"));
        [SerializeField]
        private ModeContainer[] modes = new ModeContainer[0];

        private int currentMode = 0;
        private bool IsModeValid => currentMode >= 0 && currentMode < modes.Length;



        void OnEnable()
        {
            toggleInput.EnableDirectAction();
            toggleInput.action.performed += ToggleMode;
            for (int i = 0; i < modes.Length; ++i) SetModeState(modes[i], currentMode == i);
        }

        void OnDisable()
        {
            toggleInput.action.performed -= ToggleMode;
            toggleInput.DisableDirectAction();
        }



        private void ToggleMode(InputAction.CallbackContext obj)
        {
            if (IsModeValid) SetModeState(modes[currentMode], false);
            currentMode = (currentMode + 1) % Mathf.Max(modes.Length, 1);
            if (IsModeValid) SetModeState(modes[currentMode], true);
        }

        private void SetModeState(ModeContainer mode, bool state)
        {
            if (mode.objects == null) return;
            for (int i = 0; i < mode.objects.Length; ++i) {
                mode.objects[i].SetActive(state);
            }
        }
    }
}