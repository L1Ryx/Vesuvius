using _Gameplay._Arch;

namespace Events._Arch
{
    public class DoorEventListener : GameEventListener
    {
        public DoorController doorController;

        public override void OnEventRaised() {
            // doorController.AttemptTransition();
            // deprecated
        }
    }
}