using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace cylinder_collider
{
    [DisallowMultipleComponent]
    internal class PhysicsListener : MonoBehaviour
    {
        public enum Type
        {
            CylinderCollider,
            TorusCollider
        }
        public CylinderCollider cylinderCollider;
        public TorusCollider torusCollider;
        public Type type;
        void OnTriggerEnter(Collider other)
        {
            switch (type)
            {
                case Type.CylinderCollider:
                    cylinderCollider.RequestTriggerEnter(other);
                    break;
                case Type.TorusCollider:
                    torusCollider.RequestTriggerEnter(other);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        void OnTriggerStay(Collider other)
        {
            switch (type)
            {
                case Type.CylinderCollider:
                    cylinderCollider.RequestTriggerStay(other);
                    break;
                case Type.TorusCollider:
                    torusCollider.RequestTriggerStay(other);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        void OnTriggerExit(Collider other)
        {
            switch (type)
            {
                case Type.CylinderCollider:
                    cylinderCollider.RequestTriggerExit(other);
                    break;
                case Type.TorusCollider:
                    torusCollider.RequestTriggerExit(other);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        void OnCollisionEnter(Collision other)
        {
            switch (type)
            {
                case Type.CylinderCollider:
                    cylinderCollider.RequestCollisionEnter(other);
                    break;
                case Type.TorusCollider:
                    torusCollider.RequestCollisionEnter(other);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        void OnCollisionStay(Collision other)
        {
            switch (type)
            {
                case Type.CylinderCollider:
                    cylinderCollider.RequestCollisionStay(other);
                    break;
                case Type.TorusCollider:
                    torusCollider.RequestCollisionStay(other);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        void OnCollisionExit(Collision other)
        {
            switch (type)
            {
                case Type.CylinderCollider:
                    cylinderCollider.RequestCollisionExit(other);
                    break;
                case Type.TorusCollider:
                    torusCollider.RequestCollisionExit(other);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}