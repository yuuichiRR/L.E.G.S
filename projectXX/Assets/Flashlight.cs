using UnityEngine;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    public Light flashLight;
    public float flashDuration = 0.2f;
    public float flashIntensity = 100f;
    public float flashAngle = 45f;
    public float flashRange = 15f;
    public KeyCode flashKey = KeyCode.F;
    public float cooldownTime = 10f; // Время перезарядки
    
    private float defaultIntensity;
    private bool isFlashing = false;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    
    void Start()
    {
        flashLight = GetComponentInChildren<Light>();
        if(flashLight == null) Debug.LogError("No Light component found!");
        defaultIntensity = flashLight.intensity;
        flashLight.enabled = false;
    }
    
    void Update()
    {
        // Обновляем таймер перезарядки
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
            }
        }
        
        // Проверяем можно ли активировать вспышку
        if (Input.GetKeyDown(flashKey))
        {
            TryFlash();
        }
    }
    
    private void TryFlash()
    {
        if (!isFlashing && !isOnCooldown)
        {
            StartCoroutine(Flash());
        }
    }
    
    private IEnumerator Flash()
    {
        // Активируем вспышку
        isFlashing = true;
        flashLight.enabled = true;
        flashLight.intensity = flashIntensity;
        
        yield return new WaitForSeconds(flashDuration);
        
        // Деактивируем вспышку
        flashLight.intensity = defaultIntensity;
        flashLight.enabled = false;
        isFlashing = false;
        
        // Начинаем перезарядку
        StartCooldown();
    }
    
    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }
    
    public bool IsFlashing()
    {
        return isFlashing;
    }
    
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
    
    public float GetCooldownProgress()
    {
        return 1f - (cooldownTimer / cooldownTime);
    }
}