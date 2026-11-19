using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Настройки")]
    public string requiredKeyID = "KitchenKey";
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactionDistance = 3f;

    private bool isOpen = false;
    private float closedYRotation;
    private float targetYRotation;
    private Transform playerCamera;

    void Start()
    {
        closedYRotation = transform.localEulerAngles.y;
        targetYRotation = closedYRotation;
        if (Camera.main != null) playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // Вращение
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
                    if (KeyManager.HasKey(requiredKeyID))
                    {
                        isOpen = !isOpen;
                        if (isOpen)
                            targetYRotation = closedYRotation - openAngle;
                        else
                            targetYRotation = closedYRotation;
                    }
                    else
                    {
                        Debug.Log("Дверь заперта! Нужен ключ: " + requiredKeyID);
                        // Тут можно проиграть звук "заперто"
                    }
                }
            }
        }
    }
}