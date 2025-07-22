using _Gameplay._Arch;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MirrorShardAcquistion : KeyItemAcquisition
{

    protected override void OnEnable()
    {
        PlayerControlManager.Instance.controls.Player.RealityShift.performed += CloseMenu;
        // Disable the Interact action
    }

    protected override void OnDisable()
    {
        PlayerControlManager.Instance.controls.Player.RealityShift.performed -= CloseMenu;
    }
    
    //we want player to use reality shift instead of interact to clear this menu, so override with that
    protected override void DisableControls()
    {
        // Freeze player movement
        playerController.SetFreezeMode(true);
        PlayerControlManager.Instance.controls.Player.Interact.Disable();
        PlayerControlManager.Instance.controls.Player.Swing.Disable();
        PlayerControlManager.Instance.controls.Player.Heal.Disable();
    }

    protected override void EnableControls()
    {
        // Unfreeze player movement
        playerController.SetFreezeMode(false);
        PlayerControlManager.Instance.controls.Player.Interact.Enable();
        PlayerControlManager.Instance.controls.Player.Swing.Enable();
        PlayerControlManager.Instance.controls.Player.Heal.Enable();
    }

}
