using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{

    [System.Serializable]
    public class MapLocation
    {
        public string name;
        public Transform target;
        public GameObject ring;
    }

    [Header("Map Locations")]
    public List<MapLocation> mapLocations;


    private GameObject currentSelectedRing = null;

    private Dictionary<string, MapLocation> locationMap;

    private void Awake()
    {
        locationMap = new Dictionary<string, MapLocation>();
        foreach (var loc in mapLocations)
        {
            locationMap[loc.name] = loc;
            if (loc.ring != null)
                loc.ring.SetActive(false);
        }
    }

    public void OnClickLocation(string locationName)
    {
        if (!locationMap.ContainsKey(locationName))
        {
            Debug.LogWarning($"Location '{locationName}' not found.");
            return;
        }

        MapLocation selected = locationMap[locationName];

        HighlightRing(selected.ring);
        GoTo(selected.target);
    }

    private void GoTo(Transform targetPos)
    {

        Unit.isPathRequested = true;
        //PathRequestManager.RequestPath(new PathRequest( UI_Manager.Instance.CharacterMovements.transform.position, targetPos.position, UI_Manager.Instance.Unit.OnPathFound));
        UI_Manager.Instance.Unit.Target = targetPos;
        UI_Manager.Instance.mapPanel.SetActive(false);
        if (UI_Manager.Instance.pathCancelPopUp.activeSelf == false)
        {
            UI_Manager.Instance.pathCancelPopUp.SetActive(true);
        }
    }

    private void HighlightRing(GameObject selectedRing)
    {
        if (currentSelectedRing == selectedRing)
            return;

        if (currentSelectedRing != null)
            currentSelectedRing.SetActive(false);

        selectedRing.SetActive(true);
        currentSelectedRing = selectedRing;
    }

    public void OnClickCancelPath()
    {
        UI_Manager.Instance.Unit.CancelPath();

  /*      if (UI_Manager.Instance.mapPanel.activeSelf == false)
        {
            UI_Manager.Instance.mapPanel.SetActive(true);
        }
*/

        if (currentSelectedRing != null)
        {
            currentSelectedRing.SetActive(false);
            currentSelectedRing = null;
        }
        UI_Manager.Instance.pathCancelPopUp.SetActive(false);
    }

}


