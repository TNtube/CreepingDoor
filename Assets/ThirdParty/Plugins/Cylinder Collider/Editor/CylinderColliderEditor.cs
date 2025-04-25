using System;
using cylinder_collider;
using UnityEditor;
using UnityEngine;

namespace cylinder_collider_editor
{
    [CustomEditor(typeof(CylinderCollider))]
    [CanEditMultipleObjects]
    public class CylinderColliderEditor : Editor
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

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("colliderRoot"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("colliders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isTrigger"));
#if UNITY_2022_3_OR_NEWER
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_provideContacts"));
#endif
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dontNeedCallbacks"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_material"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_shape"));
            if (((CylinderCollider)target).shape == CylinderCollider.Shape.Pipe)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_pipeThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_resolution"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_center"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_radius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_height"));
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
                    ((CylinderCollider)t).Validate();
                }
            }
        }

        void OnSceneGUI()
        {
            if (_isEditMode)
            {
                Undo.RecordObject(target, "Modify Cylinder Collider");
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
            ((CylinderCollider)target).Validate();
        }

        void DrawHandles()
        {
            var cc = (CylinderCollider)target;
            Handles.matrix = Matrix4x4.TRS(cc.transform.position, cc.transform.rotation, cc.transform.localScale);

            Handles.color = new Color(0.6f, 1f, 0.6f);
            cc.radius = DoRadiusHandle(cc, Vector3.up, out var center);
            cc.center = center;
            cc.radius = DoRadiusHandle(cc, Vector3.right, out center);
            cc.center = center;
            cc.radius = DoRadiusHandle(cc, Vector3.down, out center);
            cc.center = center;
            cc.radius = DoRadiusHandle(cc, Vector3.left, out center);
            cc.center = center;
            cc.height = DoHeightHandle(cc, out center);
            cc.center = center;

            if (cc.shape == CylinderCollider.Shape.Pipe)
            {
                cc.pipeThickness = DoThicknessHandle(cc);
            }
        }


        static float DoRadiusHandle(CylinderCollider cc, Vector3 handleDir, out Vector3 newCenter)
        {
            var center = cc.center;
            var radius = cc.radius;
            var height = cc.height;
            var dir = DirectionToVector(cc.direction);
            newCenter = center;
            Vector3 viewVector;
            var discCenter = center + dir * (height * 0.5f);
            if (Camera.current.orthographic)
            {
                viewVector = Camera.current.transform.forward;
                Handles.DrawWireDisc(discCenter, dir, radius);
            }
            else
            {
                var matrix4x4 = Matrix4x4.Inverse(Handles.matrix);
                viewVector = discCenter - matrix4x4.MultiplyPoint(Camera.current.transform.position);

                Handles.DrawWireDisc(discCenter, dir, radius);
            }

            var controlId = GUIUtility.GetControlID("RadiusHandle".GetHashCode(), FocusType.Passive);
            var viewAngle = Vector3.Angle(dir, -viewVector);
            if (viewAngle > 5.0 && viewAngle < 175.0 || GUIUtility.hotControl == controlId)
            {
                var changed = GUI.changed;
                GUI.changed = false;

                handleDir = Quaternion.LookRotation(dir) * handleDir;
                
                var handlePos = discCenter + handleDir * radius;
                var newHandlePos = Handles.Slider(controlId, handlePos, handleDir,
                    HandleUtility.GetHandleSize(handlePos) * 0.04f, Handles.DotHandleCap, 0.0f);
                
                var delta = newHandlePos - handlePos;
                if (GUI.changed)
                {
                    if(!Event.current.alt)
                    {
                        newCenter += delta * 0.5f;
                        discCenter = newCenter + dir * (height * 0.5f);
                    }
                    radius = Vector3.Distance(newHandlePos, discCenter);
                }
                GUI.changed |= changed;
            }

            return radius;
        }

        static float DoThicknessHandle(CylinderCollider cc)
        {
            var center = cc.center;
            var radius = cc.radius * (1 - cc.pipeThickness);
            var height = cc.height;
            var dir = DirectionToVector(cc.direction);
            Vector3 viewVector;
            var discCenter = center + dir * (height * 0.5f);
            if (Camera.current.orthographic)
            {
                viewVector = Camera.current.transform.forward;
                Handles.DrawWireDisc(discCenter, dir, radius);
            }
            else
            {
                var matrix4x4 = Matrix4x4.Inverse(Handles.matrix);
                viewVector = discCenter - matrix4x4.MultiplyPoint(Camera.current.transform.position);

                Handles.DrawWireDisc(discCenter, dir, radius);
            }

            var controlId = GUIUtility.GetControlID("RadiusHandle".GetHashCode(), FocusType.Passive);
            var viewAngle = Vector3.Angle(dir, -viewVector);
            if (viewAngle > 5.0 && viewAngle < 175.0 || GUIUtility.hotControl == controlId)
            {
                var changed = GUI.changed;
                GUI.changed = false;

                Vector3 handleDir;
                switch (cc.direction)
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

                var handlePos = discCenter + handleDir * radius;
                var newHandlePos = Handles.Slider(controlId, handlePos, handleDir,
                    HandleUtility.GetHandleSize(handlePos) * 0.04f, Handles.DotHandleCap, 0.0f);
                if (GUI.changed)
                    radius = Vector3.Distance(newHandlePos, discCenter);
                GUI.changed |= changed;
            }

            if (radius > cc.radius) radius = cc.radius;

            return (1 - radius / cc.radius);
        }

        static float DoHeightHandle(CylinderCollider cc, out Vector3 newCenter)
        {
            var height = cc.height;
            var center = cc.center;
            var dir = DirectionToVector(cc.direction);
            Vector3 viewVector;
            newCenter = center;
            if (Camera.current.orthographic)
            {
                viewVector = Camera.current.transform.forward;
            }
            else
            {
                Matrix4x4 matrix4x4 = Matrix4x4.Inverse(Handles.matrix);
                viewVector = center - matrix4x4.MultiplyPoint(Camera.current.transform.position);
            }

            for (int i = 0; i < 2; i++)
            {
                var normal = i == 0 ? dir : -dir;
                var controlId = GUIUtility.GetControlID("HeightHandle".GetHashCode(), FocusType.Passive);
                var viewAngle = Vector3.Angle(Vector3.up, -viewVector);
                if (viewAngle > 5.0 && viewAngle < 175.0 || GUIUtility.hotControl == controlId)
                {
                    var changed = GUI.changed;
                    GUI.changed = false;

                    var handlePos = center + normal * (height * 0.5f);
                    var newHandlePos = Handles.Slider(controlId, handlePos, normal,
                        HandleUtility.GetHandleSize(handlePos) * 0.04f, Handles.DotHandleCap, 0.0f);
                    if (GUI.changed)
                    {
                        var lastHeight = height;
                        height = Vector3.Distance(newHandlePos, center) * 2;
                        if (!Event.current.alt)
                        {
                            var delta = height - lastHeight;
                            delta *= 0.5f;
                            height -= delta;
                            newCenter += normal * (delta * 0.5f);    
                        }
                    }

                    GUI.changed |= changed;
                }
            }

            return height;
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