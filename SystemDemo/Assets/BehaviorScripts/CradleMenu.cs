using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CradleMenu : MonoBehaviour
{
    [Header("Menu Options")]
    public TMP_Text[] menuOptions; // Only menu option texts (for navigation)

    [Header("Other Text Elements")]
    public TMP_Text[] nonMenuTexts; // Texts not used as options (e.g., titles or descriptions)

    [Header("Settings")]
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;
    private PlayerControls playerControls;

    public delegate void OnMenuClosed(); // Delegate to notify when the menu is closed
    public event OnMenuClosed MenuClosed;

    private void Awake()
    {
        playerControls = new PlayerControls();

        // Subscribe to navigation and selection actions
        playerControls.Player.Navigate.performed += OnNavigate;
        playerControls.Player.Confirm.performed += OnSelect;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        // Make all menu and non-menu texts fully visible
        SetTextAlpha(menuOptions, 1);
        SetTextAlpha(nonMenuTexts, 1);
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        // Ensure all texts are fully visible at the start
        SetTextAlpha(menuOptions, 1);
        SetTextAlpha(nonMenuTexts, 1);

        UpdateMenu(); // Highlight the default selected option
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        float navigationValue = context.ReadValue<float>();

        if (navigationValue > 0.1f) // Move up
        {
            selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
            UpdateMenu();
        }
        else if (navigationValue < -0.1f) // Move down
        {
            selectedIndex = (selectedIndex + 1) % menuOptions.Length;
            UpdateMenu();
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        string selectedOption = menuOptions[selectedIndex].text;

        Debug.Log($"Selected option: {selectedOption}");

        switch (selectedOption)
        {
            case "Rest":
                Debug.Log("Rest functionality to be implemented.");
                break;

            case "Read":
                Debug.Log("Read functionality to be implemented.");
                break;

            case "Leave":
                CloseMenu();
                break;
        }
    }

    private void UpdateMenu()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (i == selectedIndex)
            {
                menuOptions[i].color = selectedColor;
                menuOptions[i].fontStyle = FontStyles.Underline;
            }
            else
            {
                menuOptions[i].color = defaultColor;
                menuOptions[i].fontStyle = FontStyles.Normal;
            }
        }
    }

    private void SetTextAlpha(TMP_Text[] texts, float alpha)
    {
        foreach (var text in texts)
        {
            var currentColor = text.color;
            text.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }

    private void CloseMenu()
    {
        Debug.Log("Menu closed.");
        MenuClosed?.Invoke(); // Notify any listeners that the menu is closing
        gameObject.SetActive(false); // Hide the menu
    }
}
