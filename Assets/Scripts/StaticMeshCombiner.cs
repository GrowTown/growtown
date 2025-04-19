using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StaticMeshCombiner : MonoBehaviour
{
    [Header("Combine Settings")]
    [Tooltip("Combine meshes at start? Otherwise call CombineMeshes() manually.")]
    public bool combineOnStart = true;

    [Tooltip("Only combine children with Static tag.")]
    public bool onlyStaticTagged = true;

    [Tooltip("Parent containing all static objects to combine.")]
    public Transform targetParent;

    void Start()
    {
        if (combineOnStart)
        {
            CombineMeshes();
        }
    }

    [ContextMenu("Combine Meshes Now")]
    public void CombineMeshes()
    {
        if (targetParent == null)
        {
            Debug.LogWarning("StaticMeshCombiner: Target parent not assigned.");
            return;
        }

        Dictionary<Material, List<CombineInstance>> materialToMeshList = new();

        MeshFilter[] meshFilters = targetParent.GetComponentsInChildren<MeshFilter>();
        int skipped = 0;

        foreach (var mf in meshFilters)
        {
            if (mf.transform == this.transform) continue;

            if (onlyStaticTagged && !mf.gameObject.isStatic) continue;

            Renderer renderer = mf.GetComponent<Renderer>();
            if (!renderer || renderer.sharedMaterial == null) continue;

            Material mat = renderer.sharedMaterial;

            if (!materialToMeshList.ContainsKey(mat))
                materialToMeshList[mat] = new List<CombineInstance>();

            CombineInstance ci = new()
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };

            materialToMeshList[mat].Add(ci);

            skipped++;
            mf.gameObject.SetActive(false); // Optionally disable original
        }

        int materialIndex = 0;
        foreach (var pair in materialToMeshList)
        {
            GameObject combinedObject = new($"Combined_Mesh_{materialIndex}");
            combinedObject.transform.SetParent(transform, false);
            combinedObject.isStatic = true;

            Mesh combinedMesh = new();
            combinedMesh.CombineMeshes(pair.Value.ToArray(), true, true);

            MeshFilter mf = combinedObject.AddComponent<MeshFilter>();
            mf.sharedMesh = combinedMesh;

            MeshRenderer mr = combinedObject.AddComponent<MeshRenderer>();
            mr.sharedMaterial = pair.Key;

            materialIndex++;
        }

        Debug.Log($"StaticMeshCombiner: Combined {skipped} meshes into {materialIndex} grouped objects.");
    }
}
