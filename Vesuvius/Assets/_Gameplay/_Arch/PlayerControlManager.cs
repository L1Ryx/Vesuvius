using UnityEngine;

public class PlayerControlManager : MonoBehaviour
{
    public static PlayerControlManager Instance { get; private set; }
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
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
