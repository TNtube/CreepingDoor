using UnityEngine;

[AddComponentMenu("Project/Utils/Screen Shake Helper")]
public class ScreenShakeHelper : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField]
    private float magnitude = 1f;
    [SerializeField]
    private float length = 2f;

    /// <summary>
    /// Trigger a shake event
    /// </summary>
    /// <param name="magnitude">Magnitude of the shaking. Should range from 0 - 1</param>
    /// <param name="length">Length of the shake event.</param>
    public static void Shake(float magnitude, float length)
    {
        ShakeRenderPass.Shake(magnitude, length);
    }

    /// <summary>
    /// Trigger a shake event
    /// </summary>
    /// <param name="magnitude">Magnitude of the shaking. Should range from 0 - 1</param>
    /// <param name="length">Length of the shake event.</param>
    public void ShakeTrigger(float magnitude, float length)
    {
        Shake(magnitude, length);
    }

    /// <summary>
    /// Trigger a shake event
    /// </summary>
    public void ShakeTrigger()
    {
        Shake(magnitude, length);
    }
}