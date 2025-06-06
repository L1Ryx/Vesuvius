using _Gameplay._Arch;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class KeyItemAcquisition : MonoBehaviour
{
    public GameObject menuPrefab;
    public PlayerUnlocks playerUnlocks;
    public UnsavedPlayerInfo unsavedPlayerInfo;

    public delegate void OnMenuClosed(); // Delegate to notify when the menu is closed
    public event OnMenuClosed MenuClosed;

    private GameObject player;
    private PlayerController playerController; // Dynamically fetched PlayerController
    private GameObject menuInstance;

    public UnityEvent menuClosed;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        PlayerControlManager.Instance.controls.Player.RealityShift.performed += CloseMenu;

        // Disable the Interact action
        PlayerControlManager.Instance.controls.Player.Interact.Disable();
        PlayerControlManager.Instance.controls.Player.Swing.Disable();
        PlayerControlManager.Instance.controls.Player.Heal.Disable();
    }

    private void OnDisable()
    {
        PlayerControlManager.Instance.controls.Player.RealityShift.performed -= CloseMenu;

        PlayerControlManager.Instance.controls.Player.Interact.Enable();
        PlayerControlManager.Instance.controls.Player.Swing.Enable();
        PlayerControlManager.Instance.controls.Player.Heal.Enable();
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer();
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure PlayerSpawner and runtime player setup is correct.");
            return;
        }

        // Dynamically fetch PlayerController from the player
        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on the player.");
        }
    }

    public void OpenMenu()
    {
        if (playerController == null)
        {
            Debug.LogWarning("Cannot open menu. PlayerController not assigned.");
            return;
        }

        menuInstance = Instantiate(menuPrefab, this.transform);

        unsavedPlayerInfo.isInMenuMode = true;

        // Freeze player movement
        playerController.SetFreezeMode(true);

        Debug.Log("Menu opened and player frozen.");
    }

    private void CloseMenu(InputAction.CallbackContext context)
    {
        if(unsavedPlayerInfo.isInMenuMode == true)
        {
            unsavedPlayerInfo.isInMenuMode = false;

            if (playerController == null)
            {
                Debug.LogWarning("Cannot close menu. PlayerController not assigned.");
                return;
            }

            // Unfreeze player movement
            playerController.SetFreezeMode(false);
            Destroy(menuInstance);
            menuClosed.Invoke();

            Debug.Log("Menu closed and player unfrozen.");
        }
    }

}
