using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public KillerChase killerScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок прошел через ExitTrigger.");

            if (killerScript != null)
            {
                killerScript.StartChaseAfterExit();
            }
            else
            {
                Debug.LogError("Killer Script не привязан к ExitTrigger!");
            }
        }
    }
}