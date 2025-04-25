using UnityEngine;
using UnityEngine.Events;

namespace Project.Utils
{
    public class OnTrigger : MonoBehaviour
    {
        [SerializeField]
        private LayerMask mask = -1;
        [SerializeField]
        private UnityEvent<Collider> onTriggerEnter;
        [SerializeField]
        private UnityEvent<Collider> onTriggerExit;

        void OnTriggerEnter(Collider other)
        {
            int layer = (int)Mathf.Pow(2, other.gameObject.layer);
            if ((mask.value & layer) == 0) return;
            onTriggerEnter.Invoke(other);
        }

        void OnTriggerExit(Collider other)
        {
            int layer = (int)Mathf.Pow(2, other.gameObject.layer);
            if ((mask.value & layer) == 0) return;
            onTriggerExit.Invoke(other);
        }
    }
}