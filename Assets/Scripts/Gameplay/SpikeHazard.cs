using UnityEngine;

/// <summary>
/// Gère les obstacles piquants / cristaux / stalagmites.
/// Si le joueur touche l'obstacle, il est repoussé ou réapparaît au dernier checkpoint.
/// </summary>
public class SpikeHazard : MonoBehaviour
{
    [Tooltip("Dégâts ou réapparition")]
    public bool respawnOnTouch = true;
    public Vector3 respawnPoint = new Vector3(0, 1.5f, 0);

    private void OnTriggerEnter(Collider other)
    {
        CheckHit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckHit(collision.gameObject);
    }

    private void CheckHit(GameObject hitObj)
    {
        PlayerController25D player = hitObj.GetComponent<PlayerController25D>();
        if (player != null || hitObj.CompareTag("Player"))
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayHitSound();
            }

            if (respawnOnTouch)
            {
                // Téléporter le joueur au début du niveau / checkpoint en toute sécurité
                CharacterController cc = hitObj.GetComponent<CharacterController>();
                PlayerHealth health = hitObj.GetComponent<PlayerHealth>();
                PlayerController25D playerController = hitObj.GetComponent<PlayerController25D>();
                Vector3 targetPosition = health != null ? health.respawnPoint : respawnPoint;
                if (cc != null) cc.enabled = false;
                hitObj.transform.position = targetPosition;
                if (playerController != null) playerController.ResetMotionAfterRespawn();
                if (cc != null) cc.enabled = true;
            }
        }
    }
}
