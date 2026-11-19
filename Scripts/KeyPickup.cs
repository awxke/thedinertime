using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyID = "KitchenKey";

    void Start()
    {
        // Добавляем ключ в статический список при старте (если он на сцене)
        if (!KeyManager.collectedKeys.Contains(keyID))
        {
            // Пока ничего не делаем, ключ подбирается через PlayerPickup
        }
    }
}