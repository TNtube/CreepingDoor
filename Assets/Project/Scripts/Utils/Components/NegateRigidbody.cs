using UnityEngine;

namespace Project.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Project/Utils/Negate Rigidbody")]
    public class NegateRigidbody : MonoBehaviour
    {
        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.maxAngularVelocity = 0f;
            rb.maxLinearVelocity = 0f;
        }
    }
}