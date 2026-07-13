using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHorde : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;
    private int spawnedCount = 0;
    public int maxZombies = 10;

    void Start()
    {
        StartCoroutine(SpawnHorde());
    }

    IEnumerator SpawnHorde()
    {
        while (spawnedCount < maxZombies)
        {
            if (spawnPoints.Length > 0 && zombiePrefab != null)
            {
                int pointIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[pointIndex];
                
                GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
                try { zombie.tag = "Zombie"; } catch { /* tag not defined */ }
                
                var col = zombie.GetComponent<Collider>();
                if (col == null) {
                    var cap = zombie.AddComponent<CapsuleCollider>();
                    cap.height = 1.8f;
                    cap.center = new Vector3(0, 0.9f, 0);
                    cap.radius = 0.3f;
                }
                
                var agent = zombie.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent == null) agent = zombie.AddComponent<UnityEngine.AI.NavMeshAgent>();
                agent.speed = 1.5f;
                agent.angularSpeed = 120f;
                agent.acceleration = 8f;
                agent.stoppingDistance = 1.5f;
                
                // Enable root motion so OnAnimatorMove syncs with NavMeshAgent
                var animator = zombie.GetComponent<Animator>();
                if (animator != null) animator.applyRootMotion = true;
                
                var ai = zombie.GetComponent<EnemyAI>();
                if (ai == null) ai = zombie.AddComponent<EnemyAI>();
                ai.attackRange = 2f;
                ai.damage = 10f;
                ai.attackCooldown = 1.5f;
                
                // Add DestruirCajas so it can be killed by the shotgun
                var salud = zombie.GetComponent<DestruirCajas>();
                if (salud == null) salud = zombie.AddComponent<DestruirCajas>();
                salud.saludActual = 2; // 2 shots to kill
                
                spawnedCount++;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
