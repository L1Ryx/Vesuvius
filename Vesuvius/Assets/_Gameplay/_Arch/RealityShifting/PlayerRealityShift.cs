using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

//Handles playerinput of reality shifting. Calls event that reality shifts, which is heard by listeners elsewhere.
//Reality shifting not done here so that other mechanics can also trigger it more naturally.
public class PlayerRealityShift : MonoBehaviour
{
        public PlayerUnlocks playerUnlocks;
        public UnityEvent playerRealityChanged;


        private void OnEnable()
        {
            PlayerControlManager.Instance.controls.Player.RealityShift.performed += OnRealityShift;
        }

        private void OnDisable()
        {
            PlayerControlManager.Instance.controls.Player.RealityShift.performed -= OnRealityShift;
        }

        private void OnRealityShift(InputAction.CallbackContext context)
        {
            if(playerUnlocks.canRealityShift)
            {
                playerRealityChanged.Invoke();
            }
        }
}
