using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthbar;
    private Animator animator;
    public Text gameovertext;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthbar != null)
        {
            healthbar.maxValue = 1;
            healthbar.value = (float)currentHealth / maxHealth;
        }
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthbar != null)
        {
            float normalizedHealth = (float)currentHealth / maxHealth;
            healthbar.value = normalizedHealth;
        }

        Debug.Log("Speler HP: " + currentHealth);

        animator.SetTrigger("Hurt");

        StartCoroutine(ResetToIdle());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetInteger("AnimState", 0);
    }

    void Die()
    {
        Debug.Log("Speler is dood!");
        animator.SetTrigger("Death");
        gameovertext.gameObject.SetActive(true);


        Invoke("ReturnToStartScreen", 4f);

    }
    private void ReturnToStartScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float normalizedHealth = (float)currentHealth / maxHealth;
        healthbar.value = normalizedHealth;
    }
    public bool IsPlayerDead()
    {
        return currentHealth <= 0;
    }
}