using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Project.Debug
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Debug/Camera Fly")]
    public class CameraFly : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Transform movementRoot;
        [SerializeField]
        private Transform rotationRoot;
        [SerializeField]
        private float moveSpeed = 1f;
        [SerializeField]
        private float rotationSpeed = 0.25f;
        [SerializeField]
        private float scrollSensivity = 0.01f;

        [Space(40f)]
        [SerializeField]
        private InputActionProperty forwardInput = new InputActionProperty(new InputAction(name: "Forward", type: InputActionType.PassThrough, binding: "<Keyboard>/w"));
        [SerializeField]
        private InputActionProperty backwardInput = new InputActionProperty(new InputAction(name: "Backward", type: InputActionType.PassThrough, binding: "<Keyboard>/s"));
        [SerializeField]
        private InputActionProperty leftInput = new InputActionProperty(new InputAction(name: "Left", type: InputActionType.PassThrough, binding: "<Keyboard>/a"));
        [SerializeField]
        private InputActionProperty rightInput = new InputActionProperty(new InputAction(name: "Right", type: InputActionType.PassThrough, binding: "<Keyboard>/d"));

        [Space(40f)]
        [SerializeField]
        private InputActionProperty upInput = new InputActionProperty(new InputAction(name: "Up", type: InputActionType.PassThrough, binding: "<Keyboard>/e"));
        [SerializeField]
        private InputActionProperty downInput = new InputActionProperty(new InputAction(name: "Down", type: InputActionType.PassThrough, binding: "<Keyboard>/q"));

        [Space(40f)]
        [SerializeField]
        private InputActionProperty mouseInput = new InputActionProperty(new InputAction(name: "Mouse", type: InputActionType.PassThrough, binding: "Mouse/Delta"));
        [SerializeField]
        private InputActionProperty scrollInput = new InputActionProperty(new InputAction(name: "Scroll", type: InputActionType.PassThrough, binding: "Mouse/Scroll/y"));

        private float currentSpeed = 1f;





        private void EnableActions()
        {
            forwardInput.EnableDirectAction();
            backwardInput.EnableDirectAction();
            leftInput.EnableDirectAction();
            rightInput.EnableDirectAction();

            upInput.EnableDirectAction();
            downInput.EnableDirectAction();

            mouseInput.EnableDirectAction();
            scrollInput.EnableDirectAction();
        }

        private void DisableActions()
        {
            forwardInput.DisableDirectAction();
            backwardInput.DisableDirectAction();
            leftInput.DisableDirectAction();
            rightInput.DisableDirectAction();

            upInput.DisableDirectAction();
            downInput.DisableDirectAction();

            mouseInput.DisableDirectAction();
            scrollInput.DisableDirectAction();
        }





        void OnEnable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            EnableActions();
        }

        void OnDisable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            DisableActions();
        }

        void Update()
        {
            Vector2 mouseDelta = GetMouseDelta() * rotationSpeed;
            Vector3 rotation = rotationRoot.rotation.eulerAngles;

            float min = -1f, max = 89f;
            if (rotation.x > 180f) {
                min = 271f;
                max = 361f;
            }

            rotation += new Vector3(-mouseDelta.y, mouseDelta.x, 0f);
            rotation.x = Mathf.Clamp(rotation.x, min, max);
            rotationRoot.rotation = Quaternion.Euler(rotation);

            float scroll = GetScroll() * scrollSensivity;
            currentSpeed = Mathf.Max(currentSpeed + scroll, 0.01f);

            Vector3 moveInput = GetMoveInput() * currentSpeed * moveSpeed * Time.deltaTime;
            movementRoot.position += movementRoot.rotation * moveInput;
        }





        private Vector3 GetMoveInput()
        {
            try {
                float forward = forwardInput.action.ReadValue<float>();
                float backward = backwardInput.action.ReadValue<float>();

                float left = leftInput.action.ReadValue<float>();
                float right = rightInput.action.ReadValue<float>();

                float up = upInput.action.ReadValue<float>();
                float down = downInput.action.ReadValue<float>();

                return new Vector3(right - left, up - down, forward - backward);
            } catch { return Vector3.zero; }
        }

        private Vector2 GetMouseDelta()
        {
            try { return mouseInput.action.ReadValue<Vector2>(); }
            catch { return Vector2.zero; }
        }

        private float GetScroll()
        {
            try { return scrollInput.action.ReadValue<float>(); }
            catch { return 0f; }
        }
    }
}