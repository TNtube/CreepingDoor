using UnityEngine;

public class TransformSyncSmoothAdvanced : SyncBase
{
    [Header("Position Settings")]
    [SerializeField, Tooltip("Sync Position Data")]
    private SyncData syncPositionData = SyncData.None;
    [SerializeField, Tooltip("Position Sync Speed"), Min(0f)]
    private float syncPositionSpeed = 0f;
    [SerializeField, Tooltip("Sync Min Distance"), Min(0f)]
    private Vector2 syncMinPosition = Vector2.zero;

    [Header("Rotation Settings")]
    [SerializeField, Tooltip("Sync Position Data")]
    private SyncData syncRotationData = SyncData.None;
    [SerializeField, Tooltip("Rotation Sync Speed"), Min(0f)]
    private float syncRotationSpeed = 0f;
    [SerializeField, Tooltip("Sync Min Rotation (in degrees)"), Min(0f)]
    private Vector2 syncMinRotation = Vector2.zero;

    [Header("Scale Settings")]
    [SerializeField, Tooltip("Sync Position Data")]
    private SyncData syncScaleData = SyncData.None;
    [SerializeField, Tooltip("Scale Sync Speed"), Min(0f)]
    private float syncScaleSpeed = 0f;
    [SerializeField, Tooltip("Sync Min Scale"), Min(0f)]
    private Vector2 syncMinScale = Vector2.zero;

    private bool syncPositionDistance = false;
    private bool syncRotationDistance = false;
    private bool syncScaleDistance = false;

    void OnDisable()
    {
        syncPositionDistance = false;
        syncRotationDistance = false;
        syncScaleDistance = false;
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && !executeInEditMode) return;
#endif
        // Valid + Prepare Variables
        if (target == null) return;
        float delta = Time.deltaTime;

        // Sync Position
        if (syncPositionData != SyncData.None && syncPositionSpeed > 0f) {
            Vector3 position = GetData(syncPositionData, transform.position, target.position);
            float distance = Vector3.Distance(transform.position, position);
            if (syncPositionDistance) {
                transform.position = Vector3.MoveTowards(transform.position, position, delta * syncPositionSpeed);
                syncPositionDistance = distance > syncMinPosition.y;
            } else syncPositionDistance = distance > syncMinPosition.x;
        }

        // Sync Rotation
        if (syncRotationData != SyncData.None && syncRotationSpeed > 0f) {
            Quaternion rotation = Quaternion.Euler(GetData(syncRotationData, transform.rotation.eulerAngles, target.rotation.eulerAngles));
            float angle = Quaternion.Angle(transform.rotation, rotation);
            if (syncRotationDistance) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, delta * syncRotationSpeed);
                syncRotationDistance = angle > syncMinRotation.y;
            } else syncRotationDistance = angle > syncMinRotation.x;
        }

        // Sync Scale
        if (syncScaleData != SyncData.None && syncScaleSpeed > 0f) {
            Vector3 scale = GetData(syncScaleData, transform.localScale, transform.worldToLocalMatrix * target.lossyScale);
            float distance = Vector3.Distance(transform.localScale, scale);
            if (syncScaleDistance) {
                transform.localScale = Vector3.MoveTowards(transform.localScale, scale, delta * syncScaleSpeed);
                syncScaleDistance = distance > syncMinScale.y;
            } else syncScaleDistance = distance > syncMinScale.x;
        }
    }
}