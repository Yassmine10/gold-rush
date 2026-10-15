using UnityEngine;

/// <summary>
/// Fait tourner la pièce sur elle-même et la fait flotter légèrement de haut en bas.
/// Parfait pour les pièces flottantes du menu d'accueil et du jeu "Gold Rush".
/// </summary>
public class CoinSpinner : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Vitesse de rotation sur l'axe Y (degrés par seconde)")]
    public float rotationSpeed = 120f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Flottement vertical (Bobbing)")]
    [Tooltip("Activer le léger mouvement haut/bas")]
    public bool enableBobbing = true;
    [Tooltip("Amplitude du mouvement vertical (ex: 0.1f à 0.3f)")]
    public float bobbingAmplitude = 0.15f;
    [Tooltip("Vitesse d'oscillation")]
    public float bobbingFrequency = 2f;

    [Header("Variation aléatoire")]
    [Tooltip("Décalage pour éviter que toutes les pièces oscillent en même temps")]
    public bool randomizeOffset = true;

    private Vector3 initialPosition;
    private float timeOffset;

    void Start()
    {
        initialPosition = transform.localPosition;
        if (randomizeOffset)
        {
            timeOffset = Random.Range(0f, 10f);
        }
    }

    void Update()
    {
        // 1. Rotation continue sur elle-même
        transform.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime), Space.Self);

        // 2. Mouvement de va-et-vient haut / bas
        if (enableBobbing)
        {
            float newY = initialPosition.y + Mathf.Sin((Time.time + timeOffset) * bobbingFrequency) * bobbingAmplitude;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
    }
}
