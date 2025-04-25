using System.Collections;
using System.Linq;
using UnityEngine;

namespace Project.Utils
{
    public class WaitAny : WaitBase
    {
        public override bool keepWaiting => _wait.All(t => t);
        public WaitAny(MonoBehaviour monoBehaviour, params IEnumerator[] coroutines) : base(monoBehaviour, coroutines) { }
    }
}