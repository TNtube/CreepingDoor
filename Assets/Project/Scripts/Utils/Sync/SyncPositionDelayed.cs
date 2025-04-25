using UnityEngine;

namespace Project.Utils
{
    [ExecuteAlways]
    [AddComponentMenu("Project/Utils/Sync Position Delayed")]
    public class SyncPositionDelayed : MonoBehaviour
    {
    #if UNITY_EDITOR
        [SerializeField]
        private bool disableInEditor = true;
#endif
        [Space]
        [SerializeField]
        private GameObject target;
        [SerializeField]
        private string targetName = string.Empty;
        [SerializeField]
        private string targetTag = string.Empty;

        [Space]
        [SerializeField, Min(0f)]
        private float syncX = 0f;
        [SerializeField, Min(0f)]
        private float syncY = 0f;
        [SerializeField, Min(0f)]
        private float syncZ = 0f;

        private Transform Source {
            get {
                if (target == null && !string.IsNullOrEmpty(targetName)) target = GameObject.Find(targetName);
                if (target == null && !string.IsNullOrEmpty(targetTag)) target = GameObject.FindWithTag(targetTag);
                return target != null ? target.transform : null;
            }
        }

        void LateUpdate()
        {
    #if UNITY_EDITOR
            if (disableInEditor && !UnityEditor.EditorApplication.isPlaying) return;
    #endif
            if (Source == null) return;
            float deltaTime = Time.deltaTime;
            Vector3 pos = transform.position, position = Source.position;
            pos.x = Mathf.MoveTowards(pos.x, position.x, syncX * deltaTime);
            pos.y = Mathf.MoveTowards(pos.y, position.y, syncY * deltaTime);
            pos.z = Mathf.MoveTowards(pos.z, position.z, syncZ * deltaTime);
            transform.position = pos;
        }
    }
}