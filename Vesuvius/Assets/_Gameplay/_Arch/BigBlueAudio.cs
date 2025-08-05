namespace _Gameplay._Arch
{
    public class BigBlueAudio : GenericAudioScript
    {
        public void PlayBigBlueSolo()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            AkSoundEngine.PostEvent("Play_BigBlueSolo", gameObject);
        }

        public void StopBigBlueSolo() {
            AkSoundEngine.PostEvent("Stop_BigBlueSolo", gameObject);
        }
    }
}
