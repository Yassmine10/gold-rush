using UnityEngine;

/// <summary>
/// Anime le personnage sur l'écran d'accueil avec une respiration / flottement vertical doux (haut et bas)
/// et une légère inclinaison / respiration naturelle.
/// </summary>
public class FloatingCharacter : MonoBehaviour
{
    [Header("Mouvement Haut / Bas")]
    [Tooltip("Amplitude du mouvement vertical (léger déplacement haut/bas)")]
    public float verticalAmplitude = 0.1f;
    [Tooltip("Vitesse de l'animation haut/bas")]
    public float verticalSpeed = 2.5f;

    [Header("Effet de Respiration (Squash & Stretch léger)")]
    public bool enableBreathingScale = true;
    public float scaleIntensity = 0.03f;
    public float breatheSpeed = 2.5f;

    [Header("Légère Inclinaison (Wobble)")]
    public bool enableTilt = true;
    public float tiltAngle = 3f;
    public float tiltSpeed = 1.8f;

    private Vector3 startLocalPos;
    private Vector3 startLocalScale;
    private Quaternion startLocalRot;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalScale = transform.localScale;
        startLocalRot = transform.localRotation;
    }

    void Update()
    {
        float time = Time.time;

        // 1. Déplacement vertical doux (Haut / Bas)
        float offsetY = Mathf.Sin(time * verticalSpeed) * verticalAmplitude;
        transform.localPosition = startLocalPos + new Vector3(0, offsetY, 0);

        // 2. Respiration (légère compression/étirement)
        if (enableBreathingScale)
        {
            float scaleY = 1f + Mathf.Sin(time * breatheSpeed) * scaleIntensity;
            float scaleXZ = 1f - (Mathf.Sin(time * breatheSpeed) * (scaleIntensity * 0.5f));
            transform.localScale = new Vector3(
                startLocalScale.x * scaleXZ,
                startLocalScale.y * scaleY,
                startLocalScale.z * scaleXZ
            );
        }

        // 3. Légère inclinaison mignonne
        if (enableTilt)
        {
            float tiltZ = Mathf.Sin(time * tiltSpeed) * tiltAngle;
            transform.localRotation = startLocalRot * Quaternion.Euler(0, 0, tiltZ);
        }
    }
}
