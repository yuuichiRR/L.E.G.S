using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public Flashlight playerFlashlight;
    
    [Header("Movement Settings")]
    public float normalSpeed = 3.5f;
    public float retreatSpeed = 8f;
    
    [Header("Flash Reaction")]
    public float flashReactionDistance = 10f;
    public float retreatDuration = 2f;
    
    private bool isRetreating = false;
    private Vector3 retreatDirection;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
    }

    void Update()
    {
        if (isRetreating) return;
        
        // Простое преследование игрока
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
        
        // Реакция на фонарик
        if (CheckFlashlightHit())
        {
            StartCoroutine(Retreat());
        }
    }

    bool CheckFlashlightHit()
    {
        if (playerFlashlight == null || !playerFlashlight.IsFlashing()) 
            return false;
            
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= flashReactionDistance;
    }

    System.Collections.IEnumerator Retreat()
    {
        isRetreating = true;
        agent.speed = retreatSpeed;
        
        // Отбегаем в противоположном направлении
        retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDirection * 5f;
        
        agent.SetDestination(retreatTarget);
        
        yield return new WaitForSeconds(retreatDuration);
        
        agent.speed = normalSpeed;
        isRetreating = false;
    }

    void OnDrawGizmos()
    {
        if (isRetreating)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + retreatDirection * 3f);
        }
    }
}