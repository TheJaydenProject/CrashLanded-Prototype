using UnityEngine;

/// <summary>
/// Handles raycast-based interaction for picking up a repair module.
/// Displays UI prompts when looking at the repair module, and shows the acquired status.
/// </summary>
/*
 * Description: This script allows the player to detect and pick up a repair module using raycasting.
 * It shows a prompt when looking at the repair module, calls the pickup logic on interaction,
 * and toggles a "repair module acquired" panel based on inventory state.
 */
public class RepairModuleRaycastInteractor : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>
    /// Maximum distance allowed for interacting with the repair module.
    /// </summary>
    [Tooltip("Maximum distance to interact with repair modules")]
    public float interactDistance = 2f;

    [Header("References")]

    /// <summary>
    /// Transform from where the raycast is fired (typically the player camera).
    /// </summary>
    [Tooltip("Point from which the raycast is fired (usually the camera)")]
    public Transform checkOrigin;

    /// <summary>
    /// UI panel displayed when the player is aiming at a repair module.
    /// </summary>
    [Tooltip("UI shown when player is looking at a repair module")]
    public GameObject repairPromptPanel;

    /// <summary>
    /// UI panel that indicates the repair module has been collected.
    /// </summary>
    [Tooltip("UI shown when player has collected the repair module")]
    public GameObject repairAcquiredPanel;

    /// <summary>
    /// The currently targeted repair module pickup in view.
    /// </summary>
    private RepairModulePickupHandler currentRepairModule;

    /// <summary>
    /// Reference to the player's inventory script.
    /// </summary>
    private PlayerInventory inventory;

    void Start()
    {
        // Attempt to get the PlayerInventory from the object tagged "Player"
        inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory == null)
            Debug.LogWarning("[RepairModuleInteractor] PlayerInventory not found on Player object.");
    }

    void Update()
    {
        // Early return if essential references are missing
        if (checkOrigin == null || repairPromptPanel == null || inventory == null)
        {
            Debug.LogWarning("[RepairModuleInteractor] Missing reference(s): checkOrigin, promptPanel, or inventory.");
            return;
        }

        // Create a ray from the camera forward direction
        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.yellow); // For editor debugging

        // Perform the raycast to detect objects in front
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Check if the object hit has the "RepairModule" tag
            if (hit.collider.CompareTag("RepairModule"))
            {
                // Try to get the RepairModulePickupHandler script on the object
                RepairModulePickupHandler pickup = hit.collider.GetComponent<RepairModulePickupHandler>();

                // If a new valid repair module is detected
                if (pickup != null && pickup != currentRepairModule)
                {
                    HidePrompt(); // Hide any existing prompt
                    currentRepairModule = pickup;
                    repairPromptPanel.SetActive(true); // Show new prompt
                    Debug.Log("[RepairModuleInteractor] Looking at repair module: " + pickup.name);
                }

                // If the player presses 'E', pick up the repair module
                if (Input.GetKeyDown(KeyCode.E) && currentRepairModule != null)
                {
                    pickup.Interact(); // Call the pickup logic
                    currentRepairModule = null;
                    repairPromptPanel.SetActive(false); // Hide prompt after pickup
                    Debug.Log($"[RepairModuleInteractor] Repair module collected and added to inventory: {pickup.name}");
                }

                return; // Exit early to prevent prompt being hidden below
            }
        }

        // If not looking at a repair module, hide prompt and reset
        HidePrompt();
        currentRepairModule = null;

        // Toggle the acquired panel based on whether the player owns a repair module
        if (repairAcquiredPanel != null)
        {
            bool hasModule = inventory.HasRepairModule();
            repairAcquiredPanel.SetActive(hasModule);
        }
    }

    /// <summary>
    /// Hides the repair module interaction prompt if it is active.
    /// </summary>
    void HidePrompt()
    {
        if (repairPromptPanel != null && repairPromptPanel.activeSelf)
        {
            repairPromptPanel.SetActive(false);
            Debug.Log("[RepairModuleInteractor] Prompt hidden.");
        }
    }
}
