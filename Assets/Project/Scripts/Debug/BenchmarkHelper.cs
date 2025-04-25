using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR;
using Unity.XR.MockHMD;
using CodeStage.AdvancedFPSCounter;

namespace Project.Debug
{
    public class BenchmarkHelper : MonoBehaviour
    {
        [SerializeField]
        private bool setXRSettings = true;

        //[SerializeField]
        //private InputActionProperty disable = new InputActionProperty(new InputAction(name: "Disable", type: InputActionType.Button, binding: "<Keyboard>/f6"));
        [SerializeField]
        private InputActionProperty background = new InputActionProperty(new InputAction(name: "Background", type: InputActionType.Button, binding: "<Keyboard>/f5"));
        [SerializeField]
        private InputActionProperty normal = new InputActionProperty(new InputAction(name: "Normal", type: InputActionType.Button, binding: "<Keyboard>/f4"));

        [SerializeField]
        private InputActionProperty screen = new InputActionProperty(new InputAction(name: "Screen", type: InputActionType.Button, binding: "<Keyboard>/f2"));

        [SerializeField]
        private AFPSCounter fpsCounter;

        [Space]
        [SerializeField]
        private InputActionProperty callback = new InputActionProperty(new InputAction(name: "Screen", type: InputActionType.Button, binding: "<Keyboard>/space"));
        [SerializeField]
        private UnityEngine.Events.UnityEvent onCallback;

        void OnEnable()
        {
            if (setXRSettings) {
                XRSettings.gameViewRenderMode = GameViewRenderMode.OcclusionMesh;
                MockHMD.SetRenderMode(MockHMDBuildSettings.RenderMode.SinglePassInstanced);
                MockHMD.SetEyeResolution(3072, 3216); // 1512x1680 => 3072x3216 per eye
                MockHMD.SetFoveationMode(false, (uint)MockHMDBuildSettings.FoveationGazeSimulationMode.Disabled);
            }

            //disable.EnableDirectAction();
            //disable.action.performed += OnDisab;
            background.EnableDirectAction();
            background.action.performed += OnBackground;
            normal.EnableDirectAction();
            normal.action.performed += OnNormal;
            screen.EnableDirectAction();
            screen.action.performed += OnScreen;
            callback.EnableDirectAction();
            callback.action.performed += OnCallback;
        }

        void OnDisable()
        {
            //disable.action.performed -= OnDisab;
            //disable.DisableDirectAction();
            background.action.performed -= OnBackground;
            background.DisableDirectAction();
            normal.action.performed -= OnNormal;
            normal.DisableDirectAction();
            screen.action.performed -= OnScreen;
            screen.DisableDirectAction();
            callback.action.performed -= OnCallback;
            callback.DisableDirectAction();
            
        }

        /*private void OnDisab(InputAction.CallbackContext obj)
        {
            if (fpsCounter.OperationMode == OperationMode.Disabled) return;
            fpsCounter.OperationMode = OperationMode.Disabled;
        }*/

        private void OnBackground(InputAction.CallbackContext obj)
        {
            if (fpsCounter.OperationMode == OperationMode.Background) return;
            fpsCounter.OperationMode = OperationMode.Background;
        }

        private void OnNormal(InputAction.CallbackContext obj)
        {
            if (fpsCounter.OperationMode == OperationMode.Normal) return;
            fpsCounter.OperationMode = OperationMode.Normal;
        }

        private void OnScreen(InputAction.CallbackContext obj)
        {
            string dirPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Benchmark"));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            int id = 0;
            string filePath = Path.Combine(dirPath, "Screen_{0}.png");
            while (File.Exists(string.Format(filePath, id.ToString()))) ++id;

#if UNITY_EDITOR
            string basePath = "Benchmark";
#else
            string basePath = Path.Combine("..", "Benchmark");
#endif

            string outputPath = string.Format(Path.Combine(basePath, "Screen_{0}.png"), id.ToString());
            ScreenCapture.CaptureScreenshot(outputPath, ScreenCapture.StereoScreenCaptureMode.BothEyes);
        }

        private void OnCallback(InputAction.CallbackContext obj)
        {
            onCallback.Invoke();
        }
    }
}
