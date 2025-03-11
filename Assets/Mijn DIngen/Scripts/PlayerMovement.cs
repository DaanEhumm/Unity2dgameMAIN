using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Playermove : MonoBehaviour
{
    [SerializeField] private float m_speed = 4.0f;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private PlayerInput playerInput;
    private PlayerHealth playerHP;
    public float attackRange = 1.0f;
    private Timer TimerScript;
    private Vector2 moveInput;
    private bool attackPressed;
    private bool m_combatIdle = false;
    private bool canAttack = true;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerHP = GetComponent<PlayerHealth>();
        TimerScript = GetComponent<Timer>();

        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        playerInput.actions["Attack"].performed += ctx => attackPressed = true;
    }

    void Update()
    {
        if (playerHP.currentHealth <= 0)
        {
            moveInput = Vector2.zero;
            m_body2d.linearVelocity = Vector2.zero;
            return;
        }
        if (playerHP.IsPlayerDead())
        {

            if (TimerScript != null)
            {
                TimerScript.enabled = false;
            }
        }

            float inputX = moveInput.x;
            float inputY = moveInput.y;

            if (inputX > 0)
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (inputX < 0)
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            m_body2d.linearVelocity = new Vector2(inputX * m_speed, inputY * m_speed);

            if (attackPressed)
            {
                m_animator.SetTrigger("Attack");
                Attack();
                attackPressed = false;
            }
            else if (Mathf.Abs(inputX) > Mathf.Epsilon || Mathf.Abs(inputY) > Mathf.Epsilon)
            {
                m_animator.SetInteger("AnimState", 2);
            }
            else if (m_combatIdle)
            {
                m_animator.SetInteger("AnimState", 1);
            }
            else
            {
                m_animator.SetInteger("AnimState", 0);
            }
        
    }
    void Attack()
    {
        if (!canAttack) return;

        canAttack = false;

        LayerMask enemyLayer = LayerMask.GetMask("Enemy");  
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        int enemiesHit = 0;  

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemiesHit >= 3)  
                break;

            SkeletonHealth skeletonHealth = enemy.GetComponent<SkeletonHealth>();
            if (skeletonHealth != null)
            {
                skeletonHealth.TakeDamage(50);

                if (skeletonHealth.IsDead())
                {
                    playerHP.Heal(5);  
                }

                enemiesHit++;  
            }
        }

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(1.0f);
        canAttack = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
