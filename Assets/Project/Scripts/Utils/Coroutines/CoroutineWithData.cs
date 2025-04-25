using System.Collections;
using UnityEngine;

namespace Project.Utils
{
    public class CoroutineWithData : CustomYieldInstruction, System.IDisposable
    {
        private bool running = false;
        private readonly MonoBehaviour owner;
        private readonly IEnumerable targetHolder;
        private IEnumerator currentTarget;
        private Coroutine coroutine;
        private object result;
        



        public CoroutineWithData(MonoBehaviour owner, IEnumerable target)
        {
            this.owner = owner;
            targetHolder = target;
        }

        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.owner = owner;
            targetHolder = null;
            currentTarget = target;
        }




        ~CoroutineWithData() => Dispose();
        public void Dispose()
        {
            result = null;
            if (coroutine != null && owner != null) {
                owner.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public override void Reset()
        {
            Dispose();
            if (targetHolder != null) running = false;
            else throw new System.NotSupportedException("CoroutineWithData constructed using IEnumerator cannot be reseted. Please use IEnumerable");
        }




        public override bool keepWaiting {
            get {
                if (!running) Run();
                return coroutine != null;
            }
        }

        public bool TryGet<t>(out t result)
        {
            try {
                result = (t)this.result;
                return true;
            } catch {
                result = default;
                return false;
            }
        }




        private void Run()
        {
            running = true;
            if (targetHolder != null) currentTarget = targetHolder.GetEnumerator();
            coroutine = owner.StartCoroutine(RunCoroutine());
        }

        private IEnumerator RunCoroutine()
        {
            while (currentTarget.MoveNext()) {
                result = currentTarget.Current;
                yield return result;
            }
            coroutine = null;
        }
    }




    public static class CoroutineWithDataHelper
    {
        public static CoroutineWithData StartCoroutineData(this MonoBehaviour owner, IEnumerable target)
        {
            return new CoroutineWithData(owner, target);
        }

        public static CoroutineWithData StartCoroutineData(this MonoBehaviour owner, IEnumerator target)
        {
            return new CoroutineWithData(owner, target);
        }

        public static bool ResetSafe(this CoroutineWithData coroutineWithData)
        {
            try {
                coroutineWithData.Reset();
                return true;
            } catch { return false; }
        }
    }
}