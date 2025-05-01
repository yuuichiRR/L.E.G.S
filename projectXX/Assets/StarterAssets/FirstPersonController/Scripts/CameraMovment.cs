using UnityEngine;
using Cinemachine;

public class CameraBobbing : MonoBehaviour
{
    [Header("Настройки качания")]
    public float walkNoise = 0.1f;  // Сила качания при ходьбе
    public float runNoise = 0.3f;   // Сила качания при беге
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Ссылки")]
    public CinemachineVirtualCamera vcam;  // Виртуальная камера
    private CinemachineBasicMultiChannelPerlin noise;  // Компонент шума

    void Start()
    {
        if (vcam != null)
            noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (noise == null) return;

        // Проверяем движение (можно заменить на свой способ)
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool isRunning = isMoving && Input.GetKey(runKey);

        // Включаем/выключаем качание
        noise.m_AmplitudeGain = isMoving ? (isRunning ? runNoise : walkNoise) : 0f;
    }
}