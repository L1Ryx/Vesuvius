using System;
using System.Collections;
using _ScriptableObjects;
using _ScriptableObjects.PlayerInfo;
using Events._Arch;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Gameplay._Arch
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Main Panel UI Elements")]
        public GameObject mainPanel;
        public TMP_Text[] allTexts; // Includes all TMP texts (title, options, etc.)
        public Image[] uiImages; // Includes all UI images
        public TMP_Text[] menuOptions; // Menu options for navigation

        [Header("Info Panel UI Elements")]
        public GameObject infoPanel; // The info panel, inactive by default
        public TMP_Text[] infoTexts; // Sequentially fade-in texts
        public TMP_Text[] infoOptions; // "Begin" and "Back" options for selection

        [Header("Settings")]
        public string bugReportURL = "https://www.google.com";
        public GameObject startingCradle;
        public string startScene = "01-01";
        public int startingX = 4;
        public int startingY = -3;
        public int startingMaximumHealth = 5;
        public Color defaultColor = Color.white;
        public Color selectedColor = Color.yellow;
        public float fadeSpeed = 1f;
        public float startDelay = 0.2f;
        public float fadeInterval = 0.5f; // Time between fading in texts in the info panel
        public float switchToStartSceneDelay = 2f;
        [Header("Completion Text")]
        public TMP_Text completionText; // Reference to the completion text
        [Header("Saved Data References")]
        public TMP_Text continueText;
        public ScriptableObjectManager scriptableObjectManager;

        [Header("Data Cubes")] 
        [SerializeField] private PlayerInfo playerInfo;
        [SerializeField] private SpawnData spawnData;
        [SerializeField] private GateData gateData;
        [SerializeField] private BulbData bulbData;
        [SerializeField] private TutorialData tutorialData;
        public GameState gameState;

        [Header("Events")]
        public UnityEvent roomEntered;
        public UnityEvent roomExited;
        public UnityEvent uiNavigated;
        public UnityEvent uiSelected;
        public UnityEvent gameSaved;
        public UnityEvent gameLoaded;
        public UnityEvent mainMenuLoaded;

        private int selectedIndex = 0;
        private PlayerControls playerControls;
        private bool menuActive = true;
        private bool infoPanelActive = false;
        private bool infoOptionsReady = false; // Failsafe to ensure options are fully visible

        private void InitializeGameState()
        {
            playerInfo.SetMaximumHealth(startingMaximumHealth);
            playerInfo.SetCurrentHealth(playerInfo.GetMaximumHealth());
            playerInfo.SetTotalCurrency(0);
            playerInfo.SetTotemPower(50);
            playerInfo.SetCheckpoint(startingCradle.GetComponent<EmptyCradle>().checkpointScene, 
                startingCradle.GetComponent<EmptyCradle>().checkpointLocation);

            spawnData.spawnLocation.x = playerInfo.GetSpawnLocation().x;
            spawnData.spawnLocation.y = playerInfo.GetSpawnLocation().y;

            gateData.ResetGates();
            bulbData.ResetBulbs();
            tutorialData.ResetTutorials();
            scriptableObjectManager.ResetAllData();
            gameState.Reset();

            roomExited.Invoke();
            gameSaved.Invoke();
            gameLoaded.Invoke();


            StartCoroutine(SwitchToStartScene());

        }

        private void LoadExistingGameState()
        {
            gameLoaded.Invoke();
            print("Loading");
            spawnData.spawnLocation.x = playerInfo.GetSpawnLocation().x;
            spawnData.spawnLocation.y = playerInfo.GetSpawnLocation().y;
            StartCoroutine(SwitchToStartScene());
        }

        private IEnumerator SwitchToStartScene() {
            yield return new WaitForSeconds(switchToStartSceneDelay);
            SceneManager.LoadScene(playerInfo.GetSceneToLoad());
        }

        private void Awake()
        {
            mainMenuLoaded.Invoke();
            playerControls = new PlayerControls();

            playerControls.Player.Navigate.performed += OnNavigate;
            playerControls.Player.Confirm.performed += OnConfirm;
            playerControls.Player.Quit.performed += OnQuit; // Subscribe to the Quit action
        }

        private void OnQuit(InputAction.CallbackContext context)
        {
            Debug.Log("On Quit Called.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
        Application.Quit(); // Quit the application in a build
#endif
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
            // Set all main panel texts and images to alpha 0
            SetTextAlpha(allTexts, 0);
            SetTextAlpha(menuOptions, 0);
            SetImageAlpha(uiImages, 0);

            // Set info panel texts and options to alpha 0
            SetTextAlpha(infoTexts, 0);
            SetTextAlpha(infoOptions, 0);

            if(scriptableObjectManager.CheckSaveDataExists())
            {
                continueText.gameObject.SetActive(true);
            }
            else
            {
                continueText.gameObject.SetActive(false);
            }

            if(continueText.gameObject.activeSelf)
            {
                SetTextAlpha(new TMP_Text[] { continueText }, 0);
                //Prepend the continue text to the menu options if it's active so it's the first thing underlined
                Array.Resize(ref menuOptions, menuOptions.Length + 1);
                Array.Copy(menuOptions, 0, menuOptions, 0 + 1, menuOptions.Length - 0 - 1);
                menuOptions[0] = continueText;
            }

            // Check if the demo has been completed
            if (playerInfo.hasCompletedDemo)
            {
                completionText.gameObject.SetActive(true); // Show completion text
            }
            else
            {
                completionText.gameObject.SetActive(false); // Hide completion text
            }

            // Ensure completion text starts with alpha 0
            if (completionText.gameObject.activeSelf)
            {
                SetTextAlpha(new TMP_Text[] { completionText }, 0);
            }

            infoPanel.SetActive(false); // Ensure info panel is inactive initially
            roomEntered.Invoke();

            // Hide and lock the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnNavigate(InputAction.CallbackContext context)
        {
            if (!menuActive) return;

            float navigationValue = context.ReadValue<float>();
            TMP_Text[] currentOptions = infoPanelActive ? infoOptions : menuOptions;

            if (infoPanelActive && !infoOptionsReady) return; // Prevent premature navigation

            if (navigationValue > 0.1f) // Move up or left
            {
                selectedIndex = (selectedIndex - 1 + currentOptions.Length) % currentOptions.Length;
                UpdateMenu(currentOptions);
            }
            else if (navigationValue < -0.1f) // Move down or right
            {
                selectedIndex = (selectedIndex + 1) % currentOptions.Length;
                UpdateMenu(currentOptions);
            }
            uiNavigated.Invoke();
        }


        private void OnConfirm(InputAction.CallbackContext context)
        {
            if (!menuActive) return;

            if (infoPanelActive)
            {
                if (!infoOptionsReady) return; // Prevent premature confirmation

                string selectedOption = infoOptions[selectedIndex].text;
                Debug.Log($"Selected option on info panel: {selectedOption}");

                switch (selectedOption)
                {
                    case "Begin":
                        InitializeGameState();
                        break;
                    case "Back":
                    
                        StartCoroutine(SwitchToMainPanel());
                        break;
                }
            }
            else
            {
                string selectedOption = menuOptions[selectedIndex].text;
                Debug.Log($"Selected option on main panel: {selectedOption}");

                switch (selectedOption)
                {
                    case "Continue":
                        LoadExistingGameState();
                        break;
                    case "Play":
                        Debug.Log("Play selected. Starting info panel sequence.");
                        StartCoroutine(SwitchToInfoPanel());
                        break;
                    case "Bug Report":
                        Application.OpenURL(bugReportURL);
                        break;
                    case "Exit":
                        QuitGame();
                        break;
                }
            }
            uiSelected.Invoke();
        }

        private IEnumerator SwitchToInfoPanel()
        {
            menuActive = false;

            // Start fading out all elements simultaneously
            Coroutine fadeTexts = StartCoroutine(FadeOutUI(allTexts));
            Coroutine fadeMenuOptions = StartCoroutine(FadeOutUI(menuOptions));
            Coroutine fadeImages = StartCoroutine(FadeOutUI(uiImages));

            // Wait for all fades to complete
            yield return fadeTexts;
            yield return fadeMenuOptions;
            yield return fadeImages;

            mainPanel.SetActive(false);

            // Activate info panel and start fading in texts
            infoPanel.SetActive(true);
            infoPanelActive = true;
            menuActive = true;

            yield return FadeInInfoPanel();
        }


        private IEnumerator SwitchToMainPanel()
        {
            menuActive = false;

            // Fade out info panel
            yield return FadeOutUI(infoTexts);
            yield return FadeOutUI(infoOptions);

            infoPanel.SetActive(false);

            // Activate main panel and fade it in
            mainPanel.SetActive(true);
            infoPanelActive = false;
            menuActive = true;

            yield return FadeInUI();
        }

        private IEnumerator FadeInInfoPanel()
        {
            for (int i = 0; i < infoTexts.Length; i++)
            {
                yield return StartCoroutine(FadeInText(infoTexts[i]));
                yield return new WaitForSeconds(fadeInterval);
            }

            // Fade in options after all info texts are visible
            yield return StartCoroutine(FadeInUI(infoOptions));

            // Initialize navigation logic for info panel options
            infoOptionsReady = true;
            selectedIndex = 0; // Reset selection index for the options
            UpdateMenu(infoOptions); // Highlight the first option
        }



        private IEnumerator FadeInUI()
        {
            yield return new WaitForSeconds(startDelay);

            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                SetTextAlpha(allTexts, alpha);
                SetTextAlpha(menuOptions, alpha);
                SetImageAlpha(uiImages, alpha);

                // Fade in completion text if it is active
                if (completionText.gameObject.activeSelf)
                {
                    SetTextAlpha(new TMP_Text[] { completionText }, alpha);
                }

                yield return null;
            }

            SetTextAlpha(allTexts, 1f);
            SetTextAlpha(menuOptions, 1f);
            SetImageAlpha(uiImages, 1f);

            // Ensure completion text is fully visible
            if (completionText.gameObject.activeSelf)
            {
                SetTextAlpha(new TMP_Text[] { completionText }, 1f);
            }

            UpdateMenu(menuOptions); // Update menu after fade-in
        }



        private void UpdateMenu(TMP_Text[] currentOptions)
        {
            for (int i = 0; i < currentOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    currentOptions[i].color = selectedColor;
                    currentOptions[i].fontStyle = FontStyles.Underline;
                }
                else
                {
                    currentOptions[i].color = defaultColor;
                    currentOptions[i].fontStyle = FontStyles.Normal;
                }
            }
        }

        private IEnumerator FadeOutUI(Image[] images)
        {
            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                SetImageAlpha(images, Mathf.Clamp01(alpha));
                yield return null;
            }
        }

        private IEnumerator FadeOutImage(Image image)
        {
            float alpha = image.color.a;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                SetImageAlpha(new Image[] { image }, Mathf.Clamp01(alpha));
                yield return null;
            }
        }


        private IEnumerator FadeInUI(TMP_Text[] texts)
        {
            foreach (var text in texts)
            {
                yield return StartCoroutine(FadeInText(text));
            }
        }

        private IEnumerator FadeInText(TMP_Text text)
        {
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                SetTextAlpha(new TMP_Text[] { text }, alpha);
                yield return null;
            }
        }

        private IEnumerator FadeOutUI(TMP_Text[] texts)
        {
            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                SetTextAlpha(texts, Mathf.Clamp01(alpha));
                yield return null;
            }
        }

        private IEnumerator FadeOutText(TMP_Text text)
        {
            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                SetTextAlpha(new TMP_Text[] { text }, alpha);
                yield return null;
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
        Application.Quit(); // Quit the application in a build
#endif
        }

        private void SetTextAlpha(TMP_Text[] texts, float alpha)
        {
            foreach (var text in texts)
            {
                if (text != null)
                {
                    var color = text.color;
                    text.color = new Color(color.r, color.g, color.b, alpha);
                }
            }
        }

        private void SetImageAlpha(Image[] images, float alpha)
        {
            foreach (var image in images)
            {
                if (image != null)
                {
                    var color = image.color;
                    image.color = new Color(color.r, color.g, color.b, alpha);
                }
            }
        }



    }
}
