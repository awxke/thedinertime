using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Уличный звук (3D)")]
    public AudioSource worldAudio;
    public AudioClip[] streetAmbientClips;
    public AudioLowPassFilter worldLowPass;

    [Header("Звук погони (2D в ушах)")]
    public AudioSource chaseAudio;
    public AudioClip chaseMusicClip;

    [Header("Настройки перехода")]
    public float fadeDuration = 1.5f;

    private float defaultWorldVolume = 0.6f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (chaseAudio != null) chaseAudio.volume = 0;
        if (worldAudio != null) defaultWorldVolume = worldAudio.volume;
    }

    // --- УПРАВЛЕНИЕ УЛИЦЕЙ ---

    public void PlayStreetAmbient(int index)
    {
        if (worldAudio == null || streetAmbientClips.Length == 0) return;

        worldAudio.clip = streetAmbientClips[index % streetAmbientClips.Length];
        worldAudio.Play();
    }

    public void SetWorldAudioVolume(float targetVolume)
    {
        if (worldAudio != null)
            StartCoroutine(FadeAudio(worldAudio, worldAudio.volume, targetVolume));
    }

    public void SetMuffledWorldAudio(bool isMuffled)
    {
        if (worldLowPass != null)
        {
            worldLowPass.cutoffFrequency = isMuffled ? 500f : 22000f;
        }
    }

    // --- УПРАВЛЕНИЕ ПОГОНЕЙ ---

    public void StartChase()
    {
        if (chaseAudio == null || chaseMusicClip == null)
        {
            Debug.LogError("[AudioManager] ОШИБКА: Не привязан Chase Audio или Chase Music Clip!");
            return;
        }

        chaseAudio.clip = chaseMusicClip;
        chaseAudio.Play();
        StartCoroutine(FadeAudio(chaseAudio, 0f, 1f));

        if (worldAudio != null)
            StartCoroutine(FadeAudio(worldAudio, worldAudio.volume, 0.1f));
    }

    public void StopChase()
    {
        if (chaseAudio == null) return;

        StartCoroutine(FadeAudio(chaseAudio, chaseAudio.volume, 0f));

        if (worldAudio != null)
            StartCoroutine(FadeAudio(worldAudio, worldAudio.volume, defaultWorldVolume));
    }

    // --- ВСПОМОГАТЕЛЬНЫЙ КОРУТИН ---

    private IEnumerator FadeAudio(AudioSource source, float from, float to)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            source.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }
        source.volume = to;

        if (to <= 0.01f) source.Stop();
    }
}