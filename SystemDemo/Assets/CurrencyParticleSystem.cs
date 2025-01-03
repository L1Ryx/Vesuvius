using UnityEngine;

public class CurrencyParticleSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float trackSpeed = 8f;
    private ParticleSystem particleSystem;
    private Transform playerTransform;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void EmitCurrencyParticles()
    {
        if (playerTransform == null || particleSystem == null) return;

        // Emit the particles
        particleSystem.Play();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to emit particles
        {
            EmitCurrencyParticles();
        }

        if (particleSystem == null || playerTransform == null) return;

        // Update the particle velocities to track the player
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int numParticles = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            if (particles[i].remainingLifetime > 0) // Ensure the particle is still alive
            {
                Vector3 direction = (playerTransform.position - particles[i].position).normalized;
                particles[i].velocity = direction * trackSpeed; // Adjust speed for faster tracking
            }
        }

        particleSystem.SetParticles(particles, numParticles);
    }
}
