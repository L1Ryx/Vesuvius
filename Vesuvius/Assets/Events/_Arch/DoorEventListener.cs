using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorEventListener : GameEventListener
{
    public DoorController doorController;

    public override void OnEventRaised() {
        // doorController.AttemptTransition();
        // deprecated
    }
}