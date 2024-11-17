using UnityEngine;
using UnityEngine.UI;

public class TotemStatusEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image totemImage; // The image component of the totem
    [SerializeField] private ParticleSystem totemParticleSystem; // The particle system child
    [SerializeField] private PlayerInfo playerInfo; // Reference to PlayerInfo ScriptableObject

    [Header("Grayscale Settings")]
    [SerializeField] private Color colorWhenEnough = Color.white; // Original color
    [SerializeField] private Color colorWhenNotEnough = Color.gray; // Grayscale color

    private void Update()
    {
        // Check if the player has enough totems
        bool hasEnoughTotems = playerInfo.GetTotemPower() >= playerInfo.GetAbilityCost();

        // Toggle image color based on totem status
        totemImage.color = hasEnoughTotems ? colorWhenEnough : colorWhenNotEnough;

        // Toggle particle system based on totem status
        if (hasEnoughTotems)
        {
            if (!totemParticleSystem.isPlaying)
                totemParticleSystem.Play();
        }
        else
        {
            if (totemParticleSystem.isPlaying)
                totemParticleSystem.Stop();
        }
    }
}
