using UnityEngine;
using UnityEngine.Events;
using Project.Events;

namespace Project.Debug
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Debug/Debug Menu")]
    public class DebugMenu : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent OnOn = new UnityEvent();
        [SerializeField]
        private UnityEvent OnOff = new UnityEvent();
        [SerializeField]
        private BoolEvent OnToggle = new BoolEvent();

        private bool _selected = false;
        public void Select()
        {
            _selected = !_selected;
            if (_selected) OnOn?.Invoke();
            else OnOff?.Invoke();
            OnToggle?.Invoke(_selected);
        }
    }
}