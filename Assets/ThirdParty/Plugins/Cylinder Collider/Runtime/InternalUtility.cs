using UnityEngine;

namespace cylinder_collider
{
    internal static class InternalUtility
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Get Instance ID")]
        public static void GetInstanceID(UnityEditor.MenuCommand command)
        {
            var target = command.context as GameObject;
            Debug.Log(target.GetInstanceID());
        }
#endif

        public static Bounds LocalRenderBounds(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            Bounds b = new Bounds(Vector3.zero, Vector3.zero);
            var corners = new Vector3[8];
            if(renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    var r = renderers[i];
#if UNITY_2021_2_OR_NEWER
                    var lb = r.localBounds;
#else
                    var t = gameObject.transform;
                    var lb = r.bounds;
                    
                    Matrix4x4 matrix = t.parent != null ?
                        t.parent.localToWorldMatrix.inverse : t.worldToLocalMatrix;
                    
                    Vector3 localPosition = matrix.MultiplyPoint3x4(t.position);
                    lb.center = localPosition;

#endif
                    
                    if (r.transform == transform)
                    {
                        b.Encapsulate(lb);
                    }
                    else
                    {
                        var rt = r.transform;
                        var center = transform.InverseTransformPoint(rt.position);
                        var size = new Vector3(
                            rt.lossyScale.x * lb.size.x,
                            rt.lossyScale.y * lb.size.y,
                            rt.lossyScale.z * lb.size.z);
                        
                        var localRot = Quaternion.Inverse(transform.rotation) * rt.rotation;
                        
                        size = localRot * size;
                        var extents = lb.extents;

                        corners[0] = new Vector3(rt.lossyScale.x *  -extents.x, rt.lossyScale.y * -extents.y, rt.lossyScale.z * -extents.z); // Left-Bottom-Back
                        corners[1] = new Vector3(rt.lossyScale.x *  extents.x , rt.lossyScale.y * -extents.y, rt.lossyScale.z * -extents.z);  // Right-Bottom-Back
                        corners[2] = new Vector3(rt.lossyScale.x *  -extents.x, rt.lossyScale.y * -extents.y, rt.lossyScale.z * extents.z);  // Left-Bottom-Front
                        corners[3] = new Vector3(rt.lossyScale.x *  extents.x , rt.lossyScale.y * -extents.y, rt.lossyScale.z * extents.z);   // Right-Bottom-Front
                        corners[4] = new Vector3(rt.lossyScale.x *  -extents.x, rt.lossyScale.y * extents.y , rt.lossyScale.z * -extents.z);  // Left-Top-Back
                        corners[5] = new Vector3(rt.lossyScale.x *  extents.x , rt.lossyScale.y * extents.y , rt.lossyScale.z * -extents.z);   // Right-Top-Back
                        corners[6] = new Vector3(rt.lossyScale.x *  -extents.x, rt.lossyScale.y * extents.y , rt.lossyScale.z * extents.z);   // Left-Top-Front
                        corners[7] = new Vector3(rt.lossyScale.x *  extents.x , rt.lossyScale.y * extents.y , rt.lossyScale.z * extents.z);    // Right-Top-Front

                        var bb = new Bounds(center, size);
                        for (int ii = 0; ii < corners.Length; ii++)
                        {
                            corners[ii] = localRot * corners[ii];
                            corners[ii] += center;
                            bb.Encapsulate(corners[ii]);
                        }
                    
                        b.Encapsulate(bb);    
                    }
                    
                }
            }
            return b;
        }
    }
}