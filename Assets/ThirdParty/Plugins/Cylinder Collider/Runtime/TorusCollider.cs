using System;
using System.Collections.Generic;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace cylinder_collider
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TorusCollider : MonoBehaviour
    {
        [HideInInspector] public Transform colliderRoot;
        [HideInInspector] public List<CapsuleCollider> colliders = new List<CapsuleCollider>();

#if UNITY_6000_0_OR_NEWER
        public PhysicsMaterial material
#else
        public PhysicMaterial material
#endif
        {
            get => colliders == null || colliders.Count == 0 ? null : colliders[0].material;
            set
            {
                foreach (var col in colliders)
                {
                    col.material = value;
                }
            }
        }

#if UNITY_6000_0_OR_NEWER
        public PhysicsMaterial sharedMaterial
#else
        public PhysicMaterial sharedMaterial
#endif
        {
            get => colliders == null || colliders.Count == 0 ? null : colliders[0].sharedMaterial;
            set
            {
                foreach (var col in colliders)
                {
                    col.sharedMaterial = value;
                }
            }
        }

        public bool isTrigger
        {
            get => _isTrigger;
            set
            {
                _isTrigger = value;
                UpdateIsTrigger();
            }
        }

        public Vector3 center
        {
            get => _center;
            set
            {
                _center = value;
                UpdateCenter();
            }
        }

        public float radius
        {
            get => _radius;
            set
            {
                if (value < 0) value = 0;
                _radius = value;
                UpdateSize();
            }
        }

        public float thickness
        {
            get => _thickness;
            set
            {
                _thickness = Mathf.Clamp01(value);
                UpdateSize();
            }
        }

        public int resolution
        {
            get => _resolution;
            set
            {
                if (value < 1) value = 1;
                _resolution = value;
                UpdateResolution();
            }
        }

        public Direction direction
        {
            get => _direction;
            set
            {
                if (!Enum.IsDefined(typeof(Direction), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Direction));
                direction = value;
                UpdateDirection();
            }
        }

        public Bounds bounds
        {
            get
            {
                var v = colliders[0].bounds;
                for (int i = 1; i < colliders.Count; i++)
                {
                    v.Encapsulate(colliders[i].bounds);
                }

                return v;
            }
        }

        public Rigidbody attachedRigidbody =>
            colliders == null || colliders.Count == 0 ? null : colliders[0].attachedRigidbody;

        public float contactOffset
        {
            get => colliders == null || colliders.Count == 0
                ? Physics.defaultContactOffset
                : colliders[0].contactOffset;
            set
            {
                foreach (var col in colliders)
                {
                    col.contactOffset = value;
                }
            }
        }

#if UNITY_2020_1_OR_NEWER
        public ArticulationBody attachedArticulationBody => colliders == null || colliders.Count == 0
            ? null
            : colliders[0].attachedArticulationBody;
#endif
#if UNITY_2021_2_OR_NEWER
        public bool hasModifiableContacts
        {
            get => colliders == null || colliders.Count == 0 ? false : colliders[0].hasModifiableContacts;
            set
            {
                foreach (var col in colliders)
                {
                    col.hasModifiableContacts = value;
                }
            }
        }
#endif
#if UNITY_2022_3_OR_NEWER
        public bool provideContacts
        {
            get => _provideContacts;
            set
            {
                _provideContacts = value;
                UpdateProvideContacts();
            }
        }

        public int layerOverrideProperty
        {
            get => _layerOverridePriority;
            set
            {
                _layerOverridePriority = value;
                UpdateLayerOverrides();
            }
        }

        public LayerMask includeLayers
        {
            get => _includeLayers;
            set
            {
                _includeLayers = value;
                UpdateLayerOverrides();
            }
        }
        
        public LayerMask excludeLayers
        {
            get => _excludeLayers;
            set
            {
                _excludeLayers = value;
                UpdateLayerOverrides();
            }
        }
#endif
        
        /// <summary>
        /// If you don't need the callbacks(e.g. OnTriggerEnter, OnCollisionEnter) set this true.
        /// TorusCollider is composed of multiple child colliders
        /// and uses a separate event management class to call triggers
        /// that can be called multiple times only once.
        /// If this variable is checked as true, the event management class will not be created and overrides
        /// for these callbacks will not be created, which helps with optimization.
        /// </summary>
        public bool dontNeedCallbacks
        {
            get => _dontNeedCallbacks;
            set
            {
                _dontNeedCallbacks = value;
                UpdatePhysicsListner();
            }
        }
        
        
        [SerializeField] bool _isTrigger;
#if UNITY_6000_0_OR_NEWER
        [SerializeField] PhysicsMaterial _material;
#else
        [SerializeField] PhysicMaterial _material;
#endif
        [SerializeField] [Range(1, 50)] int _resolution = 4;
        [SerializeField] Vector3 _center;
        [SerializeField] float _radius = 0.5f;
        [SerializeField] [Range(0, 1)] float _thickness = 0.25f;
        [SerializeField] Direction _direction = Direction.YAxis;
        [Tooltip("If you don't need the callbacks(e.g. OnTriggerEnter, OnCollisionEnter) set this true.\nTorusCollider is composed of multiple child colliders and uses a separate event management class to call triggers that can be called multiple times only once.\nIf this variable is checked as true, the event management class will not be created and overrides for these callbacks will not be created, which helps with optimization.")]
        [SerializeField] bool _dontNeedCallbacks;
#if UNITY_2022_3_OR_NEWER
        [SerializeField] bool _provideContacts;
        [SerializeField] int _layerOverridePriority;
        [SerializeField] LayerMask _includeLayers;
        [SerializeField] LayerMask _excludeLayers;  
#endif
        internal readonly HashSet<Collider> triggerEnterRequests = new HashSet<Collider>();
        internal readonly HashSet<Collider> triggerStayRequests = new HashSet<Collider>();
        internal readonly HashSet<Collider> triggerExitRequests = new HashSet<Collider>();
        internal readonly HashSet<Collision> collisionEnterRequests = new HashSet<Collision>();
        internal readonly HashSet<Collision> collisionStayRequests = new HashSet<Collision>();
        internal readonly HashSet<Collision> collisionExitRequests = new HashSet<Collision>();
        int desiredColliderCount => (resolution + 2) * 2;
        int polyCount => (resolution + 2) * 2;
        List<PhysicsListener> _physicsListeners = new List<PhysicsListener>();
        #region Unity methods

        void Reset()
        {
            CalcBounds();
            CreateRoot();
            GetComponentsInChildren(colliders);
            UpdateResolution();
            UpdateCenter();
            UpdateDirection();
        }

        // It works instead of OnValidate(). Because creating or destroying GameObject OnValidate makes errors in Built-in RenderPipeline
        public void Validate()
        {
            if (_radius < 0) _radius = 0;
            _thickness = Mathf.Clamp01(_thickness);
            CreateRoot();
            UpdateResolution();
            UpdateIsTrigger();
            UpdateCenter();
            UpdateDirection();
#if UNITY_2022_3_OR_NEWER
            UpdateProvideContacts();
            UpdateLayerOverrides();
#endif
            sharedMaterial = _material;
            UpdatePhysicsListner();
        }
        void UpdatePhysicsListner()
        {
            if (dontNeedCallbacks)
            {
                for (int i = 0; i < _physicsListeners.Count; i++)
                {
                    if(Application.isPlaying) Destroy(_physicsListeners[i]);
                    else DestroyImmediate(_physicsListeners[i]);
                }
                _physicsListeners.Clear();
            }
            else
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    if(!colliders[i].TryGetComponent(out PhysicsListener listener))
                    {
                        listener = colliders[i].gameObject.AddComponent<PhysicsListener>();
                        listener.type = PhysicsListener.Type.TorusCollider;
                        listener.torusCollider = this;
                        _physicsListeners.Add(listener);
                    }
                }   
            }
        }
        void OnEnable()
        {
            if (colliderRoot == null) return;
            colliderRoot.gameObject.SetActive(true);
            if(Application.isPlaying)CallbackPropagator.Register(this);
#if UNITY_EDITOR
            UpdatePhysicsListner();
#endif
        }

        void Start()
        {
            if (colliderRoot == null) Reset();
        }

        void OnDisable()
        {
            if (colliderRoot == null) return;
            colliderRoot.gameObject.SetActive(false);
            if(Application.isPlaying)CallbackPropagator.Unregister(this);
        }

        void OnDestroy()
        {
            if (colliderRoot == null) return;
            if (Application.isPlaying)
            {
                Destroy(colliderRoot.gameObject);
            }
#if UNITY_EDITOR
            else
            {
                if(Event.current != null && Event.current.type == EventType.DragExited)
                    DestroyImmediate(colliderRoot.gameObject);
                else Undo.DestroyObjectImmediate(colliderRoot.gameObject);
            }
#endif
        }

        #endregion


        void CreateRoot()
        {
            if (colliderRoot == null)
            {
                colliderRoot = transform.Find("Torus Collider Root");
                if (colliderRoot == null)
                {
                    colliderRoot = new GameObject("Torus Collider Root").transform;
                    colliderRoot.SetParent(transform);
                }
            }

            colliderRoot.localPosition = Vector3.zero;
            colliderRoot.localRotation = Quaternion.identity;
            colliderRoot.localScale = Vector3.one;

            colliderRoot.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        }

        void CalcBounds()
        {
            var b = gameObject.LocalRenderBounds();
            if (b == default) b = new Bounds(Vector3.zero, new Vector3(1,2,1));
            _center = transform.InverseTransformPoint(b.center);
            var localSize = transform.InverseTransformVector(b.size);
            _radius = Mathf.Max(localSize.x, localSize.z) / 2 * (1 - thickness);
        }

        void UpdateResolution()
        {
            if (colliderRoot == null) return;
            var childCount = colliderRoot.childCount;

            var diff = desiredColliderCount - childCount;
            colliders.Clear();
            if (diff > 0) // should add
            {
                for (int i = childCount; i < desiredColliderCount; i++)
                {
                    var col = new GameObject("col").AddComponent<CapsuleCollider>();
                    col.transform.SetParent(colliderRoot);
                    col.transform.localPosition = Vector3.zero;
                    var listener = col.gameObject.AddComponent<PhysicsListener>();
                    listener.type = PhysicsListener.Type.TorusCollider;
                    listener.torusCollider = this;
                    colliders.Add(col);
                }
            }
            else // should remove
            {
                for (int i = 0; i < -diff; i++)
                {
                    var col = colliderRoot.GetChild(colliderRoot.childCount - 1).GetComponent<CapsuleCollider>();
                    colliders.Remove(col);
                    if (Application.isPlaying)
                    {
                        Destroy(col.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(col.gameObject);
                    }
                }
            }

            var targetCount = colliderRoot.childCount;
            for (int i = 0; i < targetCount; i++)
            {
                colliders.Add(colliderRoot.GetChild(i).GetComponent<CapsuleCollider>());
            }

            UpdateSize();
        }

        void UpdateSize(CapsuleCollider col, int index)
        {
            var h = 2f * radius * Mathf.Sin(Mathf.PI / polyCount) + col.radius;
            var r = Mathf.Lerp(0, radius, thickness);

            if (float.IsNaN(r))
            {
                r = 0;
            }

            col.center = new Vector3(0, 0, radius);
            col.radius = r;
            col.height = h + r;
            col.transform.localRotation = Quaternion.Euler(0, index * (360f / polyCount), 0);
            col.direction = 0;
        }

        void UpdateSize()
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                UpdateSize(colliders[i], i);
            }
        }

        void UpdateIsTrigger()
        {
            foreach (var col in colliders)
            {
                col.isTrigger = _isTrigger;
            }
        }

        void UpdateCenter()
        {
            colliderRoot.localPosition = center;
        }

        void UpdateDirection()
        {
            switch (direction)
            {
                case Direction.XAxis:
                    colliderRoot.localRotation = Quaternion.Euler(0, 180, 90);
                    break;
                case Direction.YAxis:
                    colliderRoot.localRotation = Quaternion.identity;
                    break;
                case Direction.ZAxis:
                    colliderRoot.localRotation = Quaternion.Euler(-90, 180, 0);
                    break;
            }
        }
                
#if UNITY_2022_3_OR_NEWER
        void UpdateProvideContacts()
        {
            foreach (var col in colliders)
            {
                col.providesContacts = _provideContacts;
            }
        }

        void UpdateLayerOverrides()
        {
            foreach (var col in colliders)
            {
                col.layerOverridePriority = _layerOverridePriority;
                col.includeLayers = _includeLayers;
                col.excludeLayers = _excludeLayers;
            }
        }
#endif

        internal void RequestTriggerEnter(Collider other)
        {
            if(attachedRigidbody != null) return;
            triggerEnterRequests.Add(other);
        }

        internal void RequestTriggerStay(Collider other)
        {
            if(attachedRigidbody != null) return;
            triggerStayRequests.Add(other);
        }

        internal void RequestTriggerExit(Collider other)
        {
            if(attachedRigidbody != null) return;
            triggerExitRequests.Add(other);
        }
        internal void RequestCollisionEnter(Collision other)
        {
            if(attachedRigidbody != null) return;
            collisionEnterRequests.Add(other);
        }

        internal void RequestCollisionStay(Collision other)
        {
            if(attachedRigidbody != null) return;
            collisionStayRequests.Add(other);
        }

        internal void RequestCollisionExit(Collision other)
        {
            if(attachedRigidbody != null) return;
            collisionExitRequests.Add(other);
        }
        
        public bool Contains(int colliderID)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].GetInstanceID() == colliderID) return true;
            }

            return false;
        }

        #region Collider API

        public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                var wasHit = colliders[i].Raycast(ray, out hitInfo, maxDistance);
                if (wasHit) return true;
            }

            hitInfo = default;
            return false;
        }

        public Vector3 ClosestPoint(Vector3 position)
        {
            Vector3 closest = Vector3.positiveInfinity;
            for (int i = 0; i < colliders.Count; i++)
            {
                var point = colliders[i].ClosestPoint(position);
                if (point.sqrMagnitude < closest.sqrMagnitude) closest = point;
            }

            return closest;
        }

        public Vector3 ClosestPointOnBounds(Vector3 position)
        {
            Vector3 closest = Vector3.positiveInfinity;
            for (int i = 0; i < colliders.Count; i++)
            {
                var point = colliders[i].ClosestPointOnBounds(position);
                if (point.sqrMagnitude < closest.sqrMagnitude) closest = point;
            }

            return closest;
        }

        #endregion
    }
}