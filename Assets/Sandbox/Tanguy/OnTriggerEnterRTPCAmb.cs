using UnityEngine;

public class OnTriggerEnterRTPCAmb : MonoBehaviour
{
    public AK.Wwise.Event Amb_Placeholder;
    public void Start()
    {
        Amb_Placeholder.Post(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        AkUnitySoundEngine.SetRTPCValue("Wind_Intensity", 100.0f, gameObject);
        AkUnitySoundEngine.SetRTPCValue("Water_Intensity", 100.0f, gameObject);
        AkUnitySoundEngine.SetRTPCValue("RumbleIntensity", 100.0f, gameObject);
        Debug.Log("Entered");
    }

}
