using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float attackRange = 2f;
    public float damage = 10f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = 0f;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (player == null)
        {
            var pObj = GameObject.Find("Player") ?? GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;
        if (agent == null || !agent.isOnNavMesh) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // Attack
            agent.isStopped = true;
            if (animator != null) {
                animator.SetBool("attack", true);
                animator.SetBool("move", false);
                animator.SetBool("idle", false);
            }

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Deal damage
                player.SendMessage("perderVida", (int)damage, SendMessageOptions.DontRequireReceiver);
                if (GameManager.Instance != null) {
                    GameManager.Instance.TakeDamage((int)damage);
                }
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Chase
            agent.isStopped = false;
            agent.SetDestination(player.position);
            
            if (animator != null) {
                animator.SetBool("move", true);
                animator.SetBool("attack", false);
                animator.SetBool("idle", false);
            }
        }
    }

    void OnAnimatorMove()
    {
        if (animator != null && agent != null && agent.enabled && !agent.isStopped && Time.deltaTime > 0)
        {
            // Update agent velocity from root motion delta, but keep NavMeshAgent controlling Y (gravity/snap)
            Vector3 v = (animator.deltaPosition) / Time.deltaTime;
            v.y = agent.velocity.y; // preserve NavMesh gravity
            agent.velocity = v;
            
            // Allow animation to rotate it too if needed, though agent rotation is usually fine
            // transform.rotation = animator.rootRotation; 
        }
    }

    // Called by Animation Event
    public void AttackHitEvent()
    {
        // We can do damage here instead of in Update if we want, or just leave it empty to suppress the error
    }

    public void Hit()
    {
    }

    // Called when the enemy's health reaches 0 (e.g. from DestruirCajas or similar)
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayZombieDeathSFX();

        agent.isStopped = true;
        agent.enabled = false;
        
        if (animator != null) {
            animator.SetBool("die", true);
            animator.SetBool("move", false);
            animator.SetBool("attack", false);
            animator.SetBool("idle", false);
        }

        if (GameManager.Instance != null) {
            GameManager.Instance.AddKill();
            GameManager.Instance.AddPoints(10);
        }

        // Disable colliders so it doesn't block player
        var cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = false;
        
        Destroy(gameObject, 5f);
    }
    
    // Fallback if Weapon.cs uses SendMessage("perderVida") instead of ApplyDamage
    public void perderVida(int dmg)
    {
        var salud = GetComponent<DestruirCajas>();
        if (salud != null) {
            salud.Da\u00f1o(dmg);
        } else {
            Die();
        }
    }
}
