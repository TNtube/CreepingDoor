using System.Collections.Generic;
using UnityEngine;

namespace Project.Utils
{
    [AddComponentMenu("Project/Utils/Controller Haptic")]
    public class ControllerHaptic : MonoBehaviour
    {
        private static readonly Dictionary<ControllerHapticID, ControllerHapticLink> controllerHaptics = new Dictionary<ControllerHapticID, ControllerHapticLink>();

        public static void Register(ControllerHapticLink hapticLink)
        {
            if (hapticLink.Id == ControllerHapticID.None) return;
            if (controllerHaptics.ContainsKey(hapticLink.Id)) return;
            controllerHaptics.Add(hapticLink.Id, hapticLink);
        }

        public static void Remove(ControllerHapticLink hapticLink)
        {
            if (controllerHaptics.TryGetValue(hapticLink.Id, out ControllerHapticLink link)) {
                if (link == hapticLink) controllerHaptics.Remove(hapticLink.Id);
            }
        }

        public static void Trigger(ControllerHapticID id, float intensity, float duration)
        {
            if (!controllerHaptics.TryGetValue(id, out ControllerHapticLink link)) return;
            link.Trigger(intensity, duration);
        }



        [Header("Default Settings")]
        [SerializeField]
        private ControllerHapticID id = ControllerHapticID.None;
        [SerializeField]
        private float intensity = 0.5f;
        [SerializeField]
        private float duration = 0.5f;

        public void Trigger()
        {
            Trigger(id, intensity, duration);
        }
    }
}