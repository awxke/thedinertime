using UnityEngine;

public class ZoneAudioTrigger : MonoBehaviour
{
    [Tooltip("Поставь галочку, если этот триггер находится ВНУТРИ кафе")]
    public bool isInsideCafe = false;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что зашел именно игрок
        if (other.CompareTag("Player"))
        {
            if (AudioManager.Instance != null)
            {
                if (isInsideCafe)
                {
                    // Зашли в кафе — звук улицы становится тихим (0.05) и глухим
                    AudioManager.Instance.SetWorldAudioVolume(0.05f);
                    AudioManager.Instance.SetMuffledWorldAudio(true);
                }
                else
                {
                    // Вышли на улицу — звук улицы становится громким (0.6) и чистым
                    AudioManager.Instance.SetWorldAudioVolume(0.6f);
                    AudioManager.Instance.SetMuffledWorldAudio(false);
                }
            }
        }
    }
}