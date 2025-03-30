using _ScriptableObjects.PlayerInfo;
using UnityEngine;

namespace _Gameplay._Arch
{
    public class FinishDoor : MonoBehaviour
    {
        [Header("Data Cubes")]
        [SerializeField] private PlayerInfo playerInfo;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInfo.hasCompletedDemo = true;
            }
        }
    }
}
