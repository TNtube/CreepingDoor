using System;
using cylinder_collider;
using UnityEditor;
using UnityEngine;

namespace cylinder_collider_editor
{
    [CustomEditor(typeof(TorusCollider))]
    public class TorusColliderEditor : Editor
    {
        Texture2D _editIcon;
        bool _isEditMode;
        bool _modified;
        static bool _layerOverridesFoldout = true;
        int _undoID;

        void OnEnable()
        {
            _editIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Cylinder Collider/Editor/Icons/Edit Icon.png");
            Undo.undoRedoPerformed += Validate;
        }

        void OnDisable()
        {
            _isEditMode = false;
            Undo.undoRedoPerformed -= Validate;
        }

        public override void OnInspectorGUI()
        {
            if (_isEditMode) GUI.color = Color.white * 1.25f;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Edit Collider");
            if (GUILayout.Button(_editIcon))
            {
                _isEditMode = !_isEditMode;
                if (_isEditMode) _undoID = Undo.GetCurrentGroup();
                SceneView.RepaintAll();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.color = Color.white;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isTrigger"));
#if UNITY_2022_3_OR_NEWER
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_provideContacts"));
#endif
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dontNeedCallbacks"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_material"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_resolution"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_center"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_radius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_thickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_direction"));
            
#if UNITY_2022_3_OR_NEWER
            _layerOverridesFoldout = EditorGUILayout.Foldout(_layerOverridesFoldout, "Layer Overrides", true);
            if (_layerOverridesFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_layerOverridePriority"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeLayers"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_excludeLayers"));
                EditorGUI.indentLevel--;
            }
#endif

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (var t in targets)
                {
                    ((TorusCollider)t).Validate();
                }
            }
        }

        void OnSceneGUI()
        {
            if (_isEditMode)
            {
                Undo.RecordObject(target, "Modify Torus Collider");
                EditorGUI.BeginChangeCheck();
                DrawHandles();
                if (EditorGUI.EndChangeCheck())
                {
                    _modified = true;
                    Undo.CollapseUndoOperations(_undoID);
                    EditorUtility.SetDirty(target);
                }

                if (!_modified) Undo.ClearUndo(target);
                else Undo.CollapseUndoOperations(_undoID);
            }
        }

        void Validate()
        {
            ((TorusCollider)target).Validate();
        }

        void DrawHandles()
        {
            var tc = (TorusCollider)target;
            Handles.matrix = Matrix4x4.TRS(tc.transform.position, tc.transform.rotation, tc.transform.localScale);

            Handles.color = new Color(0.6f, 1f, 0.6f);
            tc.radius = DoRadiusHandle(tc, Vector3.up, out var center);
            tc.center = center;
            tc.radius = DoRadiusHandle(tc, Vector3.right, out center);
            tc.center = center;
            tc.radius = DoRadiusHandle(tc, Vector3.left, out center);
            tc.center = center;
            tc.radius = DoRadiusHandle(tc, Vector3.down, out center);
            tc.center = center;
            tc.thickness = DoThicknessHandle(tc);
        }

        static float DoRadiusHandle(TorusCollider tc, Vector3 handleDir, out Vector3 newCenter)
        {
            var center = tc.center;
            var radius = tc.radius;
            var dir = DirectionToVector(tc.direction);
            newCenter = center;
            Vector3 viewVector;
            if (Camera.current.orthographic)
            {
                viewVector = Camera.current.transform.forward;
                Handles.DrawWireDisc(center, dir, radius);
            }
            else
            {
                var matrix4x4 = Matrix4x4.Inverse(Handles.matrix);
                viewVector = center - matrix4x4.MultiplyPoint(Camera.current.transform.position);

                Handles.DrawWireDisc(center, dir, radius);
            }

            var controlId = GUIUtility.GetControlID("RadiusHandle".GetHashCode(), FocusType.Passive);
            var viewAngle = Vector3.Angle(Vector3.up, -viewVector);
            if (viewAngle > 5.0 && viewAngle < 175.0 || GUIUtility.hotControl == controlId)
            {
                var changed = GUI.changed;
                GUI.changed = false;


                handleDir = Quaternion.LookRotation(dir) * handleDir;

                var handlePos = center + handleDir * radius;
                var newHandlePos = Handles.Slider(controlId, handlePos, handleDir,
                    HandleUtility.GetHandleSize(handlePos) * 0.04f, Handles.DotHandleCap, 0.0f);
                var delta = newHandlePos - handlePos;
                if (GUI.changed)
                {
                    if (!Event.current.alt)
                    {
                        newCenter += delta * 0.5f;
                    }
                    radius = Vector3.Distance(newHandlePos, newCenter);
                }
                GUI.changed |= changed;
            }

            return radius;
        }

        static float DoThicknessHandle(TorusCollider tc)
        {
            var center = tc.center;
            var radius = tc.radius;
            var thickness = tc.thickness;
            var dir = DirectionToVector(tc.direction);
            Vector3 viewVector;

            Vector3 discOffsetDir;
            switch (tc.direction)
            {
                case Direction.XAxis:
                    discOffsetDir = Vector3.up;
                    break;
                case Direction.YAxis:
                    discOffsetDir = Vector3.right;
                    break;
                case Direction.ZAxis:
                    discOffsetDir = Vector3.up;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var discCenter = center + discOffsetDir * radius;

            Vector3 discDir;
            switch (tc.direction)
            {
                case Direction.XAxis:
                    discDir = Vector3.right;
                    break;
                case Direction.YAxis:
                    discDir = Vector3.forward;
                    break;
                case Direction.ZAxis:
                    discDir = Vector3.right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Camera.current.orthographic)
            {
                viewVector = Camera.current.transform.forward;
                Handles.DrawWireDisc(discCenter, discDir, radius * thickness);
            }
            else
            {
                var matrix4x4 = Matrix4x4.Inverse(Handles.matrix);
                viewVector = center - matrix4x4.MultiplyPoint(Camera.current.transform.position);

                Handles.DrawWireDisc(discCenter, discDir, radius * thickness);
            }

            var controlId = GUIUtility.GetControlID("ThicknessHandle".GetHashCode(), FocusType.Passive);
            var viewAngle = Vector3.Angle(Vector3.up, -viewVector);
            if (viewAngle > 5.0 && viewAngle < 175.0 || GUIUtility.hotControl == controlId)
            {
                var changed = GUI.changed;
                GUI.changed = false;

                Vector3 handleDir;
                switch (tc.direction)
                {
                    case Direction.XAxis:
                        handleDir = Vector3.up;
                        break;
                    case Direction.YAxis:
                        handleDir = Vector3.right;
                        break;
                    case Direction.ZAxis:
                        handleDir = Vector3.up;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var handlePos = discCenter + handleDir * (radius * thickness);
                var newHandlePos = Handles.Slider(controlId, handlePos, handleDir,
                    HandleUtility.GetHandleSize(handlePos) * 0.04f, Handles.DotHandleCap, 0.0f);
                if (GUI.changed)
                {
                    var capsuleRadius = Vector3.Distance(newHandlePos, discCenter);
                    thickness = capsuleRadius / radius;
                }

                GUI.changed |= changed;
            }

            return thickness;
        }

        static Vector3 DirectionToVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.XAxis:
                    return Vector3.right;
                case Direction.YAxis:
                    return Vector3.up;
                case Direction.ZAxis:
                    return Vector3.forward;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}