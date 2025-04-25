using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Project.Utils;

[DisallowMultipleComponent]
[AddComponentMenu("Project/Game/Reload Game")]
public class ReloadGame : MonoBehaviour
{
    private static ReloadGame instance;
    void Awake() => instance = this;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField, Min(0.01f)]
    private float circleThickness = 1f;
    [SerializeField, Min(0.01f)]
    private float lineThickness = 1f;
#endif

    [Header("Reload Settings")]
    [SerializeField, Min(0f)]
    private float reloadFinishDelay = 1f;
    [SerializeField]
    private SceneLoader[] sceneLoaders = new SceneLoader[0];

    [Header("Position Settings")]
    [SerializeField]
    private Transform positionReference;
    [SerializeField, Min(0f)]
    private Vector3 positionDistance = new Vector3(2f, 0.1f, 2f);
    [SerializeField]
    private bool positionYSuperior = false;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onReloadStart = new UnityEvent();
    [SerializeField]
    private UnityEvent onReloadFinish = new UnityEvent();

    private bool waitForPlayerPosition = false;

    public static void Reload()
    {
        if (instance == null) return;
        instance.StartCoroutine(instance.StartReloadCoroutine());
    }

    void Update()
    {
        // Check Settings
        if (!waitForPlayerPosition) return;
        //if (ReferenceHelper.CharacterController == null) return;
        Vector3 playerPos = Vector3.zero; //ReferenceHelper.CharacterControllerPosition;
        Vector3 referencePos = positionReference.position;

        // Check each Position component individually
        bool xGood = Mathf.Abs(playerPos.x - referencePos.x) <= positionDistance.x;
        bool yGood = Mathf.Abs(playerPos.y - referencePos.y) <= positionDistance.y && (!positionYSuperior || playerPos.y >= referencePos.y);
        bool zGood = Mathf.Abs(playerPos.z - referencePos.z) <= positionDistance.z;

        // Check Position is good + Trigger Callbacks
        if (xGood && yGood && zGood) {
            StartCoroutine(FinishReloadCoroutine());
            waitForPlayerPosition = false;
        }
    }

    private IEnumerator StartReloadCoroutine()
    {
        onReloadStart.Invoke();
        WaitForSecondsRealtime waitForSeconds = new WaitForSecondsRealtime(0.1f);
        for (int i = 0; i < sceneLoaders.Length; ++i) {
            AsyncOperation op = sceneLoaders[i].UnLoadCallback();
            while (op != null && !op.isDone) yield return waitForSeconds;
        }

        yield return null;
        waitForPlayerPosition = true;
    }

    private IEnumerator FinishReloadCoroutine()
    {
        WaitForSecondsRealtime waitForSeconds = new WaitForSecondsRealtime(0.1f);
        for (int i = 0; i < sceneLoaders.Length; ++i) {
            AsyncOperation op = sceneLoaders[i].LoadCallback();
            while (op != null && !op.isDone) yield return waitForSeconds;
        }

        yield return null;
        yield return new WaitForSeconds(reloadFinishDelay);
        Resources.UnloadUnusedAssets();
        onReloadFinish.Invoke();
    }



#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (positionReference == null) return;
        Color originalColor = UnityEditor.Handles.color;
        Matrix4x4 originalMatrix = UnityEditor.Handles.matrix;
        Vector3 refPosition = positionReference.position;

        Matrix4x4 circleXMatrix = Matrix4x4.TRS(refPosition, Quaternion.identity, new Vector3(positionDistance.x, 1f, positionDistance.z));
        Matrix4x4 circleYMatrix = Matrix4x4.TRS(refPosition, Quaternion.identity, new Vector3(positionDistance.x, positionDistance.y, 1f));
        Matrix4x4 circleZMatrix = Matrix4x4.TRS(refPosition, Quaternion.identity, new Vector3(1f, positionDistance.y, positionDistance.z));
        Matrix4x4 LineXMatrix = Matrix4x4.TRS(refPosition, Quaternion.identity, new Vector3(positionDistance.x, 1f, 1f));
        Matrix4x4 LineYMatrix = Matrix4x4.TRS(refPosition, Quaternion.identity, new Vector3(1f, positionDistance.y, 1f));
        Matrix4x4 LineZMatrix = Matrix4x4.TRS(refPosition, Quaternion.identity, new Vector3(1f, 1f, positionDistance.z));

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.matrix = circleXMatrix;
        UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f, circleThickness);
        UnityEditor.Handles.matrix = LineXMatrix;
        UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.right, lineThickness);

        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.matrix = circleYMatrix;
        UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1f, circleThickness);
        UnityEditor.Handles.matrix = LineYMatrix;
        UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.up, lineThickness);

        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.matrix = circleZMatrix;
        UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.right, 1f, circleThickness);
        UnityEditor.Handles.matrix = LineZMatrix;
        UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.forward, lineThickness);

        UnityEditor.Handles.matrix = originalMatrix;
        UnityEditor.Handles.color = originalColor;
    }
#endif
}