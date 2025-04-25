using UnityEngine;

namespace Project.Utils
{
    [AddComponentMenu("Project/Utils/UI Settings")]
    public class UISettings : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] objects = new GameObject[0];

        private bool isEnabled = false;

        public void Toggle()
        {
            isEnabled = !isEnabled;
            foreach (GameObject go in objects) {
                go.SetActive(isEnabled);
            }
        }
    }
}