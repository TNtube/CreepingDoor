using UnityEngine;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Utils/Sync With Target")]
    public class SyncWithTarget : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;
        [SerializeField]
        private string targetName = string.Empty;
        [SerializeField]
        private string targetTag = string.Empty;

        public void Sync()
        {
            GameObject target = GetTarget();
            if (target == null) return;
            target.transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            transform.SetPositionAndRotation(position, rotation);
        }

        private GameObject GetTarget()
        {
            if (target == null && !string.IsNullOrEmpty(targetName)) target = GameObject.Find(targetName);
            if (target == null && !string.IsNullOrEmpty(targetTag)) target = GameObject.FindWithTag(targetTag);
            return target;
        }
    }
}