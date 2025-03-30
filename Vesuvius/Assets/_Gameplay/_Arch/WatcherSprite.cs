using UnityEngine;
using UnityEngine.Events;

namespace _Gameplay._Arch
{
    public class WatcherSprite : MonoBehaviour
    {
        [Header("Refs")]
        public WatcherAI watcherAI;
        public UnityEvent watcherSwung;

        public void EndPokeFromSprite() {
            watcherAI.EndPoke();
        }
        public void WatcherSwung() {
            watcherSwung.Invoke();
        }
    }
}
