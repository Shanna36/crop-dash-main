using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGPUInstance : MonoBehaviour
{
    public int Instances = 1000; // Increase the number of instances
    public Mesh mesh;
    public Material[] Materials; // Ensure they are GPU-enabled
    public Terrain terrain; // Reference to the terrain

    // Field constraints
    public float fieldStartX = -300;
    public float fieldEndX = -185;
    public float fieldStartZ = 50;
    public float fieldEndZ = 200;

    public List<List<Matrix4x4>> Batches = new List<List<Matrix4x4>>(); // Batches of 1,023

    private void RenderBatches()
    {
        foreach (List<Matrix4x4> Batch in Batches)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, Materials[i], Batch);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Calculate the field's width and depth
        float fieldAreaWidth = fieldEndX - fieldStartX;
        float fieldAreaDepth = fieldEndZ - fieldStartZ;

        // Number of rows and columns
        int maxRows = Mathf.CeilToInt(Mathf.Sqrt(Instances)); // Adjust to your preferred logic
        int maxColumns = Mathf.CeilToInt(Mathf.Sqrt(Instances)); // Adjust to your preferred logic

        // Calculate the distance between each wheat in the grid (reduce this value for closer spacing)
        float rowDistance = fieldAreaDepth / maxRows * 0.75f; //distance between instanced meshes.
        float columnDistance = fieldAreaWidth / maxColumns * 0.75f; 

        int AddedMatrices = 0;
        Batches.Add(new List<Matrix4x4>());

        // Loop to create and distribute wheat instances
        for (int rowIndex = 0; rowIndex < maxRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < maxColumns; columnIndex++)
            {
                // Calculate the x and z positions based on the grid
                float x = fieldStartX + columnIndex * columnDistance;
                float z = fieldStartZ + rowIndex * rowDistance;

                // Sample the terrain height for the y-coordinate
                float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;

               

                // Create the transformation matrix with the desired scale
                Matrix4x4 matrix = Matrix4x4.TRS(
                    new Vector3(x, y, z),                     // position
                    Quaternion.identity,                       // rotation
                    new Vector3(6.0f, 6.0f, 6.0f)          // scale (adjust as necessary)
                );

                // Add to the current batch
                if (AddedMatrices < 1000)
                {
                    Batches[Batches.Count - 1].Add(matrix);
                    AddedMatrices++;
                }
                else
                {
                    Batches.Add(new List<Matrix4x4>());
                    Batches[Batches.Count - 1].Add(matrix);
                    AddedMatrices = 1;
                }

                // Stop if we reach the desired number of instances
                if (AddedMatrices >= Instances) break;
            }
            if (AddedMatrices >= Instances) break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderBatches();
    }
}
