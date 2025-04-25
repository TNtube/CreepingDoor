using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Project.Utils;

[DisallowMultipleComponent]
[AddComponentMenu("Project/Death/Death Player")]
public class DeathPlayer : MonoBehaviour, IDeathCallback
{
    [SerializeField, Tooltip("This is used to trigger PlayerDeathMenu")]
    private UnityEvent deathMenuTrigger = new UnityEvent();

    public IEnumerator OnDeathCallback()
    {
        yield return null;
        deathMenuTrigger.Invoke();
        /*CharacterController cc = ReferenceHelper.CharacterController;
        if (cc != null) {
            cc.SimpleMove(Vector3.zero);
            cc.enabled = false;
            Vector3 localPos = cc.transform.localPosition;
            localPos.y = 0f;
            cc.transform.localPosition = localPos;
            cc.enabled = true;
        }*/
    }
}