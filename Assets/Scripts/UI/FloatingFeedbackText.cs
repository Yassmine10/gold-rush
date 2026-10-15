using UnityEngine;
using TMPro;

/// <summary>
/// Anime le texte flottant "+10" vers le haut avec fondu de transparence.
/// </summary>
public class FloatingFeedbackText : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float floatSpeed = 2f;
    public float duration = 0.8f;
    public Vector3 scaleMultiplier = new Vector3(1.2f, 1.2f, 1.2f);

    private float timer = 0f;
    private Color startColor;

    void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }
        if (textMesh != null)
        {
            startColor = textMesh.color;
        }
    }

    public void SetText(string content)
    {
        if (textMesh != null)
        {
            textMesh.text = content;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Déplacement vers le haut
        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);

        // Disparition progressive (Fade out)
        if (textMesh != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
