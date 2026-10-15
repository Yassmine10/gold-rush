using UnityEngine;

/// <summary>
/// Caméra fluide 2.5D qui suit le joueur horizontalement et verticalement avec un amortissement doux.
/// </summary>
public class CameraFollow25D : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, -10f);
    public float smoothSpeed = 5f;

    [Header("Limites de la caméra")]
    public bool useLimits = false;
    public float minX = 0f;
    public float maxX = 100f;
    public float minY = -2f;
    public float maxY = 20f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useLimits)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Lisser la transition
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
