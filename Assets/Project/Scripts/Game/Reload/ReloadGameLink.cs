using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Project/Game/Reload Game Link")]
public class ReloadGameLink : MonoBehaviour
{
    public void Reload()
    {
        ReloadGame.Reload();
    }
}