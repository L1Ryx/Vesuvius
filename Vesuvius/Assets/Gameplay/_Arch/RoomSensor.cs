using UnityEngine;
using UnityEngine.Events;

public class RoomSensor : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent roomLoaded;
    
    void Start() {
        roomLoaded.Invoke();
    }
} 
