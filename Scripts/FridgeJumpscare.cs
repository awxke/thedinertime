using UnityEngine;
using System.Collections;
using TMPro;

public class FridgeJumpscare : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform mainCamera;
    public Transform lookAtTarget;
    public AudioSource scareSound;
    public AudioSource heartbeatSound;
    public TextMeshProUGUI subtitleText;
    public Transform player;

    [Header("Маньяк")]
    public KillerChase killerScript;

    [Header("Настройки")]
    public string dialogueLine = "What the hell...? What is going on here?";
    public float scareDelay = 0f;
    public float soundBuildUp = 1f;
    public float lockDuration = 1.5f;
    public float subtitleDuration = 3f;

    private bool hasTriggered = false;

    void Start()
    {
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);
        if (heartbeatSound != null) heartbeatSound.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayScare());
        }
    }

    IEnumerator PlayScare()
    {
        Debug.Log("1. Триггер трупа сработал!");

        yield return new WaitForSeconds(scareDelay);

        if (scareSound != null) scareSound.Play();
        yield return new WaitForSeconds(soundBuildUp);

        if (player.TryGetComponent<PlayerMovement>(out var pm)) pm.enabled = false;

        if (mainCamera != null && lookAtTarget != null)
        {
            mainCamera.LookAt(lookAtTarget);
        }

        yield return new WaitForSeconds(lockDuration);

        if (player.TryGetComponent<PlayerMovement>(out var pmScript))
        {
            Vector3 cameraEuler = mainCamera.eulerAngles;
            player.rotation = Quaternion.Euler(0, cameraEuler.y, 0);

            float pitch = cameraEuler.x;
            if (pitch > 180f) pitch -= 360f;

            pmScript.SetLookRotation(pitch);
            pmScript.enabled = true;
        }

        if (heartbeatSound != null) heartbeatSound.Play();
        if (subtitleText != null)
        {
            subtitleText.gameObject.SetActive(true);
            subtitleText.text = dialogueLine;
        }

        yield return new WaitForSeconds(subtitleDuration);

        if (subtitleText != null) subtitleText.gameObject.SetActive(false);

        // --- 8. ПОЯВЛЕНИЕ МАНЬЯКА ---
        Debug.Log("2. Пытаемся вызвать маньяка...");

        if (killerScript != null)
        {
            Debug.Log("3. Маньяк найден! Вызываем TeleportAndWait.");
            killerScript.TeleportAndWait();
        }
        else
        {
            Debug.LogError("ОШИБКА: Поле Killer Script пустое в инспекторе!");
        }
    }
}