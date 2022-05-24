using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    #region HEADER LINE PROPERTIES
    [Space(10)]
    [Header("Line Properties")]
    #endregion
    [Tooltip("The preferred distance between each point on the line")]
    public float preferredPointsDistance = 0.1f;
    [Tooltip("How many points on the line will be visible. Set to Zero to draw all the line")]
    public int lineLengthInPoints = 10;

    [HideInInspector] public List<Vector2> inputPositions { get; set; }// points of the drawing line
    [HideInInspector] public List<float> timeIntervals { get; set; } // time interval between each point    
    [HideInInspector] public float lastDrawnTime { get; set; }
    [HideInInspector] public float lastTime = 0f;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider2D;
    private Vector2 previousPoint; // we keep last point position to know where to put the next point on our line in continueLineFlow function        

    private void Start()
    {

        inputPositions = new List<Vector2>();
        timeIntervals = new List<float>();

        // getting components
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();

        // we should clear the inputPosition & timeIntervals array
        inputPositions.Clear();
        timeIntervals.Clear();

        // we add the starting point of user input
        inputPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // set the time Interval to zero, so the line would be continue as soon as possible
        timeIntervals.Add(0f);
        lastTime = Time.realtimeSinceStartup;

        // we manually set lineRenderer first vertex position
        lineRenderer.SetPosition(0, inputPositions[0]);

        // setting previous point for the purpose of continueLineFlow
        previousPoint = inputPositions[0];

        // updating edgeCollider on out LinePrefab
        edgeCollider2D.points = inputPositions.ToArray();
    }

    // updating our lines positions and adding new points
    public void AddNewPoint(Vector2 newInput)
    {
        // adding the new input into our inputPositions array
        inputPositions.Add(newInput);

        // adding the interval time from our last point
        timeIntervals.Add(Time.realtimeSinceStartup - lastTime);
        lastTime = Time.realtimeSinceStartup;

        if (inputPositions.Count <= lineLengthInPoints || lineLengthInPoints == 0)
        {
            // we have a new member for our lineRenderer
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newInput);
        }
        else // we reached the line length so we should remove the last point and add the new one
        {
            for (int i = 1; i <= preferredPointsDistance; i++)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount - i, inputPositions[inputPositions.Count - i]);
            }
        }

        // update the line renderer and edge collider
        UpdateLineRenderer();
    }

    // based on the delta positions and times between each vertex in InputPosition we will continue its path
    public void ContinueLineFlow()
    {
        // if there's already a line and we don't have any input, we should continue the line flow
        if (!(Time.realtimeSinceStartup - lastDrawnTime > timeIntervals[0]))
        {
            return;
        }

        // putting our first point relatively after our last point in the inputPositions array to make the line continue the flow
        // each point will move according to the distance from its last point
        int linePoints = inputPositions.Count;
        Vector2 firstLinePoint = inputPositions[0];

        // adding the new point            
        inputPositions.Add(inputPositions[linePoints - 1] + (firstLinePoint - previousPoint));

        // removing the beginning point from array since it's been moved to the end of the line
        inputPositions.RemoveAt(0);

        // adding the delta time of the new drawn point
        timeIntervals.Add(timeIntervals[0]);

        // removing the first point time interval after moving it
        timeIntervals.RemoveAt(0);

        // setting the last drawn time to current
        lastDrawnTime = Time.realtimeSinceStartup;

        // keeping the first point position for next step
        previousPoint = firstLinePoint;

        // update the line renderer and edge collider
        UpdateLineRenderer();
    }

    // check whether we have a point in screen or not
    public bool HasAtLeaseOnePointInScreen()
    {
        // check whether all points are in screen
        for (int i = 0; i < inputPositions.Count; i++)
        {
            // check if the point is in the screen
            if (new Rect(0, 0, 1, 1).Contains(Camera.main.WorldToViewportPoint(inputPositions[i])))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAddNewPoint(Vector2 here)
    {
        if (inputPositions.Count > 0 && Vector2.Distance(here, inputPositions[inputPositions.Count - 1]) > preferredPointsDistance)
        {
            return true;
        }

        return false;
    }

    public void UpdateLineRenderer()
    {
        // updating LineRenderer on our LinePrefab
        // remember we just have to set position on the visible points not all the inputPositions
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, inputPositions[inputPositions.Count - lineRenderer.positionCount + i]);
        }

        // updating the edge collider with all the input positions
        edgeCollider2D.points = inputPositions.ToArray();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger, Tag:{other.gameObject.tag}");
        if (other.gameObject.tag == "Obstacle")
        {
            ObjectBehaviour behaviour;
            if (other.TryGetComponent<ObjectBehaviour>(out behaviour))
            {

            }
        }
    }
}
