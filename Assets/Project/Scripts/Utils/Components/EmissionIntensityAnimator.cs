using UnityEngine;

public class EmissionIntensityAnimator : MonoBehaviour
{
    // Propriétés du script
    public float speed = 1.0f;
    public Vector2 intensityRange = new Vector2(0.5f, 1.5f);
    public Color intensityColor = Color.white;

    private Material material;


    void Start()
    {
        // Obtention du matériau du rendu
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) material = renderer.material;
    }

    void Update()
    {
        if (material == null) return;

        // Calcul du facteur d'oscillation basé sur le temps
        float emissionFactor = Mathf.Sin(Time.time * speed);
        // Calcul de l'intensité d'émission en fonction du facteur d'oscillation et de la plage d'intensité
        float newIntensity = Mathf.Lerp(intensityRange.x, intensityRange.y, (emissionFactor + 1) / 2);

        // Application de la nouvelle intensité d'émission au matériau
        float factor = Mathf.Pow(2, newIntensity);
        material.SetColor("_EmissionColor", intensityColor * factor);
    }
}