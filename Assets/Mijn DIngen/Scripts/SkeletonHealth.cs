using UnityEngine;
using UnityEngine.UI;

public class SkeletonHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Transform player;
    public GameObject healthBarSkeleton;
    private Slider healthBar;
    public Transform healthBarPosition;
    private Animator animator;
    private bool isDead = false;
    public PlayerHealth playerHealth;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (healthBarSkeleton != null)
        {
            GameObject hb = Instantiate(healthBarSkeleton, FindFirstObjectByType<Canvas>().transform);
            healthBar = hb.GetComponent<Slider>();

            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
            }
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (healthBar != null && healthBarPosition != null)
        {
            healthBar.transform.position = Camera.main.WorldToScreenPoint(healthBarPosition.position);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Death");

        SkeletonAI ai = GetComponent<SkeletonAI>();
        if (ai != null)
        {
            ai.Die();
        }

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        Destroy(gameObject, 3f);
    }
}