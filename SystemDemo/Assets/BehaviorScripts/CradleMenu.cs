using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Required for handling UI images

public class CradleMenu : MonoBehaviour
{
    [Header("Menu Options")]
    public TMP_Text[] menuOptions; // Only menu option texts (for navigation)

    [Header("Other Text Elements")]
    public TMP_Text[] nonMenuTexts; // Texts not used as options (e.g., titles or descriptions)

    [Header("UI Images")]
    public Image[] uiImages; // UI images to be faded in and out
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject readPanel;
    private TMP_Text[] readPanelTexts;


    [Header("Settings")]
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;
    public float fadeSpeed = 1f; // Speed of the fade-in and fade-out
    public float startDelay = 0.2f; // Delay before starting fade-in

    private int selectedIndex = 0;
    private PlayerControls playerControls;
    private bool isReading = false; // False = Main Panel, True = Read Panel


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
        StartCoroutine(FadeInUI());
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        UpdateMenu(); // Highlight the default selected option

        // Set initial alpha to 0 for texts and images
        SetTextAlpha(menuOptions, 0);
        SetTextAlpha(nonMenuTexts, 0);
        SetImageAlpha(uiImages, 0);

        // Temporarily activate readPanel to gather its TMP_Text components
        readPanel.SetActive(true);
        readPanelTexts = readPanel.GetComponentsInChildren<TMP_Text>();
        SetTextAlpha(readPanelTexts, 0); // Set initial alpha to 0 for read panel texts
        readPanel.SetActive(false); // Deactivate readPanel again
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
        if (isReading)
        {
            // Switch back to Main Panel
            StartCoroutine(SwitchPanels(readPanel, mainPanel));
            isReading = false;
        }
        else
        {
            string selectedOption = menuOptions[selectedIndex].text;

            Debug.Log($"Selected option: {selectedOption}");

            switch (selectedOption)
            {
                case "Rest":
                    Debug.Log("Rest functionality to be implemented.");
                    break;

                case "Read":
                    // Switch to Read Panel
                    StartCoroutine(SwitchPanels(mainPanel, readPanel));
                    isReading = true;
                    break;

                case "Leave":
                    StartCoroutine(FadeOutUIAndClose()); // Correct method name
                    break;
            }
        }
    }


private IEnumerator SwitchPanels(GameObject outgoingPanel, GameObject incomingPanel)
{
    CanvasGroup outgoingGroup = outgoingPanel.GetComponent<CanvasGroup>();
    CanvasGroup incomingGroup = incomingPanel.GetComponent<CanvasGroup>();

    TMP_Text[] outgoingTexts = outgoingPanel.GetComponentsInChildren<TMP_Text>();
    TMP_Text[] incomingTexts = incomingPanel.GetComponentsInChildren<TMP_Text>();

    float alpha = 1f;

    // Fade out outgoing panel
    while (alpha > 0f)
    {
        alpha -= Time.deltaTime * fadeSpeed;

        if (outgoingGroup != null)
            outgoingGroup.alpha = alpha;

        SetTextAlpha(outgoingTexts, alpha);
        SetImageAlpha(uiImages, alpha); // Use the class field for UI images
        yield return null;
    }

    if (outgoingGroup != null)
        outgoingGroup.alpha = 0f;

    SetTextAlpha(outgoingTexts, 0f);
    SetImageAlpha(uiImages, 0f); // Ensure the UI images are fully transparent
    outgoingPanel.SetActive(false);

    // Activate incoming panel and fade it in
    incomingPanel.SetActive(true);
    alpha = 0f;
    while (alpha < 1f)
    {
        alpha += Time.deltaTime * fadeSpeed;

        if (incomingGroup != null)
            incomingGroup.alpha = alpha;

        SetTextAlpha(incomingTexts, alpha);
        SetImageAlpha(uiImages, alpha); // Use the class field for UI images
        yield return null;
    }

    if (incomingGroup != null)
        incomingGroup.alpha = 1f;

    SetTextAlpha(incomingTexts, 1f);
    SetImageAlpha(uiImages, 1f); // Ensure the UI images are fully visible
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

    private void SetImageAlpha(Image[] images, float alpha)
    {
        foreach (var image in images)
        {
            var currentColor = image.color;
            image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }

    private IEnumerator FadeInUI()
    {
        yield return new WaitForSeconds(startDelay); // Short delay to avoid race conditions

        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetTextAlpha(menuOptions, alpha);
            SetTextAlpha(nonMenuTexts, alpha);
            SetImageAlpha(uiImages, alpha);
            yield return null;
        }

        // Ensure full alpha
        SetTextAlpha(menuOptions, 1f);
        SetTextAlpha(nonMenuTexts, 1f);
        SetImageAlpha(uiImages, 1f);
    }

    private IEnumerator FadeOutUIAndClose()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetTextAlpha(menuOptions, alpha);
            SetTextAlpha(nonMenuTexts, alpha);
            SetImageAlpha(uiImages, alpha);
            yield return null;
        }

        // Ensure zero alpha
        SetTextAlpha(menuOptions, 0f);
        SetTextAlpha(nonMenuTexts, 0f);
        SetImageAlpha(uiImages, 0f);

        MenuClosed?.Invoke(); // Notify any listeners that the menu is closing
        Destroy(gameObject); // Destroy the menu object
    }
}
