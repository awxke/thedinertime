using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Ссылки")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public GameObject choiceButtonsContainer;
    public Button buttonPrefab;

    [Header("Настройки печати")]
    public float typingSpeed = 0.05f;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForChoice = false;
    private System.Action onDialogueEnd;

    void Start()
    {
        dialogPanel.SetActive(false);
        choiceButtonsContainer.SetActive(false);
    }

    public void StartDialogue(string[] lines, System.Action onEnd = null)
    {
        currentLines = lines;
        currentLineIndex = 0;
        onDialogueEnd = onEnd;

        dialogPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Блокируем движение через синглтон
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = false;
        }

        ShowLine();
    }

    void ShowLine()
    {
        if (currentLineIndex < currentLines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine(currentLines[currentLineIndex]));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && dialogPanel.activeSelf && !isWaitingForChoice)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = currentLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                ShowLine();
            }
        }
    }

    void EndDialogue()
    {
        dialogPanel.SetActive(false);
        choiceButtonsContainer.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Возвращаем управление
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = true;
        }

        onDialogueEnd?.Invoke();
    }

    public void ShowFoodChoice(System.Action<string> onFoodChosen)
    {
        isWaitingForChoice = true;
        dialogPanel.SetActive(true);
        dialogText.text = "What would you like to order?";
        choiceButtonsContainer.SetActive(true);

        foreach (Transform child in choiceButtonsContainer.transform)
            Destroy(child.gameObject);

        string[] foods = { "Burger", "Ramen", "Pizza" };

        foreach (string food in foods)
        {
            Button newBtn = Instantiate(buttonPrefab, choiceButtonsContainer.transform);
            newBtn.GetComponentInChildren<TextMeshProUGUI>().text = food;

            newBtn.onClick.AddListener(() => {
                choiceButtonsContainer.SetActive(false);
                isWaitingForChoice = false;
                onFoodChosen(food);
            });
        }
    }
}