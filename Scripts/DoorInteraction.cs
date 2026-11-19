using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Настройки двери")]
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public float interactionDistance = 3f;

    private bool isOpen = false;
    private float closedYRotation;
    private float targetYRotation;
    private Transform playerCamera;

    void Start()
    {
        // Запоминаем начальный поворот по Y
        closedYRotation = transform.localEulerAngles.y;
        targetYRotation = closedYRotation;

        if (Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // Вращаем только по Y, жестко фиксируя X и Z (чтобы дверь не ложилась)
        float currentY = Mathf.LerpAngle(transform.localEulerAngles.y, targetYRotation, Time.deltaTime * openSpeed);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentY, transform.localEulerAngles.z);

        // Взаимодействие
        if (Input.GetKeyDown(KeyCode.E) && playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isOpen = !isOpen;

                    if (isOpen)
                    {
                        // Открываем "от себя"
                        Vector3 directionToPlayer = (playerCamera.position - transform.position).normalized;
                        float dot = Vector3.Dot(transform.forward, directionToPlayer);

                        if (dot > 0)
                            targetYRotation = closedYRotation - openAngle;
                        else
                            targetYRotation = closedYRotation + openAngle;
                    }
                    else
                    {
                        targetYRotation = closedYRotation;
                    }
                }
            }
        }
    }
}