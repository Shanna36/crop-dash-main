using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for working with UI elements

public class WheatDestroyer : MonoBehaviour
{
    public SimpleGPUInstance wheatSpawner; // Reference to the wheat spawner script
    private Collider combineCollider;      // Reference to the combine harvester's header collider
    public Slider fillBar;                 // Reference to the UI Slider fill bar
    private List<List<Matrix4x4>> batches;
    private int totalWheatCount;           // Total number of wheat instances at the start
    private int remainingWheatCount;       // Track remaining wheat

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

        // Initialize the wheat count based on the total number of wheat instances
        totalWheatCount = 0;
        foreach (var batch in batches)
        {
            totalWheatCount += batch.Count;
        }

        remainingWheatCount = totalWheatCount;

        // Ensure the fill bar starts at 0%
        UpdateFillBar();
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

                    // Decrease the remaining wheat count and update the fill bar
                    remainingWheatCount--;
                    UpdateFillBar();
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

    // Update the fill bar based on the remaining wheat count
    private void UpdateFillBar()
    {
        // Slider value is between 0 and 1
        float progress = 1 - ((float)remainingWheatCount / totalWheatCount);
        fillBar.value = progress;
    }
}
