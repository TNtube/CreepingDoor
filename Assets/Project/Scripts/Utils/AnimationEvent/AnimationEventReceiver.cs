using System.Collections.Generic;
using UnityEngine;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Project/Utils/Animation Event Receiver")]
    public class AnimationEventReceiver : MonoBehaviour
    {
        private struct EventData
        {
            public IAnimationEvent animationEvent;
            public bool autoRemove;
        }

        private readonly Dictionary<int, EventData> animationEvents = new Dictionary<int, EventData>();

        public int RegisterEvent(IAnimationEvent animationEvent, bool autoRemove = true)
        {
            int key = (System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString()).GetHashCode();
            animationEvents.Add(key, new EventData { animationEvent = animationEvent, autoRemove = autoRemove });
            return key;
        }

        public void RemoveEvent(int key)
        {
            animationEvents.Remove(key);
        }

        public void OnEventCallback(int key)
        {
            if (animationEvents.TryGetValue(key, out EventData eventData)) {
                if (eventData.animationEvent != null) eventData.animationEvent.OnAnimationEvent(key);
                if (eventData.autoRemove) animationEvents.Remove(key);
            }
        }
    }
}