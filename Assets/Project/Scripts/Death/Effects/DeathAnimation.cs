using System.Collections;
using UnityEngine;
using Project.Utils;

[DisallowMultipleComponent]
[AddComponentMenu("Project/Death/Death Animation")]
public class DeathAnimation : MonoBehaviour, IDeathCallback, IAnimationEvent
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip clip;

    private AnimationEventData eventData;

    public IEnumerator OnDeathCallback()
    {
        if (eventData == null) {
            if (animator == null || clip == null) yield break;
            eventData = animator.RegisterEvent(this, clip, clip.GetEnd());
            animator.CrossFadeNicely(clip.name, 0.1f);
        }
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
        while (eventData != null) yield return waitForSeconds;
    }

    public void OnAnimationEvent(int key)
    {
        if (eventData == key) {
            eventData = null;
        }
    }
}