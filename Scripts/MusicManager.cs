using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Аудио Источники")]
    public AudioSource ambientSource; // Для фонового эмбиента (норм музыка)
    public AudioSource chaseSource;   // Для погони (тревожная музыка)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartChaseMusic(AudioClip chaseClip)
    {
        if (ambientSource != null) ambientSource.Stop(); // Выключаем спокойную

        if (chaseSource != null)
        {
            chaseSource.clip = chaseClip;
            chaseSource.loop = true;
            chaseSource.Play();
        }
    }

    public void StopChaseMusic()
    {
        if (chaseSource != null) chaseSource.Stop();

        if (ambientSource != null) ambientSource.Play(); // Возвращаем спокойную
    }
}