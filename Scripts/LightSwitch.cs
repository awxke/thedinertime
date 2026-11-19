using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light[] lightsToToggle;
    public float interactionDistance = 2f;
    public Transform playerCamera;

    private bool isOn = true;

    void Start()
    {
        if (playerCamera == null && Camera.main != null) playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ToggleLights();
                }
            }
        }
    }

    void ToggleLights()
    {
        isOn = !isOn;
        foreach (Light l in lightsToToggle)
        {
            if (l != null) l.enabled = isOn;
        }
    }
}