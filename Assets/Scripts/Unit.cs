using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;
    public GameObject arrowPrefab; // Assign in Inspector
    Transform _target;
    public float speed = 20;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 0.2f;
    static internal bool isPathRequested = false;

    PathG path;
    Vector3 lastRequestedPosition;
    Coroutine pathUpdateCoroutine;

    public Transform Target
    {
        get => _target;
        set
        {
            _target = value;
            if(value != null)
            RequestImmediatePath();
        }
    }

    void Start()
    {
       // StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful && waypoints != null && waypoints.Length > 1)
        {
            path = new PathG(waypoints, transform.position, turnDst, stoppingDst);

            foreach (var arrow in GameObject.FindGameObjectsWithTag("Arrow"))
            {
                Destroy(arrow);
            }

            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Vector3 dir = (waypoints[i + 1] - waypoints[i]).normalized;

                // Custom rotation with Y = 90
                Quaternion rot = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90f, 0);

                // Custom position with Y = 0.5
                Vector3 pos = new Vector3(waypoints[i].x, 0.2f, waypoints[i].z);

                GameObject arrow = Instantiate(arrowPrefab, pos, rot);
                arrow.tag = "Arrow";
            }

        }
    }

    void RequestImmediatePath()
    {
        if (_target == null) return;

        StopExistingPathCoroutine();

        PathRequestManager.RequestPath(
            new PathRequest(transform.position, _target.position, OnPathFound)
        );
        lastRequestedPosition = transform.position;

        pathUpdateCoroutine = StartCoroutine(UpdatePath());
    }

    void StopExistingPathCoroutine()
    {
        if (pathUpdateCoroutine != null)
        {
            StopCoroutine(pathUpdateCoroutine);
            pathUpdateCoroutine = null;
        }
    }

    IEnumerator UpdatePath()
    {
        float sqrThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);

            if (Vector3.Distance(transform.position, Target.position) < stoppingDst)
            {
                UI_Manager.Instance.MapHandler.OnClickCancelPath();
                yield break;
            }
            if ((transform.position - lastRequestedPosition).sqrMagnitude > sqrThreshold)
            {
                PathRequestManager.RequestPath(
                    new PathRequest(transform.position, _target.position, OnPathFound)
                );
                lastRequestedPosition = transform.position;
            }
        }
    }

    public void CancelPath()
    {
        StopExistingPathCoroutine();
        _target = null;
        path = null;

        foreach (var arrow in GameObject.FindGameObjectsWithTag("Arrow"))
        {
            Destroy(arrow);
        }

        Debug.Log("Path canceled.");
    }

    /* IEnumerator UpdatePath()
     {
         if (isPathRequested)
         {

             PathRequestManager.RequestPath(new PathRequest(transform.position, Target.position, OnPathFound));

             float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;

             Vector3 startPosOld = transform.position;

             while (true)
             {
                 yield return new WaitForSeconds(minPathUpdateTime);
                 if ((transform.position - startPosOld).sqrMagnitude > sqrMoveThreshold)
                 {
                     PathRequestManager.RequestPath(new PathRequest(transform.position, Target.position, OnPathFound));
                     startPosOld = transform.position;
                 }
             }

         }
     }*/


    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }




}


