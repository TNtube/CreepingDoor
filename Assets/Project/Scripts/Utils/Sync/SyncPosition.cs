using UnityEngine;

namespace Project.Utils
{
    [ExecuteAlways]
    [AddComponentMenu("Project/Utils/Sync Position")]
    public class SyncPosition : MonoBehaviour
    {
    #if UNITY_EDITOR
        [SerializeField]
        private bool disableInEditor = true;
    #endif

        [Space]
        [SerializeField]
        private Transform source;
        [SerializeField]
        private bool syncX = false;
        [SerializeField]
        private bool syncY = false;
        [SerializeField]
        private bool syncZ = false;

        void LateUpdate()
        {
    #if UNITY_EDITOR
            if (disableInEditor && !UnityEditor.EditorApplication.isPlaying) return;
    #endif
            if (source == null) return;
            Vector3 position = source.position;
            Vector3 pos = transform.position;
            if (syncX) pos.x = position.x;
            if (syncY) pos.y = position.y;
            if (syncZ) pos.z = position.z;
            transform.position = pos;
        }
    }
}