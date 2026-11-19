using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Громкость")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    [Header("Чувствительность")]
    public Slider sensSlider;
    public TextMeshProUGUI sensText;

    void Start()
    {
        // Защита: если слайдеры не привязаны, скрипт не будет ломать игру
        if (volumeSlider == null || sensSlider == null)
        {
            Debug.LogError("[SettingsManager] ОШИБКА: Слайдеры не привязаны в инспекторе!");
            return;
        }

        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.8f);
        float savedSens = PlayerPrefs.GetFloat("Sensitivity", 2.5f);

        volumeSlider.value = savedVolume;
        sensSlider.value = savedSens;

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        sensSlider.onValueChanged.AddListener(UpdateSensitivity);

        UpdateVolume(savedVolume);
        UpdateSensitivity(savedSens);
    }

    public void UpdateVolume(float value)
    {
        if (volumeText != null) volumeText.text = Mathf.RoundToInt(value * 100).ToString();
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void UpdateSensitivity(float value)
    {
        if (sensText != null) sensText.text = value.ToString("F1");
        PlayerPrefs.SetFloat("Sensitivity", value);
    }
}