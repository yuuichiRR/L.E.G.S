using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    public float visionRange = 10f; // Дальность видимости
    public float visionAngle = 90f; // Угол обзора (в градусах)
    public LayerMask obstacleMask; // Слой препятствий

    private NavMeshAgent agent;
    private Vector3 lastKnownPosition;
    private bool playerVisible = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Проверяем видимость игрока
        playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            // Если игрок виден - запоминаем его позицию и двигаемся к нему
            lastKnownPosition = player.position;
            agent.SetDestination(lastKnownPosition);
            agent.isStopped = false;
        }
        else
        {
            // Если игрок не виден - останавливаемся
            agent.isStopped = true;
        }
    }

    bool IsPlayerVisible()
    {
        // Проверяем расстояние до игрока
        if (Vector3.Distance(transform.position, player.position) > visionRange)
        {
            return false;
        }

        // Проверяем угол между направлением врага и направлением к игроку
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > visionAngle / 2f)
        {
            return false;
        }

        // Проверяем, нет ли препятствий между врагом и игроком
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, visionRange, obstacleMask))
        {
            if (hit.transform != player)
            {
                return false;
            }
        }

        return true;
    }

    // Визуализация зоны видимости в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBound = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionRange;
        Vector3 rightBound = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftBound);
        Gizmos.DrawLine(transform.position, transform.position + rightBound);
    }
}