using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public Transform[] spawnPoints;
    internal bool seedsSpawned = false;
    internal bool plantSpawned = false;
    internal bool plantgrowth = false;
    internal bool isCuttingStarted = false;
    private MeshRenderer _renderer;
    internal bool _hasColorChanged = false;
    private MaterialPropertyBlock _propBlock;
    private float brushRadius = 0.1f; // Adjust the area of effect
    private Texture2D texture;
    public LayerMask layerMask;
    public Color paintColor = Color.gray; // The color to apply
    public Texture2D baseTexture; // Assign the base texture in Inspector
    private Texture2D paintableTexture;
    internal bool isTileHasSeed = false;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();

        /* // Create a copy of the base texture manually
         paintableTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGB24, false);

         // Copy pixel data manually to avoid mipmap mismatch
         paintableTexture.SetPixels(baseTexture.GetPixels());
         paintableTexture.Apply(); // Apply chang

         _renderer.material.SetTexture("_BaseMap", paintableTexture);*/
    }
    public void OnPlayerEnter()
    {
        if (!seedsSpawned)
        {
            if (!GameManager.Instance.HasEnoughPoints(5, 0)) return;
            SpawnSeeds();
            seedsSpawned = true;
            GameManager.Instance.DeductEnergyPoints(5);
            UI_Manager.Instance.PlayerXp.SuperXp(2);
            GameManager.Instance.ForCropSeedDEduction();
        }
    }

    private void SpawnSeeds()
    {
        // Spawn a seed at each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            var instace = Instantiate(UI_Manager.Instance.seed, spawnPoint.position, Quaternion.identity);
            UI_Manager.Instance.spawnedSeed.Add(instace);
        }
    }

    /* internal void SpawnPlant(GameObject tilego)
     {
         if(!plantSpawned)
         {
             foreach (Transform spawnPoint in spawnPoints)
             {
                 var instance = Instantiate(UI_Manager.Instance.plantHolder, spawnPoint.position, Quaternion.identity);
                 UI_Manager.Instance.spawnPlantsForGrowth.Add(tilego,instance);
             }
             plantSpawned = true;
         }

     }*/

    internal void SpawnPlant(GameObject tilego)
    {
        if (!plantSpawned)
        {
            if (!UI_Manager.Instance.spawnPlantsForGrowth.ContainsKey(tilego))
            {
                UI_Manager.Instance.spawnPlantsForGrowth[tilego] = new List<GameObject>();
            }

            Quaternion[] rotations = new Quaternion[]
            {
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(0, 45, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 270, 0)
            };

            int index = 0;
            foreach (Transform spawnPoint in spawnPoints)
            {
                // Use the index to assign a rotation
                var rotation = rotations[index % rotations.Length];

                // Instantiate the plant with the specific rotation
                var instance = Instantiate(UI_Manager.Instance.plantHolder, spawnPoint.position, rotation);

                // Add the instance to the respective lists
                UI_Manager.Instance.spawnPlantsForGrowth[tilego].Add(instance);
                UI_Manager.Instance.spawnPlantsForInitialGrowth.Add(instance);

                if (!UI_Manager.Instance.GrownPlantsToCut.ContainsKey(UI_Manager.Instance.FieldManager.CurrentFieldID))
                {
                    UI_Manager.Instance.GrownPlantsToCut[UI_Manager.Instance.FieldManager.CurrentFieldID] = new List<GameObject>();
                }

                UI_Manager.Instance.GrownPlantsToCut[UI_Manager.Instance.FieldManager.CurrentFieldID].Add(instance);

                // Increment the index for the next rotation
                index++;
            }

            plantSpawned = true;
        }
    }

    public void ChangeColor(Color newColor)
    {
        if (_renderer != null)
        {
            StartCoroutine(ChangeColorOverTime(newColor));
        }
    }

    private IEnumerator ChangeColorOverTime(Color targetColor)
    {
        Color currentColor = _renderer.material.color;
        float duration = 1.0f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            _renderer.material.color = Color.Lerp(currentColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _renderer.material.color = targetColor;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tool"))
        {
            RaycastHit hit;
            if (Physics.Raycast(other.transform.position, Vector3.down, out hit, 4, layerMask))
            {
                UI_Manager.Instance.FieldGrid.AddCoveredTile(this.gameObject);
                /*Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= paintableTexture.width;
                pixelUV.y *= paintableTexture.height;

                Debug.Log($"Tile: {gameObject.name}, UV: {pixelUV}");

                ApplyBrush((int)pixelUV.x, (int)pixelUV.y);*/
            }
        }
        /*else if (other.CompareTag("Seed"))
        {
           *//* if (!isTileHasSeed)
            {
                UI_Manager.Instance.FieldGrid.AddCoveredTile(this.gameObject);
                OnThrowSeed(this.gameObject);
                isTileHasSeed = true;
            }

            Destroy(other.gameObject);*//*
        }*/

    }

    private void ApplyBrush(int x, int y)
    {
        int brushSize = 5;
        var color = new Color(1f, 0.9188f, 0.9188f, 1f);

        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (x + i >= 0 && x + i < paintableTexture.width && y + j >= 0 && y + j < paintableTexture.height)
                {
                    paintableTexture.SetPixel(x + i, y + j, color);
                }
            }
        }
        paintableTexture.Apply();
        _renderer.material.mainTexture = paintableTexture;

    }


    public void OnThrowSeed(GameObject coveredTile)
    {
        if (UI_Manager.Instance.seed != null && GameManager.Instance.isThroughingseeds)
        {
          
            if (coveredTile != null&& !isTileHasSeed)
            {
                UI_Manager.Instance.FieldGrid.AddCoveredTile(coveredTile);
                isTileHasSeed = true;
                OnPlayerEnter();
                Debug.Log("Seed spawned on tile");
                SpawnPlantWithDelay(coveredTile);
            }
            coveredTile = null;
        }
    }

    private void SpawnPlantWithDelay(GameObject tilego)
    {

        if (UI_Manager.Instance.plantHolder != null)
        {
            UI_Manager.Instance.isPlanted = true;
            SpawnPlant(tilego);
            Debug.Log("Plant spawned after delay");
        }

    }


}


