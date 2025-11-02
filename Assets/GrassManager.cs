using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class GrassManager : MonoBehaviour
{
    [Header("Grass Setup")]
    public Mesh grassMesh;                 // Simple quad or grass mesh
    public Material grassMaterial;         // Material with GPU instancing enabled
    public int grassCount = 5000;          // Number of grass blades
    public Vector2 areaSize = new Vector2(50, 50);

    [Header("Wind Settings")]
    public float windStrength = 0.2f;
    public float windSpeed = 1.5f;

    [Header("Performance")]
    public float cullDistance = 60f;       // Only draw grass near the camera

    private List<Matrix4x4[]> batches = new List<Matrix4x4[]>();
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        GenerateGrass();
    }

    void GenerateGrass()
    {
        batches.Clear();
        List<Matrix4x4> matrices = new List<Matrix4x4>();

        for (int i = 0; i < grassCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                0f,
                Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f)
            );

            Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            float scale = Random.Range(0.8f, 1.2f);

            matrices.Add(Matrix4x4.TRS(pos, rot, Vector3.one * scale));

            if (matrices.Count == 1023) // Unity's limit
            {
                batches.Add(matrices.ToArray());
                matrices.Clear();
            }
        }

        if (matrices.Count > 0)
            batches.Add(matrices.ToArray());
    }

    void Update()
    {
        if (grassMaterial == null || grassMesh == null) return;

        // Send wind data to shader
        grassMaterial.SetFloat("_WindStrength", windStrength);
        grassMaterial.SetFloat("_WindSpeed", windSpeed);
        grassMaterial.SetFloat("_TimeOffset", Time.time);

        Vector3 camPos = mainCam.transform.position;

        foreach (var batch in batches)
        {
            // Simple culling: skip if far away
            Vector3 pos = batch[0].GetColumn(3); 
            if ((pos - camPos).sqrMagnitude > cullDistance * cullDistance)
                continue;

            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, batch);
        }
    }
}

