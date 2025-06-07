using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;
    //called after teleport animation hides boss sprite to move them to teleported location
    public void MoveBoss()
    {
        print("called");
        sprite.enabled = !sprite.enabled;
    }
    
    // TEMP AUDIO CODE
    // ITS IN THIS SCRIPT BECAUSE THIS IS THE ONLY SCRIPT ON THE ANIMATOR GO
    public void PlaySwingAudio()
    {
        AkSoundEngine.PostEvent("Play_Swing", gameObject);
    }
}
