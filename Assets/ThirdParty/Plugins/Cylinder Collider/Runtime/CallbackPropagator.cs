using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cylinder_collider
{
    internal class CallbackPropagator : MonoBehaviour
    {
        /*
         * This class is for removing overlapping callbacks
         * Without this class, callbacks like OnTriggerEnter will be called for every child collider.
         * This means that too many callbacks may be called in one frame.
         */
        public static CallbackPropagator Instance
        {
            get
            {
                if(_Instance == null) Initialize();
                return _Instance;
            }
        }
        static CallbackPropagator _Instance;
        public List<CylinderCollider> cylinderColliders = new List<CylinderCollider>();
        public List<TorusCollider> torusColliders = new List<TorusCollider>();
        const string OnTriggerEnter = "OnTriggerEnter";
        const string OnTriggerStay = "OnTriggerStay";
        const string OnTriggerExit = "OnTriggerExit";
        const string OnCollisionEnter = "OnCollisionEnter";
        const string OnCollisionStay = "OnCollisionStay";
        const string OnCollisionExit = "OnCollisionExit";
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if(_Instance != null) return;
            var propagator = new GameObject("Callback Propagator").AddComponent<CallbackPropagator>();
            propagator.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(propagator);
        }
        void Awake()
        {
            StartCoroutine(Fixed());
            _Instance = this;
        }

        void OnDestroy()
        {
            if (_Instance == this) _Instance = null;
        }

        public static void Register(CylinderCollider cc)
        {
            Instance.cylinderColliders.Add(cc);
        }

        public static void Register(TorusCollider tc)
        {
            Instance.torusColliders.Add(tc);
        }

        public static void Unregister(CylinderCollider cc)
        {
            if(_Instance == null) return;
            _Instance.cylinderColliders.Remove(cc);
        }

        public static void Unregister(TorusCollider tc)
        {
            if(_Instance == null) return;
            _Instance.torusColliders.Remove(tc);
        }

        IEnumerator Fixed()
        {
            /*
             * Used coroutine to invoke callbacks at the same frame when it triggered.
             * FixedUpdate invokes callbacks 1 frame late.
             */
            var wait = new WaitForFixedUpdate();
            while (true)
            {
                yield return wait;
                for (int i = 0; i < cylinderColliders.Count; i++)
                {
                    var cc = cylinderColliders[i];
                    InvokePhysicalCallbacks( cc,
                        cc.triggerEnterRequests, cc.triggerStayRequests, cc.triggerExitRequests,
                        cc.collisionEnterRequests, cc.collisionStayRequests, cc.collisionExitRequests);
                }
            
                for (int i = 0; i < torusColliders.Count; i++)
                {
                    var tc = torusColliders[i];
                    InvokePhysicalCallbacks( tc,
                        tc.triggerEnterRequests, tc.triggerStayRequests, tc.triggerExitRequests,
                        tc.collisionEnterRequests, tc.collisionStayRequests, tc.collisionExitRequests);
                }
            }
        }
        
        void InvokePhysicalCallbacks(MonoBehaviour target,
            HashSet<Collider> triggerEnter, HashSet<Collider> triggerStay, HashSet<Collider> triggerExit,
            HashSet<Collision> collisionEnter, HashSet<Collision> collisionStay, HashSet<Collision> collisionExit)
        {
            foreach (var request in triggerEnter)
                target.SendMessageUpwards(OnTriggerEnter, request, SendMessageOptions.DontRequireReceiver);
            
            foreach (var request in triggerStay)
                target.SendMessageUpwards(OnTriggerStay, request, SendMessageOptions.DontRequireReceiver);
            
            foreach (var request in triggerExit)
                target.SendMessageUpwards(OnTriggerExit, request, SendMessageOptions.DontRequireReceiver);
            
            foreach (var request in collisionEnter)
                target.SendMessageUpwards(OnCollisionEnter, request, SendMessageOptions.DontRequireReceiver);
            
            foreach (var request in collisionStay)
                target.SendMessageUpwards(OnCollisionStay, request, SendMessageOptions.DontRequireReceiver);
            
            foreach (var request in collisionExit)
                target.SendMessageUpwards(OnCollisionExit, request, SendMessageOptions.DontRequireReceiver);
            
            triggerEnter.Clear();
            triggerStay.Clear();
            triggerExit.Clear();
            collisionEnter.Clear();
            collisionStay.Clear();
            collisionExit.Clear();
        }
    }
}