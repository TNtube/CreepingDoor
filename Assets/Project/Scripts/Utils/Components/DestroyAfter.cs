using UnityEngine;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Utils/Destroy After")]
    public class DestroyAfter : MonoBehaviour
    {
        [SerializeField]
        private float duration = 1f;
        private float currentDuration = 0f;

        void Update()
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= duration) Destroy(gameObject);
        }
    }
}