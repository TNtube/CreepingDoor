using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Utils
{
    [AddComponentMenu("Project/Utils/Scene Loader")]
    public class SceneLoader : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string sceneName = string.Empty;
        [SerializeField]
        private bool isAdditive = false;
        [SerializeField]
        private bool autoLoad = false;

        void Start()
        {
            if (autoLoad) {
                Load();
            }
        }


        public void Load() => LoadCallback();
        public AsyncOperation LoadCallback()
        {
#if UNITY_EDITOR
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid()) return null;
#endif
            LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }


        public void UnLoad() => UnLoadCallback();
        public AsyncOperation UnLoadCallback()
        {
            return SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}