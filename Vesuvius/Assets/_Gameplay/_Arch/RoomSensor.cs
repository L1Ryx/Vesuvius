using UnityEngine;
using UnityEngine.Events;

namespace _Gameplay._Arch
{
    public class RoomSensor : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private UnityEvent roomLoaded;
    
        void Start() {
            roomLoaded.Invoke();
        }
    }
} 
