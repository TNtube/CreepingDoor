using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRBaseController))]
    [AddComponentMenu("Project/Utils/Controller Haptic Link")]
    public class ControllerHapticLink : MonoBehaviour
    {
        [SerializeField]
        private ControllerHapticID id = ControllerHapticID.None;
        public ControllerHapticID Id => id;
        private XRBaseController controller;

        void OnDestroy() => ControllerHaptic.Remove(this);
        void Awake()
        {
            controller = GetComponent<XRBaseController>();
            ControllerHaptic.Register(this);
        }

        public void Trigger(float intensity, float duration)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    public enum ControllerHapticID
    {
        None,
        Left,
        Right
    }
}