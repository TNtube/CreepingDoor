using UnityEngine;

namespace Project.Utils
{
    public static class PhysicsHelper
    {
        const int maxDepenetrationCount = 25;

        public struct TriggerPoint
        {
            public static TriggerPoint Default => new TriggerPoint {
                position = Vector3.positiveInfinity,
                normal = Vector3.up
            };

            public Vector3 position;
            public Vector3 normal;
        }



        public static void InitTrigger(Collider collider)
        {
            Rigidbody rigidbody = collider.attachedRigidbody;
            if (rigidbody == null) rigidbody = collider.gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            collider.isTrigger = true;
        }





        public static TriggerPoint GetTriggerPoint(Collider self, Collider other) => GetTriggerPoint(self, other, true);
        public static TriggerPoint GetTriggerPoint(Collider self, Collider other, bool getClosest)
        {
            // Check if Rigidbody is Good
            Rigidbody rigidbody = self.attachedRigidbody;
            if (rigidbody == null) return TriggerPoint.Default;

            // Setup Settings for processing
            Physics.autoSyncTransforms = true;
            Vector3 velocity = rigidbody.linearVelocity;
            Vector3 angularVelocity = rigidbody.angularVelocity;
            rigidbody.isKinematic = false;
            self.isTrigger = false;

            // Try Depenetrate if needed and possible
            int depenetrationCount = 0;
            Vector3 rigidbodyPosition = rigidbody.position, physicsMovement = Vector3.zero;
            self.transform.GetPositionAndRotation(out Vector3 selfPosition, out Quaternion selfRotation);
            other.transform.GetPositionAndRotation(out Vector3 otherPosition, out Quaternion otherRotation);
            while (Physics.ComputePenetration(self, selfPosition, selfRotation, other, otherPosition, otherRotation, out Vector3 separateDirection, out float separateDistance)) {
                if (++depenetrationCount >= maxDepenetrationCount) break;
                Vector3 movement = separateDirection * (separateDistance * 1.05f);
                physicsMovement += movement;
                selfPosition += movement;
            }
            rigidbody.position += physicsMovement;

            // Setup and Process SweepTest + Setup fallback if bad settings
            float distance = physicsMovement == Vector3.zero ? Mathf.Infinity : physicsMovement.magnitude;
            Vector3 direction = physicsMovement == Vector3.zero ? rigidbody.transform.forward : -physicsMovement;
            RaycastHit[] hits = rigidbody.SweepTestAll(direction, distance, QueryTriggerInteraction.Ignore);
            Vector3 hitPosition = rigidbody.ClosestPointOnBounds(other.transform.position);
            Vector3 hitNormal = (hitPosition - other.transform.position);

            // Try using SweepTest results, by distance if requested
            TriggerPoint result;
            if (getClosest) result = GetTriggerPointClosest(self, other, hits, hitPosition, hitNormal);
            else result = GetTriggerPointSimple(other, hits, hitPosition, hitNormal);

            // Reset Settings
            rigidbody.position = rigidbodyPosition;
            rigidbody.angularVelocity = angularVelocity;
            rigidbody.linearVelocity = velocity;
            rigidbody.isKinematic = true;
            self.isTrigger = true;
            Physics.autoSyncTransforms = false;

            // Return Result
            return result;
        }





        private static TriggerPoint GetTriggerPointSimple(Collider other, RaycastHit[] hits, Vector3 hitPosition, Vector3 hitNormal)
        {
            // Get First Hit corresponding
            Rigidbody otherRigidbody = other.attachedRigidbody;
            for (int i = 0; i < hits.Length; ++i) {
                Transform hitTransform = hits[i].transform;
                if (hitTransform != other.transform && (otherRigidbody == null || hitTransform != otherRigidbody.transform)) continue;
                hitPosition = hits[i].point;
                hitNormal = hits[i].normal;
                break;
            }

            // Return Processed Trigger Point
            return new TriggerPoint {
                position = hitPosition,
                normal = hitNormal
            };
        }

        private static TriggerPoint GetTriggerPointClosest(Collider self, Collider other, RaycastHit[] hits, Vector3 hitPosition, Vector3 hitNormal)
        {
            // Get Closest Hit corresponding
            Rigidbody otherRigidbody = other.attachedRigidbody;
            float closestDistance = float.PositiveInfinity;
            for (int i = 0; i < hits.Length; ++i) {
                Transform hitTransform = hits[i].transform;
                if (hitTransform != other.transform && (otherRigidbody == null || hitTransform != otherRigidbody.transform)) continue;
                float hitDistance = Vector3.Distance(self.transform.position, hits[i].point);
                if (hitDistance > closestDistance) continue;
                hitPosition = hits[i].point;
                hitNormal = hits[i].normal;
            }

            // Return Processed Trigger Point
            return new TriggerPoint {
                position = hitPosition,
                normal = hitNormal
            };
        }
    }

    
}