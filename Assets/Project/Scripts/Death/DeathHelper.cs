using System.Collections;
using System.Linq;
using UnityEngine;
using Project.Utils;

public static class DeathHelper
{
    public static void OnDeathCallback(MonoBehaviour behaviour) => OnDeathCallback(behaviour, true);
    public static void OnDeathCallback(MonoBehaviour behaviour, bool destroyObject)
    {
        if (behaviour == null) return;
        behaviour.StartCoroutine(OnDeathCallbackInternal(behaviour, destroyObject));
    }

    private static IEnumerator OnDeathCallbackInternal(MonoBehaviour behaviour, bool destroyObject)
    {
        IDeathCallback[] callbackHolders = behaviour.GetComponentsInChildren<IDeathCallback>();
        IEnumerator[] callbacks = callbackHolders.Select((IDeathCallback c) => c.OnDeathCallback()).ToArray();
        if (callbacks.Length > 0) yield return new WaitAll(behaviour, callbacks);
        if (destroyObject) GameObject.Destroy(behaviour.gameObject);
    }
}