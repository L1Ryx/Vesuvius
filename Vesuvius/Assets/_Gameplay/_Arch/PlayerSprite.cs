using Events._Arch;
using UnityEngine;
using UnityEngine.Events;

namespace _Gameplay._Arch
{
    public class PlayerSprite : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent playerStartedHeal;
        public UnityEvent playerHealed;

        [Header("Data Cubes")]
        public SpawnData spawnData;

        public void PlayerStartedHeal() {
            playerStartedHeal.Invoke();
        }

        public void PlayerHealed() {
            playerHealed.Invoke();
        }

        public void UpdateFacingDirection() {
            spawnData.isFacingLeft = this.GetComponent<SpriteRenderer>().flipX;
        }
    }
}
