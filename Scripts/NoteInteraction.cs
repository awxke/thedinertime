using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NoteInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject notePanel;
    public TextMeshProUGUI noteText;
    public string noteContent = "Day 14: The old man keeps dancing. He says the ritual must continue. The freezer is full again. I can't do this anymore.";

    [Header("Настройки")]
    public float interactionDistance = 3f;
    public Transform playerCamera;

    private bool isOpen = false;

    void Start()
    {
        notePanel.SetActive(false);
        if (playerCamera == null && Camera.main != null) playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOpen)
            {
                CloseNote();
            }
            else
            {
                TryInteract();
            }
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                OpenNote();
            }
        }
    }

    void OpenNote()
    {
        isOpen = true;
        notePanel.SetActive(true);
        noteText.text = noteContent;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameObject.FindWithTag("Player").TryGetComponent<PlayerMovement>(out var pm))
            pm.enabled = false;
    }

    void CloseNote()
    {
        isOpen = false;
        notePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameObject.FindWithTag("Player").TryGetComponent<PlayerMovement>(out var pm))
            pm.enabled = true;
    }
}