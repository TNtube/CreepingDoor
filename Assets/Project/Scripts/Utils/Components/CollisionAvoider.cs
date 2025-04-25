using System.Collections.Generic;
using UnityEngine;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Project/Utils/Collision Avoider")]
    public class CollisionAvoider : MonoBehaviour
    {
        private readonly static Dictionary<string, List<Collider>> collisionGroups = new Dictionary<string, List<Collider>>();

        private static void Register(string group, Collider collider)
        {
            if (string.IsNullOrEmpty(group) || collider == null) return;
            if (!collisionGroups.TryGetValue(group, out List<Collider> colliders)) {
                colliders = new List<Collider>();
                collisionGroups.Add(group, colliders);
            }
            SetCollision(colliders, collider, true);
            colliders.Add(collider);
        }

        private static void Unregister(string group, Collider collider)
        {
            if (string.IsNullOrEmpty(group) || collider == null) return;
            if (!collisionGroups.TryGetValue(group, out List<Collider> colliders)) return;
            if (colliders.Remove(collider)) SetCollision(colliders, collider, false);
            if (colliders.Count <= 0) collisionGroups.Remove(group);
        }

        private static void SetCollision(List<Collider> colliders, Collider collider, bool state)
        {
            colliders.RemoveAll((Collider col) => col == null);
            for (int i = 0; i < colliders.Count; ++i) {
                Physics.IgnoreCollision(colliders[i], collider, state);
            }
        }



        [SerializeField, Tooltip("Object should only be in 1 group at same time")]
        private string group = "DefaultGroup";

        private Collider[] colliders = new Collider[0];
        private string registeredGroup = string.Empty;

        void OnEnable()
        {
            registeredGroup = group;
            colliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; ++i) {
                Register(group, colliders[i]);
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < colliders.Length; ++i) {
                Unregister(registeredGroup, colliders[i]);
            }
        }
    }
}