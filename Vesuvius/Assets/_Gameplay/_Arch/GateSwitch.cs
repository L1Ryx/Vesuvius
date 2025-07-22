using _ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class GateSwitch : MonoBehaviour
    {
        [Header("Sprites")]
        public SpriteRenderer switchSpriteRenderer;
        public Sprite lockedSprite;
        public Sprite unlockedSprite;


        public void UpdateSwitchSprite()
        {
            if (switchSpriteRenderer == null || lockedSprite == null || unlockedSprite == null)
            {
                Debug.LogWarning("Switch sprite references are not set properly.");
                return;
            }

            switchSpriteRenderer.sprite =  unlockedSprite;
        }
    }
}
