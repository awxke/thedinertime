using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class KillerChase : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public Transform escapePoint;
    public Transform cameraTransform;
    public Transform spawnPoint;
    public GameObject killerModel;

    [Header("Настройки погони")]
    public float chaseSpeed = 2.5f;
    public float catchDistance = 1.5f;
    public float escapeDistance = 2f;

    [Header("Настройки катсцены")]
    public float zoomDistance = 1.5f;
    public float zoomDuration = 2f;
    public float lookDuration = 3f;
    public float returnDuration = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing = false;
    private bool canChase = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null) agent.enabled = false;
        if (animator != null) animator.SetBool("isRunning", false);
    }

    public void TeleportAndWait()
    {
        if (killerModel != null) killerModel.SetActive(true);

        if (spawnPoint != null)
        {
            if (agent != null) agent.enabled = false;
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        if (animator != null) animator.SetBool("isRunning", false);
        canChase = true;
    }

    public void StartChaseAfterExit()
    {
        if (isChasing) return;
        if (!canChase) return;

        // ВКЛЮЧАЕМ МУЗЫКУ ПОГОНИ
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartChase();
        }

        StartCoroutine(ChaseSequence());
    }

    IEnumerator ChaseSequence()
    {
        isChasing = true;

        if (player.TryGetComponent<PlayerMovement>(out var pm)) pm.enabled = false;

        if (cameraTransform != null)
        {
            Vector3 originalCamPos = cameraTransform.position;
            Quaternion originalCamRot = cameraTransform.rotation;

            Vector3 facePos = transform.position + Vector3.up * 1.5f;
            Vector3 targetCamPos = transform.position + transform.forward * zoomDistance + Vector3.up * 1.5f;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / zoomDuration;
                cameraTransform.position = Vector3.Lerp(originalCamPos, targetCamPos, t);
                cameraTransform.LookAt(facePos);
                yield return null;
            }

            yield return new WaitForSeconds(lookDuration);

            t = 0;
            Vector3 startReturnPos = cameraTransform.position;
            Quaternion startReturnRot = cameraTransform.rotation;

            while (t < 1f)
            {
                t += Time.deltaTime / returnDuration;
                cameraTransform.position = Vector3.Lerp(startReturnPos, originalCamPos, t);
                cameraTransform.rotation = Quaternion.Slerp(startReturnRot, originalCamRot, t);
                yield return null;
            }
        }

        if (player.TryGetComponent<PlayerMovement>(out var pmOut)) pmOut.enabled = true;

        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = chaseSpeed;
        }
        if (animator != null) animator.SetBool("isRunning", true);
    }

    void Update()
    {
        if (!isChasing) return;

        if (player != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }

        if (player != null && Vector3.Distance(transform.position, player.position) < catchDistance)
        {
            CatchPlayer();
        }

        if (escapePoint != null && Vector3.Distance(player.position, escapePoint.position) < escapeDistance)
        {
            PlayerEscaped();
        }
    }

    void CatchPlayer()
    {
        isChasing = false;
        if (agent != null) agent.enabled = false;
        if (animator != null) animator.SetBool("isRunning", false);

        // ВЫКЛЮЧАЕМ МУЗЫКУ
        if (AudioManager.Instance != null) AudioManager.Instance.StopChase();

        Debug.Log("Вас поймали! (Тут нужно сделать экран смерти)");
    }

    void PlayerEscaped()
    {
        isChasing = false;
        if (agent != null) agent.enabled = false;
        if (animator != null) animator.SetBool("isRunning", false);

        // ВЫКЛЮЧАЕМ МУЗЫКУ
        if (AudioManager.Instance != null) AudioManager.Instance.StopChase();

        Debug.Log("Вы сбежали! (Тут нужно будет сделать переход в следующую сцену)");
    }
}