using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepSound : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip footstepSound;
    public float stepInterval = 0.5f;
    public float runMultiplier = 0.6f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private AudioSource audioSource;
    private CharacterController characterController;
    private float nextStepTime;
    private bool isPlayingSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            audioSource.loop = false; // Важно: отключаем зацикливание
        }

        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool isMoving = IsGrounded() && characterController.velocity.magnitude > 0.1f;

        // Если игрок остановился — прерываем звук
        if (!isMoving && isPlayingSound)
        {
            audioSource.Stop();
            isPlayingSound = false;
            CancelInvoke(nameof(ResetSoundFlag)); // Отменяем отложенный сброс
        }

        // Если игрок движется — воспроизводим шаги
        if (isMoving)
        {
            float currentStepInterval = Input.GetKey(KeyCode.LeftShift) ? 
                stepInterval * runMultiplier : stepInterval;

            if (Time.time > nextStepTime && !isPlayingSound)
            {
                nextStepTime = Time.time + currentStepInterval;
                PlayFootstep();
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private void PlayFootstep()
    {
        if (footstepSound != null)
        {
            isPlayingSound = true;
            audioSource.PlayOneShot(footstepSound);
            Invoke(nameof(ResetSoundFlag), footstepSound.length);
        }
    }

    private void ResetSoundFlag()
    {
        isPlayingSound = false;
    }
}