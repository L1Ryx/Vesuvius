using _Gameplay._Arch;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class KeyItemAcquisition : MonoBehaviour
{
    public GameObject menuPrefab;
    public PlayerUnlocks playerUnlocks;

    public delegate void OnMenuClosed(); // Delegate to notify when the menu is closed
    public event OnMenuClosed MenuClosed;

    protected GameObject player;
    protected PlayerController playerController; // Dynamically fetched PlayerController
    protected GameObject menuInstance;

    public UnityEvent menuClosed;

    private void Awake()
    {

    }

    protected virtual void OnEnable()
    {
        PlayerControlManager.Instance.controls.Player.Interact.performed += CloseMenu;
    }

    protected virtual void OnDisable()
    {
        PlayerControlManager.Instance.controls.Player.Interact.performed -= CloseMenu;
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
        // TEMP AUDIO CODE
        AkSoundEngine.PostEvent("Play_UISelect", gameObject);
        if (playerController == null)
        {
            Debug.LogWarning("Cannot open menu. PlayerController not assigned.");
            return;
        }

        menuInstance = Instantiate(menuPrefab, this.transform);

        PlayerControlManager.Instance.EnterMenuMode();

        DisableControls();

        Debug.Log("Menu opened and player frozen.");
    }

    protected void CloseMenu(InputAction.CallbackContext context)
    {
        if (PlayerControlManager.Instance.IsInMenuMode())
        {
            PlayerControlManager.Instance.ExitMenuMode();

            if (playerController == null)
            {
                Debug.LogWarning("Cannot close menu. PlayerController not assigned.");
                return;
            }

            // Unfreeze player movement
            EnableControls();
            Destroy(menuInstance);
            menuClosed.Invoke();

            Debug.Log("Menu closed and player unfrozen.");
        }
    }

    //virtual methods incase inheritors want to change which controls are enabled/disabled
    protected virtual void DisableControls()
    {
        // Freeze player movement
        playerController.SetFreezeMode(true);
        PlayerControlManager.Instance.controls.Player.RealityShift.Disable();
        PlayerControlManager.Instance.controls.Player.Swing.Disable();
        PlayerControlManager.Instance.controls.Player.Heal.Disable();
    }

    protected virtual void EnableControls()
    {
        // Unfreeze player movement
        playerController.SetFreezeMode(false);
        PlayerControlManager.Instance.controls.Player.RealityShift.Enable();
        PlayerControlManager.Instance.controls.Player.Swing.Enable();
        PlayerControlManager.Instance.controls.Player.Heal.Enable();
    }

}
