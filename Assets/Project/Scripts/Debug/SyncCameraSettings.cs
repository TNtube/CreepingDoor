using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.Universal;
#endif

namespace Project.Utils
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Project/Utils/Sync Camera Settings")]
    public class SyncCameraSettings : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private Camera sourceCamera;
        [SerializeField, HideInInspector]
        private Camera targetCamera;

        const int skipCount = 20;
        private int currentSkip = 0;

        SyncCameraSettings() => EditorApplication.update += OnUpdate;
        ~SyncCameraSettings() => EditorApplication.update -= OnUpdate;

        void OnUpdate()
        {
            if (this == null || gameObject == null) {
                EditorApplication.update -= OnUpdate;
                return;
            }

            if (sourceCamera == null) return;
            if (++currentSkip % skipCount != 0) return;
            if (targetCamera == null && !TryGetComponent(out targetCamera)) return;

            UniversalAdditionalCameraData sourceData = sourceCamera.GetUniversalAdditionalCameraData();
            UniversalAdditionalCameraData cameraData = targetCamera.GetUniversalAdditionalCameraData();
            EditorUtility.CopySerialized(sourceData, cameraData);
            EditorUtility.CopySerialized(sourceCamera, targetCamera);
        }
#endif
    }
}