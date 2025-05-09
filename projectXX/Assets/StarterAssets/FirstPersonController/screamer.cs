using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class ScrimerMobTeleport : MonoBehaviour
{
    [Header("Настройки")]
    public float distanceFromCamera = 0.5f;
    public float reloadDelay = 1.5f;
    public float cameraLockDuration = 1.2f;

    [Header("Эффекты")]
    public ParticleSystem teleportEffect;
    public AudioClip screamSound;

    private CinemachineVirtualCamera virtualCamera;
    private bool isCameraLocked = false;

    private void Start()
    {
        // Ищем камеру по тегу или имени (у вас она называется "PlayerFollowCamera")
        virtualCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera не найдена! Убедитесь, что она есть в сцене и называется 'PlayerFollowCamera'");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCameraLocked)
        {
            TeleportMobToCamera();
            LockCameraOnMonster();
            Invoke("ReloadLevel", reloadDelay);
        }
    }

    private void TeleportMobToCamera()
    {
        if (virtualCamera != null)
        {
            Vector3 cameraForward = virtualCamera.transform.forward;
            Vector3 newPosition = virtualCamera.transform.position + cameraForward * distanceFromCamera;
            transform.position = newPosition;
            transform.LookAt(virtualCamera.transform);

            if (teleportEffect != null)
                Instantiate(teleportEffect, transform.position, Quaternion.identity);

            if (screamSound != null)
                AudioSource.PlayClipAtPoint(screamSound, transform.position);
        }
    }

    private void LockCameraOnMonster()
    {
        if (virtualCamera == null) return;

        isCameraLocked = true;
        
        // 1. Сохраняем оригинальный Follow (чтобы потом вернуть)
        Transform originalFollow = virtualCamera.Follow;
        
        // 2. Заставляем камеру смотреть на монстра
        virtualCamera.LookAt = transform;
        
        // 3. Возвращаем камеру через cameraLockDuration секунд
        Invoke("UnlockCamera", cameraLockDuration);
    }

    private void UnlockCamera()
    {
        if (virtualCamera == null) return;
        
        virtualCamera.LookAt = null; // Сбрасываем LookAt
        isCameraLocked = false;
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}