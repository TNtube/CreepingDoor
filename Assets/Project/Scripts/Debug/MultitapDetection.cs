using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[AddComponentMenu("Project/Debug/Multitap Detection")]
public class MultitapDetection : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int tapCountRequired = 3;
    [SerializeField, Min(0.1f)]
    private float tapDelay = 2f;
    
    [SerializeField, Space]
    private InputActionProperty toggleInput = new InputActionProperty(new InputAction(name: "Toggle", type: InputActionType.Button, binding: "<Keyboard>/f2"));
    [SerializeField, Space]
    private UnityEvent OnMultitapPerformed = new UnityEvent();

    private int tapCount = 0;
    private float currentDelay = 0f;

    void OnEnable()
    {
        toggleInput.EnableDirectAction();
        toggleInput.action.performed += Trigger;
    }

    void OnDisable()
    {
        toggleInput.action.performed -= Trigger;
        toggleInput.DisableDirectAction();
    }

    void Update()
    {
        if (tapCount <= 0) return;
        currentDelay -= Time.deltaTime;
        if (currentDelay > 0f) return;
        tapCount = 0;
    }

    public void Trigger(InputAction.CallbackContext ctx)
    {
        if (++tapCount >= tapCountRequired) {
            tapCount = 0;
            currentDelay = 0f;
            OnMultitapPerformed?.Invoke();
        } else currentDelay = tapDelay;
    }
}