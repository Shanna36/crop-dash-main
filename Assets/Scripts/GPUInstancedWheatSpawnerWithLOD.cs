using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancedWheatSpawnerWithLOD : MonoBehaviour
{
    public GameObject wheatPrefab;
    public Terrain terrain;
    public int numberOfWheat = 600;
    public float[] lodDistances = new float[] { 10f, 20f, 30f }; // Adjust these distances as needed

    private List<Matrix4x4>[] matricesPerLOD;
    private List<MaterialPropertyBlock> propertyBlocksPerLOD;
    private LODGroup lodGroup;
    private Mesh[] lodMeshes;
    private Material[] lodMaterials;

    void Start()
    {
        lodGroup = wheatPrefab.GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            Debug.LogError("Wheat prefab must have a LODGroup component");
            return;
        }

        LOD[] lods = lodGroup.GetLODs();
        int lodCount = lods.Length;

        lodMeshes = new Mesh[lodCount];
        lodMaterials = new Material[lodCount];
        matricesPerLOD = new List<Matrix4x4>[lodCount];
        propertyBlocksPerLOD = new List<MaterialPropertyBlock>(lodCount);

        for (int i = 0; i < lodCount; i++)
        {
            Renderer renderer = lods[i].renderers[0];
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                Debug.LogError($"LOD {i} must have a MeshFilter component");
                return;
            }

            lodMeshes[i] = meshFilter.sharedMesh;
            lodMaterials[i] = renderer.sharedMaterial;
            lodMaterials[i].enableInstancing = true;

            matricesPerLOD[i] = new List<Matrix4x4>();
            propertyBlocksPerLOD.Add(new MaterialPropertyBlock());
        }

        SpawnWheat();
    }

    void SpawnWheat()
    {
        float fieldStartX = -290;
        float fieldEndX = -189;
        float fieldStartZ = 100;
        float fieldEndZ = 200;

        float fieldAreaWidth = fieldEndX - fieldStartX;
        float fieldAreaDepth = fieldEndZ - fieldStartZ;

        int maxRows = Mathf.FloorToInt(fieldAreaDepth);
        int maxColumns = Mathf.FloorToInt(fieldAreaWidth);

        float rowDistance = fieldAreaDepth / maxRows;
        float wheatSpacing = fieldAreaWidth / maxColumns;

        int index = 0;
        for (int rowIndex = 0; rowIndex < maxRows && index < numberOfWheat; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < maxColumns && index < numberOfWheat; columnIndex++)
            {
                float x = fieldStartX + columnIndex * wheatSpacing;
                float z = fieldStartZ + rowIndex * rowDistance;

                float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;

                Vector3 position = new Vector3(x, y, z);
                Vector3 normal = terrain.terrainData.GetInterpolatedNormal(
                    (x - fieldStartX) / fieldAreaWidth,
                    (z - fieldStartZ) / fieldAreaDepth
                );

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(10f, 0f, 0f);

                Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

                // Add the matrix to all LOD levels
                for (int i = 0; i < matricesPerLOD.Length; i++)
                {
                    matricesPerLOD[i].Add(matrix);
                }

                index++;
            }
        }
    }

    void Update()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 cameraPosition = mainCamera.transform.position;

        for (int lodIndex = 0; lodIndex < matricesPerLOD.Length; lodIndex++)
        {
            List<Matrix4x4> visibleMatrices = new List<Matrix4x4>();

            foreach (Matrix4x4 matrix in matricesPerLOD[lodIndex])
            {
                float distance = Vector3.Distance(cameraPosition, matrix.GetColumn(3));

                if (lodIndex == 0 && distance <= lodDistances[0])
                {
                    visibleMatrices.Add(matrix);
                }
                else if (lodIndex == matricesPerLOD.Length - 1 && distance > lodDistances[lodIndex - 1])
                {
                    visibleMatrices.Add(matrix);
                }
                else if (lodIndex > 0 && distance > lodDistances[lodIndex - 1] && distance <= lodDistances[lodIndex])
                {
                    visibleMatrices.Add(matrix);
                }
            }

            // Draw visible instances for this LOD level
            if (visibleMatrices.Count > 0)
            {
                Graphics.DrawMeshInstanced(lodMeshes[lodIndex], 0, lodMaterials[lodIndex], visibleMatrices, propertyBlocksPerLOD[lodIndex]);
            }
        }
    }
}