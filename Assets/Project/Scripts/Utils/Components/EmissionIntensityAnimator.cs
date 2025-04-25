using UnityEngine;

public class EmissionIntensityAnimator : MonoBehaviour
{
    // Propri�t�s du script
    public float speed = 1.0f;
    public Vector2 intensityRange = new Vector2(0.5f, 1.5f);
    public Color intensityColor = Color.white;

    private Material material;


    void Start()
    {
        // Obtention du mat�riau du rendu
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) material = renderer.material;
    }

    void Update()
    {
        if (material == null) return;

        // Calcul du facteur d'oscillation bas� sur le temps
        float emissionFactor = Mathf.Sin(Time.time * speed);
        // Calcul de l'intensit� d'�mission en fonction du facteur d'oscillation et de la plage d'intensit�
        float newIntensity = Mathf.Lerp(intensityRange.x, intensityRange.y, (emissionFactor + 1) / 2);

        // Application de la nouvelle intensit� d'�mission au mat�riau
        float factor = Mathf.Pow(2, newIntensity);
        material.SetColor("_EmissionColor", intensityColor * factor);
    }
}