using UnityEngine;

namespace Project.Utils
{
    [ExecuteAlways]
    [AddComponentMenu("Project/Utils/Sync Rotation Delayed")]
    public class SyncRotationDelayed : MonoBehaviour
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
            Vector3 rot = transform.rotation.eulerAngles, rotation = Source.rotation.eulerAngles;
            if (syncX > 0f) rot.x = GetMovement(rot.x, rotation.x, deltaTime * syncX);
            if (syncY > 0f) rot.y = GetMovement(rot.y, rotation.y, deltaTime * syncY);
            if (syncZ > 0f) rot.z = GetMovement(rot.z, rotation.z, deltaTime * syncZ);
            transform.rotation = Quaternion.Euler(rot);
        }

        private float GetMovement(float source, float target, float speed)
        {
            float minZ = 360f - target, maxZ = 360f + target;
            float minDistance = Mathf.Abs(minZ - source), distance = Mathf.Abs(target - source), maxDistance = Mathf.Abs(maxZ - source);
            if (minDistance < distance && minDistance < maxDistance) target = minZ;
            else if (maxDistance < distance && maxDistance < distance) target = maxZ;
            return Mathf.MoveTowards(source, target, speed);
        }
    }
}