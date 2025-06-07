using _Gameplay._Arch;
using Public.Tarodev_2D_Controller.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour
{
	private static PlayerControlManager _Instance;
	public static PlayerControlManager Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = new GameObject().AddComponent<PlayerControlManager>();
				// name it for easy recognition
				_Instance.name = _Instance.GetType().ToString();
				// mark root as DontDestroyOnLoad();
				DontDestroyOnLoad(_Instance.gameObject);
			}
			return _Instance;
		}
	}

    
    private PlayerControls m_Controls;

    
    public PlayerControls controls
    {
        get
        {
            if (m_Controls == null)
                m_Controls = new PlayerControls();
            return m_Controls;
        }
    }

    private GameObject player;
    private PlayerController playerController; // Dynamically fetched PlayerController
    private PlayerDamage playerDamage;

    private int overlappingCalls = 0; 
    private void Awake()
    {
        controls.Enable();
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer();
        if (player != null)
        {
            // Dynamically fetch PlayerController from the player
            playerController = player.GetComponent<PlayerController>();
            playerDamage = player.GetComponent<PlayerDamage>(); 
        }

    }

    private void OnDisable()
    {
        controls.Disable();
    }

    //overlappingCalls keeps track of disable/enable calls to make sure that all
    //disables have been balanced with an enable call before re-enabling controls
    //ex. cutscene disable, dialogue disable, dialogue enable, cutscene enable 
    public void DisableNormalControls()
    {
        if (playerController == null)
        {
            player = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer();
            playerController = player.GetComponent<PlayerController>();
            playerDamage = player.GetComponent<PlayerDamage>(); 
        }
        overlappingCalls++;

        playerController.SetFreezeMode(true);
        playerDamage.SetInvincibility();
        PlayerControlManager.Instance.controls.Player.Swing.Disable();
        PlayerControlManager.Instance.controls.Player.Heal.Disable();
        PlayerControlManager.Instance.controls.Player.RealityShift.Disable();

    }

    public void EnableNormalControls()
    {
        overlappingCalls--;
        if (overlappingCalls == 0)
        {
            playerDamage.ResetInvincibility();
            playerController.SetFreezeMode(false);
            PlayerControlManager.Instance.controls.Player.Swing.Enable();
            PlayerControlManager.Instance.controls.Player.Heal.Enable();
            PlayerControlManager.Instance.controls.Player.RealityShift.Enable();
        }
    }
}
