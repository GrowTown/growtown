using UnityEngine;

public class WaterCollision : MonoBehaviour
{

    public Color hitColor = new Color(0.9098039f, 0.6431373f, 0.6431373f, 1f);
    [SerializeField]
    private ParticleSystem _particleSystem;

    private void Start()
    {
        //_particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        // Check if the object has the Tile script
        TileInfo tile = other.GetComponent<TileInfo>();
        if (tile != null)
        {
            if (!tile._hasColorChanged)
            {
                if (GameManager.Instance.isPlantStartGrowing)
                {
                    GameManager.Instance.OnWaterTile(tile.gameObject);
                }

                UI_Manager.Instance.FieldGrid.AddCoveredTile(tile.gameObject);
          
                if (UI_Manager.Instance.FieldGrid.IsCoverageComplete())
                {
                    UI_Manager.Instance.FieldGrid.StopCoverageTracking();
                    Debug.Log("All Collected");
                    UI_Manager.Instance.TriggerZoneCallBacks.CompleteAction();
                }
                tile._hasColorChanged = true;
            }
        }
    }
}


