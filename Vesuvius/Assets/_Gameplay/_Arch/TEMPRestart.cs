using _ScriptableObjects.PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEMPRestart : MonoBehaviour
{
    [SerializeField] PlayerInfo playerInfo;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerInfo.hasCompletedDemo = false;
            SceneManager.LoadScene(0);
        }
    }
}
