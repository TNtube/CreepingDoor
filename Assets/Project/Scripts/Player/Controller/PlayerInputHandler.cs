using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[DisallowMultipleComponent]
[AddComponentMenu("Project/Player/Player Input Handler")]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private ActionBasedController leftController;
    [SerializeField]
    private ActionBasedController rightController;
    [SerializeField]
    private bool gripRightOnly = false;

    [Header("Custom Bindings")]
    [SerializeField]
    private InputActionProperty menuAction = new InputActionProperty(new InputAction("Menu", type: InputActionType.Button));

    [Header("Events")]
    [SerializeField]
    private UnityEvent onMenuLeft = new UnityEvent();
    [SerializeField]
    private UnityEvent onTriggerLeft = new UnityEvent();
    [SerializeField]
    private UnityEvent onTriggerRight = new UnityEvent();
    [SerializeField]
    private UnityEvent onGripLeft = new UnityEvent();
    [SerializeField]
    private UnityEvent onGripRight = new UnityEvent();





    void OnEnable()
    {
        EnableActions();
        menuAction.action.performed += OnMenuLeft;
        if (leftController != null) {
            leftController.activateAction.action.performed += OnTriggerLeft;
            leftController.selectAction.action.performed += OnGripLeft;
        }
        if (rightController != null) {
            rightController.activateAction.action.performed += OnTriggerRight;
            rightController.selectAction.action.performed += OnGripRight;
        }
    }

    void OnDisable()
    {
        if (leftController != null) {
            leftController.activateAction.action.performed -= OnTriggerLeft;
            leftController.selectAction.action.performed -= OnGripLeft;
        }
        if (rightController != null) {
            rightController.activateAction.action.performed -= OnTriggerRight;
            rightController.selectAction.action.performed -= OnGripRight;
        }
        menuAction.action.performed -= OnMenuLeft;
        DisableActions();
    }

    private void EnableActions()
    {
        menuAction.EnableDirectAction();
    }

    private void DisableActions()
    {
        menuAction.DisableDirectAction();
    }





    private void OnMenuLeft(InputAction.CallbackContext ctx)
    {
        onMenuLeft.Invoke();
    }

    private void OnTriggerLeft(InputAction.CallbackContext ctx)
    {
        onTriggerLeft.Invoke();
    }

    private void OnTriggerRight(InputAction.CallbackContext ctx)
    {
        onTriggerRight.Invoke();
    }

    private void OnGripLeft(InputAction.CallbackContext ctx)
    {
        if (gripRightOnly) return;
        onGripLeft.Invoke();
    }

    private void OnGripRight(InputAction.CallbackContext ctx)
    {
        if (gripRightOnly) onGripLeft.Invoke();
        onGripRight.Invoke();
    }
}