using System.Collections.Generic;
using UnityEngine;

public class WheatDestroyer : MonoBehaviour
{
    public SimpleGPUInstance wheatSpawner; // Reference to the wheat spawner script
    private Collider combineCollider;      // Reference to the combine harvester's header collider

    private List<List<Matrix4x4>> batches;

    // Add a reference to the particle prefab
    public GameObject wheatDestroyParticles; 

    void Start()
    {
        combineCollider = GetComponent<Collider>();

        if (combineCollider == null)
        {
            Debug.LogError("No Collider found on the Combine Harvester. Make sure the script is attached to the object with the Capsule Collider.");
        }

        batches = wheatSpawner.Batches;
    }

    void Update()
    {
        for (int batchIndex = 0; batchIndex < batches.Count; batchIndex++)
        {
            List<Matrix4x4> batch = batches[batchIndex];

            for (int i = batch.Count - 1; i >= 0; i--)
            {
                Vector3 wheatPosition = batch[i].GetColumn(3);
                Bounds wheatBounds = new Bounds(wheatPosition, new Vector3(1f, 1f, 1f));

                if (combineCollider.bounds.Intersects(wheatBounds))
                {
                    // Remove the wheat from the batch
                    batch.RemoveAt(i);

                    // Trigger particle effects when wheat is destroyed
                    TriggerParticleEffect(wheatPosition);
                }
            }
        }
    }

    private void TriggerParticleEffect(Vector3 position)
    {  
        // Instantiate the particle system at the position of the wheat destroyed
        GameObject particles = Instantiate(wheatDestroyParticles, position, Quaternion.identity);
        
        
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startSpeed = 2; // Set the speed for upward motion
            main.startLifetime = 3; // Ensure lifetime matches the fade duration
        }

        // Destroy the particle system after its lifetime
        Destroy(particles, 3f);
    }
}
