using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Движение")]
    public float walkSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Камера")]
    public Transform cameraTransform;
    public float mouseSensitivityX = 2.5f;
    public float mouseSensitivityY = 2.5f;
    public float maxLookAngle = 85f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    // СИНГЛТОН
    public static PlayerMovement Instance { get; private set; }

    // ФЛАГИ БЛОКИРОВКИ
    private bool canMove = true;
    private bool canLook = true;
    private bool canPhysics = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- ВРАЩЕНИЕ (камера) ---
        if (canLook && cameraTransform != null)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        // --- ДВИЖЕНИЕ (WASD) ---
        if (canMove && controller.enabled)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move.normalized * walkSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && controller.isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- ФИЗИКА (Гравитация) ---
        if (canPhysics && controller.enabled)
        {
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    // Полная блокировка (для диалогов/катсцен)
    public void SetMovementState(bool enable)
    {
        canMove = enable;
        canLook = enable;
        canPhysics = enable;
    }

    // Блокировка для сидения (камера работает, ходьба и гравитация - нет)
    public void SetSittingState(bool isSitting)
    {
        canMove = !isSitting;
        canPhysics = !isSitting;
        canLook = true; // Камера всегда работает
    }

    public void SetLookRotation(float pitch)
    {
        xRotation = pitch;
    }

    // Метод для проверки, находится ли игрок в состоянии обычного геймплея
    public bool CanMove()
    {
        return canMove && canLook;
    }
}