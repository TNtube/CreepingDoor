using UnityEngine;
using UnityEngine.Events;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Utils/On State Changed")]
    public class OnStateChanged : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onEnable = new UnityEvent();
        [SerializeField]
        private UnityEvent onDisable = new UnityEvent();

        void OnEnable() => onEnable.Invoke();
        void OnDisable() => onDisable.Invoke();
    }
}