using UnityEngine;
using TMPro;

public class TextShake : MonoBehaviour
{
    public float shakeAmount = 0.5f;
    public float shakeSpeed = 20f;
    private TextMeshProUGUI textMesh;
    private Vector3 originalPos;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        originalPos = transform.localPosition;
    }

    void Update()
    {
        // Если мышка наведена на кнопку (или просто всегда)
        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;
        transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
    }
}