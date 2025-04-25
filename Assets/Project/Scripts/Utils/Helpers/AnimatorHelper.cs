using UnityEngine;

namespace Project.Utils
{
    public static class AnimatorHelper
    {
        public static void CrossFadeNicely(this Animator animator, string stateName, float transitionDuration) => CrossFadeNicely(animator, stateName, transitionDuration, 0);
        public static void CrossFadeNicely(this Animator animator, string stateName, float transitionDuration, int layerIndex)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (stateInfo.IsName(stateName)) animator.Play(stateName, layerIndex, 0f);
            else animator.CrossFade(stateName, transitionDuration, layerIndex);
        }
    }
}