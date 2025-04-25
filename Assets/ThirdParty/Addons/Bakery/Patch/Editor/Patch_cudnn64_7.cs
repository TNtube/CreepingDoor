using UnityEngine;

public class Patch_cudnn64_7 : ScriptableObject
{
    public const int Source = 1;
    public const int Destination = 2;
    [HideInInspector]
    public int data = 0;
}
