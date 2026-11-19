using UnityEngine;
using System.Collections;

public class WaiterDialog : MonoBehaviour
{
    [Header("UI Ссылки")]
    public DialogueSystem dialogueSystem;
    public Transform playerCamera;

    [Header("Точки телепортации")]
    public Transform kitchenPoint;
    public Transform tablePoint;

    [Header("Предметы")]
    public GameObject platePrefab;     // Пустая тарелка
    public GameObject burgerPrefab;    // Сам бургер
    public Transform kitchenBurgerPoint; // Где лежит бургер на кухне
    public Transform handPoint;        // Точка в руках официантки

    [Header("Настройки")]
    public float interactionDistance = 3f;

    private bool hasTriggered = false;
    private GameObject kitchenBurger;
    private GameObject handPlate;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        // Спавним бургер на кухне
        if (burgerPrefab != null && kitchenBurgerPoint != null)
        {
            kitchenBurger = Instantiate(burgerPrefab, kitchenBurgerPoint.position, kitchenBurgerPoint.rotation);
        }
    }

    void Update()
    {
        if (PlayerMovement.Instance == null || !PlayerMovement.Instance.gameObject.activeInHierarchy) return;

        if (!hasTriggered && Input.GetKeyDown(KeyCode.E) && playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
                {
                    hasTriggered = true;
                    StartDialogue();
                }
            }
        }
    }

    void StartDialogue()
    {
        string[] introLines = {
            "Oh, a new face.",
            "Please, take a seat at the table over there.",
            "One burger coming right up."
        };

        dialogueSystem.StartDialogue(introLines, () => {
            StartCoroutine(PrepareAndServeBurger());
        });
    }

    IEnumerator PrepareAndServeBurger()
    {
        // 1. Телепорт на кухню (отключаем аниматор, чтобы он не мешал)
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        transform.position = kitchenPoint.position;
        transform.rotation = kitchenPoint.rotation;

        yield return new WaitForSeconds(1f);

        // 2. Удаляем бургер с кухни
        if (kitchenBurger != null) Destroy(kitchenBurger);

        // 3. Спавним тарелку в руках
        if (platePrefab != null && handPoint != null)
        {
            handPlate = Instantiate(platePrefab, handPoint.position, handPoint.rotation, handPoint);
        }

        yield return new WaitForSeconds(1f);

        // 4. Телепорт к столу
        transform.position = tablePoint.position;
        transform.rotation = tablePoint.rotation;

        // 5. Удаляем тарелку из рук
        if (handPlate != null) Destroy(handPlate);

        // 6. Спавним финальную тарелку на столе
        GameObject finalPlate = null;
        if (platePrefab != null)
        {
            finalPlate = Instantiate(platePrefab, tablePoint.position, tablePoint.rotation);
        }

        // 7. Спавним бургер И ПРИВЯЗЫВАЕМ его к тарелке
        if (burgerPrefab != null && finalPlate != null)
        {
            // Создаем бургер внутри тарелки (SetParent делает его дочерним)
            GameObject finalBurger = Instantiate(burgerPrefab, finalPlate.transform);

            // Центрируем бургер на тарелке 
            finalBurger.transform.localPosition = new Vector3(0, 0.1f, 0);
        }

        // 8. Финальная фраза
        string[] serveLines = { "Here is your burger. Enjoy!" };
        dialogueSystem.StartDialogue(serveLines, () => {
            transform.position = startPosition;
            transform.rotation = startRotation;
            if (anim != null) anim.enabled = true;

            hasTriggered = false;
        });
    }
}