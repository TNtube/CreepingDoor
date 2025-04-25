using UnityEngine;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Utils/Game Exit")]
    public class GameExit : MonoBehaviour
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void QuitInternal()
        {
            Quit();
        }
    }
}