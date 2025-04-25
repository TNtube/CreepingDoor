using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(cylinder_collider.CylinderCollider))]
    public class CylinderColliderTrigger : MonoBehaviour
    {
        private class ColliderData : System.IEquatable<ColliderData>, System.IEquatable<Collider>
        {
            const float defaultLifetime = 0.2f;
            private float lifetime = defaultLifetime;
            private int triggerCount = 1;
            private Collider collider;

            private ColliderData() { }
            public ColliderData(Collider col) => collider = col;
            public Collider Collider => collider;

            public void Decrement() => triggerCount = Mathf.Max(0, triggerCount - 1);
            public void Increment()
            {
                lifetime = defaultLifetime;
                ++triggerCount;
            }

            public bool IsAlive() => lifetime > 0f;
            public void Update(float deltaTime)
            {
                if (triggerCount > 0) return;
                lifetime -= deltaTime;
            }

            public override bool Equals(object other)
            {
                if (other is Collider collider) return Equals(collider);
                if (other is ColliderData colliderData) return Equals(colliderData);
                return false;
            }

            public bool Equals(Collider other) => collider == other;
            public bool Equals(ColliderData other) => Equals(collider, other?.collider);
            public override int GetHashCode() => collider == null ? 0 : collider.GetHashCode();
        }





        [SerializeField]
        private LayerMask mask = -1;
        [SerializeField]
        private UnityEvent<Collider, Collider> onTriggerEnter = new UnityEvent<Collider, Collider>();
        [SerializeField]
        private UnityEvent<Collider, Collider> onTriggerExit = new UnityEvent<Collider, Collider>();

        private List<ColliderData> colliderDatas = new List<ColliderData>();

        void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = colliderDatas.Count - 1; i >= 0; --i) {
                ColliderData colliderData = colliderDatas[i];
                colliderData.Update(deltaTime);
                if (!colliderData.IsAlive()) {
                    onTriggerExit.Invoke(null, colliderData.Collider);
                    colliderDatas.RemoveAt(i);
                }
            }
        }

        void OnDisable()
        {
            for (int i = colliderDatas.Count - 1; i >= 0; --i) {
                ColliderData colliderData = colliderDatas[i];
                onTriggerExit.Invoke(null, colliderData.Collider);
                colliderDatas.RemoveAt(i);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Verify Mask
            if (mask != (mask | (1 << other.gameObject.layer))) return;

            // Process Collider Data
            ColliderData colliderData = GetColliderData(other);
            if (colliderData == null) {
                colliderDatas.Add(new ColliderData(other));
                onTriggerEnter.Invoke(null, other);
            } else colliderData.Increment();
        }

        void OnTriggerExit(Collider other)
        {
            // Verify Mask
            if (mask != (mask | (1 << other.gameObject.layer))) return;

            // Process Collider Data
            ColliderData colliderData = GetColliderData(other);
            if (colliderData != null) colliderData.Decrement();
        }

        private ColliderData GetColliderData(Collider col)
        {
            for (int i = 0; i < colliderDatas.Count; ++i) {
                if (colliderDatas[i].Collider == col) {
                    return colliderDatas[i];
                }
            }
            return null;
        }
    }
}
