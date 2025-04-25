using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public abstract class SyncBase : MonoBehaviour
{
    protected enum SyncData
    {
        None,
        X,
        Y,
        Z,
        XY,
        XZ,
        YZ,
        XYZ
    }

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField, Tooltip("Do Execute in Edit Mode")]
    protected bool executeInEditMode = false;
#endif

    [Header("Settings")]
    [SerializeField, Tooltip("The Target to Sync from")]
    protected Transform target;

    public void SetTarget(Transform target) => this.target = target;
    public Transform Target => target;

    protected Vector3 GetData(SyncData syncData, Vector3 original, Vector3 sync)
    {
        Vector3 output = original;
        switch (syncData) {
            case SyncData.X: {
                output.x = sync.x;
                break;
            }
            case SyncData.Y: {
                output.y = sync.y;
                break;
            }
            case SyncData.Z: {
                output.z = sync.z;
                break;
            }
            case SyncData.XY: {
                output.x = sync.x;
                output.y = sync.y;
                break;
            }
            case SyncData.XZ: {
                output.x = sync.x;
                output.z = sync.z;
                break;
            }
            case SyncData.YZ: {
                output.y = sync.y;
                output.z = sync.z;
                break;
            }
            case SyncData.XYZ: {
                output = sync;
                break;
            }
        }
        return output;
    }
}
