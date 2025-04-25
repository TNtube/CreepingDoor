using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utils
{
    public static class LayerHelper
    {
        public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
