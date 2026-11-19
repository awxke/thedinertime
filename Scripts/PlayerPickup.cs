using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Настройки")]
    public float pickupDistance = 3f;
    public float holdDistance = 1.2f;
    public Vector3 holdOffset = new Vector3(0.3f, -0.2f, 0f);

    [Header("Бросок (G)")]
    public float throwForce = 15f;

    private GameObject heldObject = null;
    private Transform playerCamera;

    void Start()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null) TryPickup();
            else DropObject();
        }

        if (Input.GetKeyDown(KeyCode.G) && heldObject != null)
        {
            ThrowObject();
        }

        // Держим предмет перед камерой
        if (heldObject != null)
        {
            Vector3 targetPos = playerCamera.position
                              + playerCamera.forward * holdDistance
                              + playerCamera.right * holdOffset.x
                              + playerCamera.up * holdOffset.y;

            heldObject.transform.position = targetPos;
            heldObject.transform.rotation = playerCamera.rotation;
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.GetComponent<PickupItem>() != null)
            {
                heldObject = hit.collider.gameObject;

                // Отвязываем предмет от родителя
                heldObject.transform.SetParent(null);

                if (heldObject.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
                if (heldObject.TryGetComponent<Collider>(out var col)) col.enabled = false;
            }
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            if (heldObject.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            if (heldObject.TryGetComponent<Collider>(out var col)) col.enabled = true;

            heldObject.transform.position = playerCamera.position + playerCamera.forward * 1f;
            heldObject = null;
        }
    }

    void ThrowObject()
    {
        GameObject objectToThrow = heldObject;
        heldObject = null;

        // Отвязываем на всякий случай
        objectToThrow.transform.SetParent(null);

        if (objectToThrow.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 throwDirection = playerCamera.forward;
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
        }
        if (objectToThrow.TryGetComponent<Collider>(out var col)) col.enabled = true;

        objectToThrow.transform.position = playerCamera.position + playerCamera.forward * 1f;
    }
}