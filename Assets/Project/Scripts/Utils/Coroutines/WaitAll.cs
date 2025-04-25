using System.Collections;
using System.Linq;
using UnityEngine;

namespace Project.Utils
{
    public class WaitAll : WaitBase
    {
        public override bool keepWaiting => _wait.Any(t => t);
        public WaitAll(MonoBehaviour monoBehaviour, params IEnumerator[] coroutines) : base(monoBehaviour, coroutines) { }
    }
}