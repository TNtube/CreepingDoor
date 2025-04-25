using UnityEngine;

namespace Project.Utils
{
    [ExecuteAlways]
    [AddComponentMenu("Project/Utils/Sync Rotation")]
    public class SyncRotation : MonoBehaviour
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
            Vector3 rotation = source.rotation.eulerAngles;
            Vector3 rot = transform.rotation.eulerAngles;
            if (syncX) rot.x = rotation.x;
            if (syncY) rot.y = rotation.y;
            if (syncZ) rot.z = rotation.z;
            transform.rotation = Quaternion.Euler(rot);
        }
    }
}