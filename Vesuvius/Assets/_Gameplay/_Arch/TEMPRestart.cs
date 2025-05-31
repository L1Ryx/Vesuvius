using _Gameplay._Arch;
using _ScriptableObjects.PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEMPRestart : MonoBehaviour
{
    [SerializeField] PlayerInfo playerInfo;
    [SerializeField] ScriptableObjectManager scriptableObjectManager;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerInfo.hasCompletedDemo = false;
            scriptableObjectManager.DeleteSaveData();
            SceneManager.LoadScene(0);
        }
    }
}
