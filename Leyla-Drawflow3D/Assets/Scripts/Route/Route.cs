using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public float speed = 0.5f;
    public GameObject followee;
    private ObjectBehaviour followeeBehaviour;

    private Transform[] controlPoints;
    private Dictionary<Vector3, Vector3> cachePositions;
    private int degree;
    private float tInBezierCurveFormula = 0f;
    private bool coroutineAllowed;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void Start()
    {
        cachePositions = new Dictionary<Vector3, Vector3>();
        if (followee != null)
        {
            followeeBehaviour = followee.GetComponent<ObjectBehaviour>() as BugBehaviour;
        }

        coroutineAllowed = true;

        degree = transform.childCount;
        controlPoints = new Transform[degree];
        if (degree > 1)
        {
            // populating the points to control points array
            for (int i = 0; i < degree; i++)
            {
                controlPoints[i] = transform.GetChild(i);
            }
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (coroutineAllowed && followee != null)
        {
            StartCoroutine(GoByTheRoute());
        }
        else
        {
            StopCoroutine(GoByTheRoute());
        }
    }

    private IEnumerator GoByTheRoute()
    {
        coroutineAllowed = false;

        while (tInBezierCurveFormula <= 1)
        {
            tInBezierCurveFormula += Time.deltaTime * speed;
            // calculate based on the time of the system, where should the followee be now
            if (followee != null && followeeBehaviour != null)
            {
                Vector3 newPosition = BezierCurvePointPosition(tInBezierCurveFormula, 0, degree - 1);

                // tell followee to Move
                followeeBehaviour.Move(newPosition);
            }

            yield return new WaitForEndOfFrame();
        }

        tInBezierCurveFormula = 0f;
        coroutineAllowed = true;
    }

    private Vector3 BezierCurvePointPosition(float t, int startingPoint, int endingPoint)
    {
        if (endingPoint == 0)
        {
            return controlPoints[0].position;
        }
        else if (startingPoint == endingPoint)
        {
            return controlPoints[endingPoint].position;
        }
        else
        {            
            return (1 - t) * BezierCurvePointPosition(t, startingPoint, endingPoint - 1) + t * BezierCurvePointPosition(t, startingPoint + 1, endingPoint);
        }
    }

    #region GIZMO
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (followee != null && degree > 0)
        {
            followee.transform.position = controlPoints[0].position;
        }
    }

    private void OnDrawGizmos()
    {
        // we get the number of control points (they are the children objects)
        degree = transform.childCount;
        controlPoints = new Transform[degree];
        if (degree < 1)
        {
            return;
        }

        // populating the points to control points array
        for (int i = 0; i < degree; i++)
        {
            controlPoints[i] = transform.GetChild(i);
        }

        // draw sphere on bezier curve
        Vector3 pointPosition = Vector3.zero;
        if (degree == 1)
        {
            pointPosition = controlPoints[degree - 1].position;
            Gizmos.DrawSphere(pointPosition, 0.01f);
        }
        else
        {

            for (float t = 0; t <= 1; t += 0.05f)
            {
                pointPosition = BezierCurvePointPosition(t, 0, degree - 1);
                Gizmos.DrawSphere(pointPosition, 0.1f);
            }

        }

        // draw raw line between control points
        for (int i = 0; i < degree - 1; i++)
        {
            Gizmos.DrawLine(new Vector2(controlPoints[i].position.x, controlPoints[i].position.y), new Vector2(controlPoints[i + 1].position.x, controlPoints[i + 1].position.y));
        }

    }

#endif
    #endregion
}