using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;
    
    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    
    private bool isDead = false;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else {
                player = GameObject.Find("Player");
                if (player != null) target = player.transform;
            }
        }
    }

    void Update()
    {
        if (isDead || target == null) return;
        
        float distance = Vector3.Distance(transform.position, target.position);
        
        if (distance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            animator.SetBool("attack", true);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetBool("attack", false);
            animator.SetTrigger("move");
        }
    }
    
    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        
        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);
            
        Die();
    }
    
    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("die");
        
        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);
            
        Destroy(gameObject, 3f);
    }
}