using UnityEngine;

namespace Project.Utils
{
    public static class AnimationEventHelper
    {
        public static float GetEnd(this AnimationClip animationClip)
        {
            return animationClip.length - 0.01f;
        }

        public static AnimationEventData RegisterEvent(this Animator animator, IAnimationEvent animationEvent, AnimationClip animationClip, float time, bool autoRemove = true)
        {
            if (animationEvent == null || animationClip == null) return null;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            AnimationClip wantedClip = null;
            for (int i = 0; i < clips.Length; ++i) {
                AnimationClip clip = clips[i];
                if (clip.name == animationClip.name && clip.length == animationClip.length) {
                    wantedClip = clip;
                    break;
                }
            }

            if (wantedClip == null) return null;
            if (!animator.TryGetComponent(out AnimationEventReceiver animationEventReceiver)) {
                animationEventReceiver = animator.gameObject.AddComponent<AnimationEventReceiver>();
            }

            int key = animationEventReceiver.RegisterEvent(animationEvent, autoRemove);
            AnimationEvent unityAnimationEvent = new AnimationEvent {
                messageOptions = SendMessageOptions.DontRequireReceiver, 
                functionName = nameof(AnimationEventReceiver.OnEventCallback),
                intParameter = key, time = time
            };

            wantedClip.AddEvent(unityAnimationEvent);
            return new AnimationEventData {
                animationClip = wantedClip,
                animationEvent = unityAnimationEvent,
                key = key
            };
        }

        public static void RemoveEvent(this Animator animator, AnimationEventData animationEventData)
        {
            if (animationEventData == null || animationEventData.key == -1) return;
            if (animationEventData.animationEvent == null || animationEventData.animationClip == null) return;

            if (animator.TryGetComponent(out AnimationEventReceiver animationEventReceiver)) {
                animationEventReceiver.RemoveEvent(animationEventData.key);
            }

            AnimationEvent[] events = animationEventData.animationClip.events;
            ArrayHelper.RemoveFromArray(ref events, animationEventData.animationEvent);
            animationEventData.animationClip.events = events;
        }
    }

    public class AnimationEventData
    {
        public AnimationClip animationClip;
        public AnimationEvent animationEvent;
        public int key = -1;

        public static implicit operator int(AnimationEventData data) => data is null ? -1 : data.key;
        public static bool operator ==(AnimationEventData data1, AnimationEventData data2) => (int)data1 == (int)data2;
        public static bool operator !=(AnimationEventData data1, AnimationEventData data2) => !(data1 == data2);
        public static bool operator ==(AnimationEventData data, int key) => (int)data == key;
        public static bool operator !=(AnimationEventData data, int key) => !(data == key);
        public static bool operator ==(int key, AnimationEventData data) => (int)data == key;
        public static bool operator !=(int key, AnimationEventData data) => !(data == key);

        public override bool Equals(object obj)
        {
            if (obj is AnimationEventData eventData) return this == eventData;
            if (obj is int eventDataKey) return this == eventDataKey;
            return false;
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(animationClip, animationEvent, key).GetHashCode();
        }
    }
}