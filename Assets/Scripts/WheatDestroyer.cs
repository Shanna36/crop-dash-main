using System.Collections.Generic;
using UnityEngine;

public class WheatDestroyer : MonoBehaviour
{
    public SimpleGPUInstance wheatSpawner; // Reference to the wheat spawner script
    private Collider combineCollider;      // Reference to the combine harvester's header collider

    private List<List<Matrix4x4>> batches;

    void Start()
    {
        // Automatically get the collider from the current GameObject
        combineCollider = GetComponent<Collider>();

        if (combineCollider == null)
        {
            Debug.LogError("No Collider found on the Combine Harvester. Make sure the script is attached to the object with the Capsule Collider.");
        }

        // Get a reference to the batches from the wheat spawner
        batches = wheatSpawner.Batches;
    }

    void Update()
    {
        // Loop through the wheat matrices and check for collisions
        for (int batchIndex = 0; batchIndex < batches.Count; batchIndex++)
        {
            List<Matrix4x4> batch = batches[batchIndex];

            for (int i = batch.Count - 1; i >= 0; i--) // Iterate backwards so removal doesn't mess up the indexing
            {
                Vector3 wheatPosition = batch[i].GetColumn(3); // Get the position from the matrix
                Bounds wheatBounds = new Bounds(wheatPosition, new Vector3(1f, 1f, 1f)); // Approximate bounds for wheat
                
                // Check if the combine harvester's collider intersects the wheat
                if (combineCollider.bounds.Intersects(wheatBounds))
                {
                    // Remove the wheat from the batch
                    batch.RemoveAt(i);

                    // Optionally: Trigger particle effects here when wheat is destroyed
                }
            }
        }
    }
}
