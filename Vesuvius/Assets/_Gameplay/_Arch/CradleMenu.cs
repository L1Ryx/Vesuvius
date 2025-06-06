using System.Collections;
using _ScriptableObjects.PlayerInfo;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Required for handling UI images

namespace _Gameplay._Arch
{
    public class CradleMenu : MonoBehaviour
    {
        [Header("Menu Options")]
        public TMP_Text[] menuOptions; // Only menu option texts (for navigation)

        [Header("Other Text Elements")]
        public TMP_Text[] nonMenuTexts; // Texts not used as options (e.g., titles or descriptions)

        [Header("Lore Text")]
        public TMP_Text loreText;

        [Header("UI Images")]
        public Image[] uiImages; // UI images to be faded in and out
        [Header("Panels")]
        public GameObject mainPanel;
        public GameObject readPanel;
        private TMP_Text[] readPanelTexts;
        [Header("Rest Panel")]
        public GameObject restPanel; // The panel that veils the screen during rest
        public float veilDuration = 1f; // Time it takes for the veil to fade in/out
        public float restDuration = 3f; // Duration for which the veil stays fully opaque
        public PlayerInfo playerInfo; // Reference to the PlayerInfo ScriptableObject
        [Header("Rest Panel - Saved Text")]
        public TMP_Text savedText; // Reference to the "Saved Text"
        public float savedTextLerpDuration = 1f; // Duration for the text to fade in and out
        [Header("Events")]
        public UnityEvent uiNavigated;
        public UnityEvent uiSelected;
        public UnityEvent restStarted;
        public UnityEvent gameSaved;




        [Header("Settings")]
        public Color defaultColor = Color.white;
        public Color selectedColor = Color.yellow;
        public float fadeSpeed = 1f; // Speed of the fade-in and fade-out
        public float startDelay = 0.2f; // Delay before starting fade-in
    

        private int selectedIndex = 0;

        private bool isReading = false; // False = Main Panel, True = Read Panel
        private bool menuActive = true; // Start as true since the menu is active initially



        public delegate void OnMenuClosed(); // Delegate to notify when the menu is closed
        public event OnMenuClosed MenuClosed;

        private void Awake()
        {

        }

        private void OnEnable()
        {
            PlayerControlManager.Instance.controls.Player.Navigate.performed += OnNavigate;
            PlayerControlManager.Instance.controls.Player.Confirm.performed += OnSelect;
            StartCoroutine(HandleRest());
            //StartCoroutine(FadeInUI());
            menuActive = true; // Menu is now active

            // Disable the Interact action
            PlayerControlManager.Instance.controls.Player.Interact.Disable();
            PlayerControlManager.Instance.controls.Player.Swing.Disable();
            PlayerControlManager.Instance.controls.Player.Heal.Disable();
            PlayerControlManager.Instance.controls.Player.RealityShift.Disable();
        }

        private void OnDisable()
        {
            PlayerControlManager.Instance.controls.Player.Navigate.performed -= OnNavigate;
            PlayerControlManager.Instance.controls.Player.Confirm.performed -= OnSelect;
            menuActive = false; // Menu is no longer active

            PlayerControlManager.Instance.controls.Player.Interact.Enable();
            PlayerControlManager.Instance.controls.Player.Swing.Enable();
            PlayerControlManager.Instance.controls.Player.Heal.Enable();
            PlayerControlManager.Instance.controls.Player.RealityShift.Enable();
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
           // SetTextAlpha(readPanelTexts, 0); // Set initial alpha to 0 for read panel texts
            readPanel.SetActive(false); // Deactivate readPanel again

            if (savedText != null)
            {
                SetTextAlpha(savedText, 0f); // Ensure the "Saved Text" starts fully transparent
            }

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
            uiNavigated.Invoke();
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            if (!menuActive) return; // Ignore input if the menu is not active

            if (isReading)
            {
                // Switch back to Main Panel
                StartCoroutine(SwitchPanels(readPanel, mainPanel));
                isReading = false;
            }
            else
            {
                string selectedOption = menuOptions[selectedIndex].text;

                switch (selectedOption)
                {
                    case "Rest":
                        StartCoroutine(HandleRest());
                        break;

                    case "Read":
                        // Switch to Read Panel
                        StartCoroutine(SwitchPanels(mainPanel, readPanel));
                        isReading = true;
                        break;

                    case "Read Inscription":
                        for (int i = 0; i < menuOptions.Length; i++)
                        {
                            if (i == selectedIndex)
                            {
                                if (menuOptions[i].fontStyle.HasFlag(FontStyles.Bold))
                                {
                                    menuOptions[i].color = selectedColor;
                                    menuOptions[i].fontStyle = menuOptions[i].fontStyle & ~FontStyles.Bold;
                                }
                                else
                                {
                                    menuOptions[i].color = selectedColor;
                                    menuOptions[i].fontStyle = menuOptions[i].fontStyle | FontStyles.Bold;
                                }
                            }
                            else
                            {
                                //menuOptions[i].color = defaultColor;
                                //menuOptions[i].fontStyle = FontStyles.Normal;
                            }
                        }
                        readPanel.SetActive(!readPanel.activeSelf);
                        break;


                    case "Leave":
                        StartCoroutine(FadeOutUIAndClose());
                        break;
                }

                uiSelected.Invoke();
            }
        }


        private IEnumerator HandleRest()
        {
            menuActive = false; // Lock menu interaction

            Image restImage = restPanel.GetComponent<Image>();
            if (restImage == null)
            {
                Debug.LogError("Rest panel is missing an Image component!");
                yield break;
            }

            restPanel.SetActive(true);

            // Fade in the rest panel
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime / veilDuration;
                SetImageAlpha(restImage, alpha);
                yield return null;
            }
            SetImageAlpha(restImage, 1f);

            // Restore player stats
            if (playerInfo != null)
            {
                playerInfo.SetCurrentHealth(playerInfo.GetMaximumHealth());
                playerInfo.SetTotemPower(100); // Fully restore totem power
                Debug.Log("Player stats restored: Health and Totem Power set to maximum.");

                // Get the parent EmptyCradle component
                EmptyCradle parentCradle = GetComponentInParent<EmptyCradle>();

                if (parentCradle != null)
                {
                    playerInfo.SetCheckpoint(parentCradle.checkpointScene, parentCradle.checkpointLocation);
                    Debug.Log($"Checkpoint updated: Scene - {parentCradle.checkpointScene}, Location - {parentCradle.checkpointLocation}");
                }
                else
                {
                    Debug.LogError("Parent EmptyCradle not found!");
                }
                restStarted.Invoke();
                gameSaved.Invoke();

            }
            else
            {
                Debug.LogError("PlayerInfo ScriptableObject is not assigned.");
            }

            // Lerp in the "Saved Text"
            if (savedText != null)
            {
                yield return StartCoroutine(LerpTextAlpha(savedText, 0f, 1f, savedTextLerpDuration));
            }

            // Wait for the rest duration
            yield return new WaitForSeconds(restDuration);

            // Lerp out the "Saved Text"
            if (savedText != null)
            {
                yield return StartCoroutine(LerpTextAlpha(savedText, 1f, 0f, savedTextLerpDuration));
            }

            // Fade out the rest panel
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime / veilDuration;
                SetImageAlpha(restImage, alpha);
                yield return null;
            }
            SetImageAlpha(restImage, 0f);

            restPanel.SetActive(false);
            StartCoroutine(FadeInUI());
            menuActive = true; // Unlock menu interaction
        }


        private IEnumerator LerpTextAlpha(TMP_Text text, float fromAlpha, float toAlpha, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
                SetTextAlpha(text, alpha);
                yield return null;
            }
            SetTextAlpha(text, toAlpha); // Ensure the final alpha is set
        }

        private void SetTextAlpha(TMP_Text text, float alpha)
        {
            if (text != null)
            {
                Color color = text.color;
                text.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
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
                    menuOptions[i].fontStyle = menuOptions[i].fontStyle | FontStyles.Underline;
                }
                else
                {
                    menuOptions[i].color = defaultColor;
                    menuOptions[i].fontStyle = menuOptions[i].fontStyle & ~FontStyles.Underline;
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

        private void SetImageAlpha(Image image, float alpha)
        {
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
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
            menuActive = false; // Prevent further input during fade-out

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
}
