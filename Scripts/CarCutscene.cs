using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarCutscene : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform car;
    public Transform cameraTransform;
    public Transform[] waypoints;
    public Transform cameraLookAt;
    public Transform player;
    public GameObject dancingOldMan;
    public Transform rooftopCameraPoint;

    [Header("UI")]
    public GameObject hudCanvas;

    [Header("Настройки")]
    public float speed = 10f;
    public float turnSpeed = 5f;
    public float cameraDistance = 5f;
    public float carHeightOffset = 0f;
    public int rooftopSwitchIndex = 2;

    [Header("Камера")]
    public float cameraHeight = 1.5f;
    public float playerEyeHeight = 0.6f;

    [Header("Финальная парковка")]
    public float finalParkYOffset = 0f;
    public float finalParkExtraRotation = 180f;

    [Header("Затемнение")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    private AudioSource oldManMusic;
    private AudioSource carEngineAudio;
    private bool isSkipped = false;

    void Start()
    {
        // ВЫКЛЮЧАЕМ HUD на время катсцены
        if (hudCanvas != null) hudCanvas.SetActive(false);

        cameraTransform.SetParent(null);
        if (player != null) player.gameObject.SetActive(false);

        if (dancingOldMan != null)
        {
            oldManMusic = dancingOldMan.GetComponent<AudioSource>();
            if (oldManMusic != null)
            {
                oldManMusic.Stop();
                oldManMusic.loop = false; // Отключаем зацикливание, чтобы музыка играла только 10 секунд
            }
        }

        if (car != null)
        {
            carEngineAudio = car.GetComponent<AudioSource>();
        }

        if (waypoints.Length > 0)
        {
            car.position = new Vector3(waypoints[0].position.x, waypoints[0].position.y + carHeightOffset, waypoints[0].position.z);

            if (waypoints.Length > 1)
            {
                Vector3 startDirection = (waypoints[1].position - car.position).normalized;
                startDirection.y = 0;
                car.rotation = Quaternion.LookRotation(startDirection);
            }
            else
            {
                car.rotation = waypoints[0].rotation;
            }
        }

        Vector3 startCamPos = car.position - car.forward * cameraDistance + car.up * cameraHeight;
        cameraTransform.position = startCamPos;
        cameraTransform.LookAt(cameraLookAt);

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(true);
        }

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(1f);

        bool cameraSwitched = false;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform targetPoint = waypoints[i];

            while (Vector3.Distance(car.position, targetPoint.position) > 0.5f)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    isSkipped = true;
                    break;
                }

                Vector3 targetPos = new Vector3(targetPoint.position.x, targetPoint.position.y + carHeightOffset, targetPoint.position.z);
                car.position = Vector3.MoveTowards(car.position, targetPos, speed * Time.deltaTime);

                Vector3 direction = (targetPos - car.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    car.rotation = Quaternion.Slerp(car.rotation, targetRotation, turnSpeed * Time.deltaTime);
                }

                if (!cameraSwitched)
                {
                    Vector3 cameraTargetPos = car.position - car.forward * cameraDistance + car.up * cameraHeight;
                    cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTargetPos, Time.deltaTime * 5f);
                    cameraTransform.LookAt(cameraLookAt);
                }

                yield return null;
            }

            if (isSkipped) break;

            if (i == rooftopSwitchIndex && rooftopCameraPoint != null)
            {
                cameraTransform.position = rooftopCameraPoint.position;
                cameraTransform.rotation = rooftopCameraPoint.rotation;
                cameraSwitched = true;
            }
        }

        if (waypoints.Length > 0)
        {
            Transform lastPoint = waypoints[waypoints.Length - 1];
            car.position = new Vector3(lastPoint.position.x, lastPoint.position.y + finalParkYOffset, lastPoint.position.z);
            car.rotation = lastPoint.rotation * Quaternion.Euler(0, finalParkExtraRotation, 0);
        }

        if (isSkipped)
        {
            Debug.Log("Катсцена скипнута!");
            if (carEngineAudio != null) carEngineAudio.Stop();
        }

        if (!cameraSwitched && rooftopCameraPoint != null)
        {
            cameraTransform.position = rooftopCameraPoint.position;
            cameraTransform.rotation = rooftopCameraPoint.rotation;
        }

        yield return new WaitForSeconds(isSkipped ? 0.5f : 3f);

        if (fadeImage != null)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
        }

        if (player != null)
        {
            Vector3 playerDropPos = car.position + car.right * 2f;
            RaycastHit hit;
            if (Physics.Raycast(playerDropPos + Vector3.up * 5f, Vector3.down, out hit, 20f))
                playerDropPos.y = hit.point.y + 1f;
            else
                playerDropPos.y = car.position.y + 1f;

            player.position = playerDropPos;
            player.gameObject.SetActive(true);

            if (player.TryGetComponent<PlayerMovement>(out var pm)) pm.enabled = true;

            cameraTransform.SetParent(player);
            cameraTransform.localPosition = new Vector3(0, playerEyeHeight, 0);
            cameraTransform.localRotation = Quaternion.identity;
        }

        if (fadeImage != null)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                fadeImage.color = new Color(0, 0, 0, 1 - t);
                yield return null;
            }
        }

        // ВКЛЮЧАЕМ HUD  после катсцены
        if (hudCanvas != null) hudCanvas.SetActive(true);

        // --- ЗАПУСК МУЗЫКИ ДЕДА НА 10 СЕКУНД ---
        if (oldManMusic != null)
        {
            StartCoroutine(PlayOldManMusicRoutine());
        }
    }

    IEnumerator PlayOldManMusicRoutine()
    {
        oldManMusic.spatialBlend = 1f; // 3D-звук, чтобы исходил от деда

        if (oldManMusic.clip == null)
        {
            Debug.LogError("У деда не привязан аудиофайл в AudioSource!");
            yield break;
        }

        oldManMusic.volume = 0.6f;
        oldManMusic.Play();

        // Ждем 10 секунд
        yield return new WaitForSeconds(10f);

        // Плавно выключаем звук за 2 секунды
        float fadeTime = 2f;
        float startVolume = oldManMusic.volume;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            oldManMusic.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        oldManMusic.Stop();
        oldManMusic.volume = startVolume; // Возвращаем громкость для возможного повторного включения
    }
}