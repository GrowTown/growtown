using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WaterRippleController : MonoBehaviour
{
    public Transform[] stones;
    public Material waterMat;

    void Update()
    {
        if (!waterMat || stones == null) return;

        int count = Mathf.Min(stones.Length, 10);
        waterMat.SetInt("_StoneCount", count);

        Vector4[] stonePositions = new Vector4[10];
        for (int i = 0; i < count; i++)
        {
            if (stones[i])
                stonePositions[i] = stones[i].position;
        }

        waterMat.SetVectorArray("_StonePositions", stonePositions);
    }
}
