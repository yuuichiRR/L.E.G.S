using UnityEngine;
using Cinemachine;

public class FOVAdjuster : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float minFOV = 30f; // Минимальный FOV при максимальном приближении
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Enemy Detection")]
    [SerializeField] private Transform enemy;
    [SerializeField] private LayerMask obstacleLayers; // Слои стен и препятствий
    [SerializeField] private float maxDetectionDistance = 20f; // Макс. расстояние для обнаружения
    [SerializeField] private float maxAngle = 30f; // Угол обзора для обнаружения

    private void Update()
    {
        if (enemy == null || virtualCamera == null) return;

        float targetFOV = defaultFOV;

        // Проверяем, виден ли враг
        if (IsEnemyVisible(out float distanceToEnemy))
        {
            // Чем ближе враг, тем сильнее уменьшаем FOV (плавная зависимость)
            float distanceFactor = Mathf.Clamp01(1 - (distanceToEnemy / maxDetectionDistance));
            targetFOV = Mathf.Lerp(defaultFOV, minFOV, distanceFactor);
        }

        // Плавное изменение FOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.m_Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );
    }

    private bool IsEnemyVisible(out float distance)
    {
        distance = 0f;
    if (enemy == null)
    {
        Debug.Log("Enemy is null!");
        return false;
    }

    Vector3 directionToEnemy = enemy.position - virtualCamera.transform.position;
    distance = directionToEnemy.magnitude;

    if (distance > maxDetectionDistance)
    {
        Debug.Log($"Enemy too far: {distance} > {maxDetectionDistance}");
        return false;
    }

    float angle = Vector3.Angle(virtualCamera.transform.forward, directionToEnemy);
    if (angle > maxAngle)
    {
        Debug.Log($"Enemy outside view angle: {angle} > {maxAngle}");
        return false;
    }

    if (Physics.Raycast(
        virtualCamera.transform.position,
        directionToEnemy,
        out RaycastHit hit,
        maxDetectionDistance,
        obstacleLayers))
    {
        bool isEnemy = hit.transform == enemy;
        Debug.Log($"Raycast hit: {hit.transform.name}. Is enemy? {isEnemy}");
        return isEnemy;
    }

    Debug.Log("Enemy is visible!");
    return true;
    }
}