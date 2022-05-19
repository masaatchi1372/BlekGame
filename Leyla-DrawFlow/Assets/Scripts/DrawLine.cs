using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    // The line prefab that we're going to be using in order to instantiate the user drawn path
    public GameObject linePrefab;

    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider2D;
    private float lastTime = 0f;
    private float lastDrawnTime;
    private List<Vector2> inputPositions = new List<Vector2>(); // points of the drawing line
    private List<float> timeIntervals = new List<float>(); // time interval between each point    
    Vector2 previousPoint; // we keep last point position to know where to put the next point on our line in continueLineFlow function


    void Update()
    {
        // first check for Inputs
        if (Input.GetMouseButtonDown(0))
        {
            // if we have a line we should destroy it and start creating the new one
            if (currentLine != null)
            {
                DestroyLine();
            }
            // Initiating the line
            CreateLine();
        }

        if (Input.GetMouseButton(0))
        {
            // caching each Input drag
            Vector2 tempInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempInput, inputPositions[inputPositions.Count - 1]) > 0.1f)
            {
                UpdateLine(tempInput);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastDrawnTime = Time.realtimeSinceStartup;
        }

        // if there's already a line and we don't have any input, we should continue the line flow
        if (currentLine != null && !Input.anyKey && Time.realtimeSinceStartup - lastDrawnTime > timeIntervals[0])
        {            
            ContinueLineFlow();
        }
    }

    // Starting line creation process
    private void CreateLine()
    {
        // initiating current line with our prefab with no vertex on its lineRenderer
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        // getting components
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider2D = currentLine.GetComponent<EdgeCollider2D>();

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
    private void UpdateLine(Vector2 newInput)
    {
        // adding the new input into our inputPositions array
        inputPositions.Add(newInput);

        // adding the interval time from our last point
        timeIntervals.Add(Time.realtimeSinceStartup - lastTime);
        lastTime = Time.realtimeSinceStartup;

        // we have a new member for our lineRenderer
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newInput);

        // updating edgeCollider on out LinePrefab
        edgeCollider2D.points = inputPositions.ToArray();
    }

    // based on the delta positions and times between each vertex in InputPosition we will continue its path
    private void ContinueLineFlow()
    {
        // if there's only one point then it means we shouldn't continue the flow and destroy the line
        if (inputPositions.Count == 1)
        {
            DestroyLine();
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
        timeIntervals.Add(Time.realtimeSinceStartup - lastDrawnTime);

        // removing the first point time interval after moving it
        timeIntervals.RemoveAt(0);

        // setting the last drawn time to current
        lastDrawnTime = Time.realtimeSinceStartup;

        // keeping the first point position for next step
        previousPoint = firstLinePoint;


        // updating LineRenderer and edgeCollider on out LinePrefab
        for (int i = 0; i < inputPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, inputPositions[i]);
        }
        edgeCollider2D.points = inputPositions.ToArray();

        // if there's still one point of the line which user can see (it's in the screen) we should continue the line flow
        if (!HasAtLeaseOnePointInScreen())
        {
            // if all points are outside of the screen (meaning we don't have any valid points) , we'll remove the line
            DestroyLine();
        }
    }

    // check whether we have a point in screen or not
    private bool HasAtLeaseOnePointInScreen()
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

    // destroying our line after the user Input has ended
    private void DestroyLine()
    {
        Destroy(currentLine);
        currentLine = null;
    }
}
