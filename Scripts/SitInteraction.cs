using UnityEngine;
using System.Collections;

public class SitInteraction : MonoBehaviour
{
    [Header("Настройки сидения")]
    public Transform sitPoint;
    public float interactionDistance = 2f;

    [Tooltip("Высота глаз сидящего")]
    public float seatedEyeHeight = 1.0f;

    [Tooltip("Высота глаз стоя (стандартная для твоего игрока)")]
    public float standingEyeHeight = 0.6f; // рост игрока

    private bool isSitting = false;
    private PlayerMovement playerScript;
    private CharacterController controller;
    private Transform playerCamera;

    private Vector3 savedStandPosition;
    private Quaternion originalPlayerRotation;

    void Start()
    {
        playerScript = PlayerMovement.Instance;

        if (playerScript == null)
        {
            Debug.LogError($"[SitInteraction] Игрок не найден!");
            enabled = false;
            return;
        }

        controller = playerScript.GetComponent<CharacterController>();

        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        if (playerScript == null || !playerScript.gameObject.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isSitting) StandUp();
            else if (Vector3.Distance(playerScript.transform.position, transform.position) < interactionDistance) SitDown();
        }
    }

    void SitDown()
    {
        isSitting = true;

        savedStandPosition = playerScript.transform.position;
        originalPlayerRotation = playerScript.transform.rotation;

        playerScript.SetSittingState(true);
        controller.enabled = false;

        Vector3 sitEuler = sitPoint.eulerAngles;
        playerScript.transform.rotation = Quaternion.Euler(0f, sitEuler.y, 0f);
        playerScript.transform.position = sitPoint.position;

        // Настраиваем камеру локально
        if (playerCamera != null)
        {
            playerCamera.localPosition = new Vector3(0, seatedEyeHeight, 0);
            playerCamera.localRotation = Quaternion.identity;
        }

        playerScript.SetLookRotation(0f);
    }

    void StandUp()
    {
        isSitting = false;
        StartCoroutine(TeleportAndEnableController());
    }

    IEnumerator TeleportAndEnableController()
    {
        playerScript.transform.position = savedStandPosition;
        playerScript.transform.rotation = originalPlayerRotation;

        if (playerCamera != null)
        {
            // Возвращаем камере её исходную локальную высоту
            playerCamera.localPosition = new Vector3(0, standingEyeHeight, 0);
            playerCamera.localRotation = Quaternion.identity;
        }

        playerScript.SetSittingState(false);
        playerScript.SetLookRotation(0f);

        yield return null;

        controller.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}