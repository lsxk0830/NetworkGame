using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    [Header("破坏设置")]
    public int maxHealth = 3;
    public GameObject explosionEffect;

    private int currentHealth;
    private Material originalMaterial;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        originalMaterial = GetComponent<Renderer>().material;
        originalColor = originalMaterial.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        originalMaterial.color = Color.Lerp(Color.red, originalColor, currentHealth/(float)maxHealth);

        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}