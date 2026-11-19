using UnityEngine;

[ExecuteInEditMode]
public class AutoDoorSetup : MonoBehaviour
{
    public bool fixDoorNow = false;
    public float pivotOffsetX = -0.5f;

    void Update()
    {
        if (fixDoorNow)
        {
            fixDoorNow = false;
            FixDoor();
        }
    }

    void FixDoor()
    {
        // 1. Создаем петлю
        GameObject pivotObj = new GameObject(gameObject.name + "_Pivot");
        pivotObj.transform.SetParent(transform.parent);
        pivotObj.transform.position = transform.position + transform.right * pivotOffsetX;
        pivotObj.transform.rotation = transform.rotation;

        // 2. Привязываем дверь
        transform.SetParent(pivotObj.transform);

        // 3. Вешаем скрипт
        DoorInteraction doorScript = pivotObj.AddComponent<DoorInteraction>();
        doorScript.openAngle = 90f;

        enabled = false;
    }
}