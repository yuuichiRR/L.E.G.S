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
    public float minRetreatDistance = 5f; // Минимальная дистанция для отбегания
    
    private bool isRetreating = false;
    private Vector3 retreatTarget;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
    }

    void Update()
    {
        if (player == null) return;
        
        // Если моб в режиме отступления - ничего не делаем
        if (isRetreating) return;
        
        // Обычное преследование игрока
        agent.SetDestination(player.position);
        
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
        
        // Вычисляем точку отступления
        Vector3 directionFromPlayer = (transform.position - player.position).normalized;
        retreatTarget = transform.position + directionFromPlayer * minRetreatDistance;
        
        // Находим ближайшую точку на NavMesh
        if (NavMesh.SamplePosition(retreatTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            retreatTarget = hit.position;
            agent.SetDestination(retreatTarget);
        }
        
        // Ждём, пока моб не убежит достаточно далеко или не истечёт время
        float startTime = Time.time;
        while (Time.time - startTime < retreatDuration && 
               Vector3.Distance(transform.position, player.position) < flashReactionDistance * 1.5f)
        {
            yield return null;
        }
        
        agent.speed = normalSpeed;
        isRetreating = false;
    }

    void OnDrawGizmos()
    {
        if (isRetreating)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(retreatTarget, 0.5f);
            Gizmos.DrawLine(transform.position, retreatTarget);
        }
    }
}