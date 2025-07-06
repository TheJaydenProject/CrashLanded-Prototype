using UnityEngine;

public class RepairModulePickupHandler : MonoBehaviour
{
    public void Interact()
    {
        PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.GiveRepairModule();

            // Immediately hide or remove the pickup object
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[RepairModulePickup] Could not find PlayerInventory component.");
        }
    }
}
