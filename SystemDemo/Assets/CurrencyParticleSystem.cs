using UnityEngine;

public class CurrencyParticleSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float trackSpeedMin = 6f; // Minimum speed for particle tracking
    [SerializeField] private float trackSpeedMax = 10f; // Maximum speed for particle tracking
    [SerializeField] private float particleSystemLifetime = 3f; // Time in seconds before the system destroys itself
    [SerializeField] public int particlesEmitted = 5;

    private ParticleSystem particleSystem;
    private Transform playerTransform;
    private float destructionTimer = 0f;
    private bool hasEmittedParticles = false;

    public int SetParticlesEmitted(int num) {
        particlesEmitted = num;
        return num;
    }

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        EmitCurrencyParticles(particlesEmitted);
        
    }

    public void EmitCurrencyParticles(int particleCount)
    {
        if (playerTransform == null || particleSystem == null) return;

        // Configure burst emission
        var emission = particleSystem.emission;
        var burst = emission.GetBurst(0); // Assume there’s only one burst
        burst.count = particleCount; // Set the burst count to the specified amount
        emission.SetBurst(0, burst);

        // Emit the particles
        particleSystem.Play();
        hasEmittedParticles = true; // Mark the system as active
        destructionTimer = particleSystemLifetime; // Set the timer for destruction
    }

    private void LateUpdate()
    {
        // Example: Test emission by pressing 'E'
        // if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to emit particles
        // {
        //     EmitCurrencyParticles(10); // Emit 10 particles as a test
        // }

        if (particleSystem == null || playerTransform == null) return;

        // Update the particle velocities to track the player
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int numParticles = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            if (particles[i].remainingLifetime > 0) // Ensure the particle is still alive
            {
                Vector3 direction = (playerTransform.position - particles[i].position).normalized;

                // Randomize track speed for each particle
                float randomTrackSpeed = Random.Range(trackSpeedMin, trackSpeedMax);
                particles[i].velocity = direction * randomTrackSpeed; // Adjust speed for faster tracking
            }
        }

        particleSystem.SetParticles(particles, numParticles);

        // Self-destruction after the specified lifetime
        if (hasEmittedParticles)
        {
            destructionTimer -= Time.deltaTime;
            if (destructionTimer <= 0f)
            {
                Destroy(gameObject); // Destroy the particle system GameObject
            }
        }
    }
}
