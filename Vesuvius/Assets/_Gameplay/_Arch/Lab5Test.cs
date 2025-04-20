using UnityEngine;
using UnityEngine.UI;

namespace _Gameplay._Arch
{
    public class Lab5Test
    {
        private Button _quitButton;
        private void SetupUI()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _quitButton.gameObject.SetActive(false);
            }
            else
            {
                _quitButton.onClick.AddListener(QuitGame);
            }
        }
        private void QuitGame()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }
    }
}
