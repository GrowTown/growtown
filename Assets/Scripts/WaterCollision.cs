using UnityEngine;

public class WaterCollision : MonoBehaviour
{

    public Color hitColor = new Color(0.9098039f, 0.6431373f, 0.6431373f, 1f);
    [SerializeField]
    private ParticleSystem _particleSystem;

    private void OnParticleCollision(GameObject other)
    {

        TileInfo tile = other.GetComponent<TileInfo>();
        if (tile != null)
        {
            if (!tile._hasColorChanged)
            {
                if (tile.fieldGrid.isPlantStartGrowing)
                {
                    tile.GetComponentInParent<FieldGrid>().OnWaterTile(tile.gameObject);
                }

                tile.GetComponentInParent<FieldGrid>().AddCoveredTile(tile.gameObject);
          
                if (tile.GetComponentInParent<FieldGrid>().IsCoverageComplete())
                {
                    tile.GetComponentInParent<FieldGrid>().StopCoverageTracking();
                    Debug.Log("All Collected");
                    tile.GetComponentInParent<FieldGrid>().CompleteAction();
                }
                tile._hasColorChanged = true;
            }
        }
    }
}


