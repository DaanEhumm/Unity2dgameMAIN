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

    private int skeletonsKilled = 0; 
    private bool hasPowerup = false;
    private float powerupDuration = 10f; 
    private float powerupTimer;
    public Playermove playerMove;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthbar != null)
        {
            healthbar.maxValue = 1;
            healthbar.value = (float)currentHealth / maxHealth;
        }
        animator = GetComponent<Animator>();
        playerMove = GetComponent<Playermove>();
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

    public void KillSkeleton()
    {
        if (hasPowerup) return;
        skeletonsKilled++;

        if (skeletonsKilled >= 20 && !hasPowerup)
        {
            ActivatePowerup();
        }
    }

    private void ActivatePowerup()
    {
        hasPowerup = true;
        powerupTimer = powerupDuration;

        transform.localScale = new Vector3(2f, 2f, 1f);
        playerMove.attackRange = 5f;
        skeletonsKilled = 0;

        StartCoroutine(PowerupCountdown());
    }

    private IEnumerator PowerupCountdown()
    {
        while (powerupTimer > 0)
        {
            powerupTimer -= Time.deltaTime;
            yield return null;
        }
        DeactivatePowerup();
    }

    private void DeactivatePowerup()
    {
        if (hasPowerup)
        {
            hasPowerup = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            playerMove.attackRange = 1f;
            skeletonsKilled = 0;
        }
    }

    public bool HasPowerup()
    {
        return hasPowerup;
    }
}