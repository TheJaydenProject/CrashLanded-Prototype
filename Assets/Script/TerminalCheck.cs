using UnityEngine;

public class TerminalCheck : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Distance the player must be to interact with the terminal")]
    public float interactDistance = 2f;

    [Tooltip("The player's camera for raycasting")]
    public Transform playerCamera;

    [Header("UI Elements")]
    [Tooltip("UI prompt that appears when player looks at the terminal")]
    public GameObject lookPrompt;

    [Tooltip("Warning UI if repair module not found")]
    public GameObject warningPanel;

    [Tooltip("Congratulation panel shown if player has the repair module")]
    public GameObject congratsPanel;

    [Tooltip("Time before ending the game")]
    public float endGameDelay = 5f;

    [Tooltip("Time warning message stays on screen")]
    public float warningDuration = 3f;

    private PlayerInventory inventory;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
            Debug.LogWarning("[TerminalCheck] PlayerInventory not found.");
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (lookPrompt != null)
                    lookPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    HandleInteraction();
                }
            }
            else
            {
                if (lookPrompt != null)
                    lookPrompt.SetActive(false);
            }
        }
        else
        {
            if (lookPrompt != null)
                lookPrompt.SetActive(false);
        }
    }

    void HandleInteraction()
    {
        if (inventory == null) return;

        if (!inventory.HasRepairModule())
        {
            ShowWarning();
        }
        else
        {
            ShowCongrats();
            Invoke(nameof(EndGame), endGameDelay);
        }
    }

    void ShowWarning()
    {
        if (warningPanel)
        {
            warningPanel.SetActive(true);
            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), warningDuration);
        }
    }

    void HideWarning()
    {
        if (warningPanel)
            warningPanel.SetActive(false);
    }

    void ShowCongrats()
    {
        if (congratsPanel)
            congratsPanel.SetActive(true);
    }

    void EndGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
