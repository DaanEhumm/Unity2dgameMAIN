using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 4f;
    public float attackRange = 0.3f;
    public float stopDistance = 0.5f;
    public float moveSpeed = 1.8f;
    public float attackCooldown = 0.8f;
    public int health = 100;

    public event System.Action OnDeath;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else if (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else
        {
            StopMoving();
        }
    }

    void ChasePlayer()
    {
        if (isDead) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        animator.SetInteger("AnimState", 2);
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetInteger("AnimState", 0);
    }

    void Attack()
    {
        if (isDead || Time.time < lastAttackTime + attackCooldown) return;

        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");

        isAttacking = true;
        lastAttackTime = Time.time;
        Invoke(nameof(ResetAttack), attackCooldown);

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (hit != null)
        {
            hit.GetComponent<PlayerHealth>()?.TakeDamage(15);
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");

        OnDeath?.Invoke();

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        this.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}