using UnityEngine;

/// <summary>
/// Ennemi simple qui patrouille de gauche à droite en 2.5D.
/// Si le joueur saute au-dessus de l'ennemi -> L'ennemi est éliminé (+200 pts) !
/// Si le joueur le touche latéralement -> Le joueur perd un cœur.
/// </summary>
public class SimpleEnemy : MonoBehaviour
{
    [Header("Patrouille")]
    public float moveSpeed = 3f;
    public float patrolDistance = 4f;

    [Header("Récompense")]
    public int stompScorePoints = 200;

    [Header("Visuel 2D")]
    public bool is2DSprite = true;

    private Vector3 startPos;
    private bool defeated;
    public int direction = 1;
    public bool movingRight => direction > 0;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Déplacement de patrouille horizontal
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime));

        // Inverser la direction au bout de la distance de patrouille
        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
        {
            direction *= -1;
            if (!is2DSprite)
            {
                Vector3 rot = transform.eulerAngles;
                rot.y = direction > 0 ? 0 : 180f;
                transform.eulerAngles = rot;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other.gameObject, true);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckCollision(other.gameObject, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision.gameObject, true);
    }

    private void CheckCollision(GameObject obj, bool applySideDamage)
    {
        if (defeated) return;

        PlayerController25D player = obj.GetComponent<PlayerController25D>();
        if (player == null) player = obj.GetComponentInParent<PlayerController25D>();

        if (player != null)
        {
            CharacterController playerCollider = player.GetComponent<CharacterController>();
            Collider enemyCollider = GetComponent<Collider>();
            if (playerCollider == null || enemyCollider == null) return;

            if (player.IsStomping(enemyCollider))
            {
                defeated = true;
                player.BounceFromStomp();

                if (HUDManager.Instance != null)
                {
                    HUDManager.Instance.AddCoin(0, stompScorePoints);
                }

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayJumpSound();
                }

                Destroy(gameObject);
            }
            else if (applySideDamage)
            {
                // Collision latérale -> Le joueur prend des dégâts
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(1);
                }
            }
        }
    }
}
