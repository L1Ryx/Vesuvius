using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

//Handles playerinput of reality shifting. Calls event that reality shifts, which is heard by listeners elsewhere.
//Reality shifting not done here so that other mechanics can also trigger it more naturally.
public class PlayerRealityShift : MonoBehaviour
{
        private PlayerControls swingControls;
        public PlayerUnlocks playerUnlocks;
        public UnityEvent playerRealityChanged;

        private void Awake()
        {
            swingControls = new PlayerControls();
        }

        private void OnEnable()
        {
            swingControls.Player.RealityChange.performed += OnRealityShift;
            swingControls.Player.Enable();
        }

        private void OnDisable()
        {
            swingControls.Player.RealityChange.performed -= OnRealityShift;
            swingControls.Player.Disable();
        }

        private void OnRealityShift(InputAction.CallbackContext context)
        {
            if(playerUnlocks.canRealityShift)
            {
                playerRealityChanged.Invoke();
            }
        }
}
