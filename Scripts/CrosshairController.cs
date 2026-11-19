using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // CanvasGroup позволяет плавно скрывать элемент и полностью отключать его взаимодействие
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        // Если игрок существует и находится в режиме обычного геймплея
        if (PlayerMovement.Instance != null && PlayerMovement.Instance.CanMove())
        {
            canvasGroup.alpha = 1f; // Показываем
        }
        else
        {
            canvasGroup.alpha = 0f; // Скрываем (исчезает во время катсцен и диалогов)
        }
    }
}