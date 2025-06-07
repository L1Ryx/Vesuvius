using UnityEngine;
using AK.Wwise;

public class VineWallAudio : MonoBehaviour
{
    public void PlayVineBreak()
    {
        AkSoundEngine.PostEvent("Play_VineBreak", gameObject);
    }
}
