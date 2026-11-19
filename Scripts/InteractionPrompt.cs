using UnityEngine;
using UnityEngine.UI;
using TMPro; // Если используешь TextMeshPro

public class InteractionUI : MonoBehaviour
{
    [Header("UI Ссылки")]
    public Transform playerCamera;
    public GameObject crosshairObject; // прицел
    public GameObject interactionText; // Текст "Press E"

    [Header("Настройки")]
    public float interactDistance = 3f;
    public LayerMask interactLayer; // Слой, на котором лежат предметы

    void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        // Скрываем текст при старте
        if (interactionText != null) interactionText.SetActive(false);
    }

    void Update()
    {
        // 1. Показываем/скрываем прицел в зависимости от состояния игрока
        if (PlayerMovement.Instance != null && crosshairObject != null)
        {
            crosshairObject.SetActive(PlayerMovement.Instance.CanMove());
        }

        // 2. Проверяем, на что смотрит игрок
        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            // Пускаем луч
            if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
            {
                // Проверяем, есть ли на объекте тег "Interactable" 
                // ИЛИ скрипт PickupItem / WaiterDialog / SitInteraction
                bool isInteractable = hit.collider.CompareTag("Interactable") ||
                                      hit.collider.GetComponent<PickupItem>() != null ||
                                      hit.collider.GetComponent<WaiterDialog>() != null ||
                                      hit.collider.GetComponent<SitInteraction>() != null;

                if (isInteractable)
                {
                    // Показываем текст
                    if (interactionText != null) interactionText.SetActive(true);

                }
                else
                {
                    // Если смотрим на стену — скрываем
                    if (interactionText != null) interactionText.SetActive(false);
                }
            }
            else
            {
                // Если луч вообще ни во что не попал (смотрим в небо)
                if (interactionText != null) interactionText.SetActive(false);
            }
        }
    }
}