using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    private Vector3 spawnPosition;
    private PlayerInventory subscribedInventory;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        spawnPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError($"{name}: PlayerInventory not found on '{other.name}' or its parents.");
            return;
        }

        inventory.CollectKey();

        // Subscribed here (not Awake) since the player reference is only
        // available once the player actually reaches the key. Stays
        // subscribed while inactive so it can hear the reset event.
        subscribedInventory = inventory;
        subscribedInventory.InventoryReset += HandleInventoryReset;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (subscribedInventory != null)
            subscribedInventory.InventoryReset -= HandleInventoryReset;
    }

    private void HandleInventoryReset()
    {
        transform.position = spawnPosition;
        gameObject.SetActive(true);

        subscribedInventory.InventoryReset -= HandleInventoryReset;
        subscribedInventory = null;
    }
}