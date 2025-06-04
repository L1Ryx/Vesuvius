using Unity.Cinemachine;
using UnityEngine;

public class BossRoomCameraSwitcher : MonoBehaviour
{
    public CinemachineCamera playerCamera;
    public CinemachineCamera bossCamera;

    public void UseBossCam()
    {
        bossCamera.Priority = 20;
        playerCamera.Priority = 10;
    }

    public void UsePlayerCamera()
    {
        playerCamera.Priority = 20;
        bossCamera.Priority = 10;
    }
}
