using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour, IObjectPooled
{
    #region HEADER LINE PROPERTIES
    [Space(10)]
    [Header("Line Properties")]
    [Space(3)]
    #endregion
    [Tooltip("The prefab to put on the head of the line")]
    public GameObject lineHeadPrefab;
    [Tooltip("The preferred distance between each point on the line")]
    public float preferredPointsDistance = 0.1f;
    [Tooltip("How many points on the line will be visible. Set to Zero to draw all the line")]
    public int lineLengthInPoints = 10;
    [Tooltip("seconds interval to remove one point from the line when destroying it")]
    public float lineRemovalSpeed = 0.01f;
    [Tooltip("How fast should the line header rotate towards the direction of the line")]
    public float lineHeadRotationSpeed = 1f;
    [Tooltip("If line head rotatino is below this threshold, it won't change direction. if You choose low numbers, line header become more noisy")]

    [HideInInspector] public List<Vector3> inputPositions { get; set; }// points of the drawing line
    [HideInInspector] public List<float> timeIntervals { get; set; } // time interval between each point    
    [HideInInspector] public float lastDrawnTime { get; set; }
    [HideInInspector] public float lastTime = 0f;
    [HideInInspector] public float lastPointRemovalTime = 0f;
    [HideInInspector] public bool shouldDestroy; // determine if we should destroy the line

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider2D;
    private GameObject lineHead;
    private Vector3 previousPoint; // we keep last point position to know where to put the next point on our line in continueLineFlow function        
    private Camera mainCam;
    private Quaternion toRotation;


    public void OnSpawnObjectPooled()
    {
        // initiation
        if (inputPositions == null)
        {
            inputPositions = new List<Vector3>();
        }
        if (timeIntervals == null)
        {
            timeIntervals = new List<float>();
        }
        mainCam = Camera.main;

        // we should clear the inputPosition & timeIntervals array
        inputPositions.Clear();
        timeIntervals.Clear();

        // getting components
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();

        // we add the starting point of user input
        // we should set the zero of mouse input to avoid future issues
        Vector3 tempInput = Input.mousePosition;
        tempInput.z = 1f;
        inputPositions.Add(mainCam.ScreenToWorldPoint(tempInput));

        // set the time Interval to zero, so the line would be continue as soon as possible
        timeIntervals.Add(0f);
        lastTime = Time.realtimeSinceStartup;

        // we manually set lineRenderer first vertex position        
        lineRenderer.SetPosition(0, inputPositions[0]);

        // setting previous point for the purpose of continueLineFlow
        previousPoint = inputPositions[0];

        UpdateEdgeCollider();

        // creating the line head
        if (lineHeadPrefab != null && lineHead == null)
        {
            lineHead = Instantiate(lineHeadPrefab, inputPositions[0], Quaternion.identity, gameObject.transform);
        }
        else
        {
            // lineHead.SetActive(true);
        }

        // now we activate the edgecollider on the line
        edgeCollider2D.enabled = false;
    }

    public bool OnPoolingObject()
    {
        inputPositions.Clear();
        timeIntervals.Clear();
        lineRenderer.positionCount = 0;

        return false;
    }

    private void Update()
    {
        if (toRotation != null && lineHead != null)
        {
            lineHead.transform.rotation = Quaternion.RotateTowards(lineHead.transform.rotation, toRotation, lineHeadRotationSpeed * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        UpdateLineHeader();
    }

    // updating our lines positions and adding new points
    public void AddNewPoint(Vector3 newInput)
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

    // each this function is called, it'll remove one point from the beginning of the line
    public void RemoveFirstPoint()
    {
        // first we destroy our line header
        if (lineHead != null)
        {
            Destroy(lineHead);
        }

        if (inputPositions.Count <= lineRenderer.positionCount) // just remove one of the points that are rendered
        {
            inputPositions.RemoveAt(0);
            lineRenderer.positionCount--;
            lastPointRemovalTime = Time.realtimeSinceStartup;
        }
        else // we have some points that are not rendered, so we would immediately delete them
        {
            // remove points that are not rendered
            int tmp = inputPositions.Count;
            for (int i = 0; i < tmp - lineRenderer.positionCount; i++)
            {
                inputPositions.RemoveAt(0);
            }
        }

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
        Vector3 firstLinePoint = inputPositions[0];

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
            if (mainCam != null && new Rect(0, 0, 1, 1).Contains(mainCam.WorldToViewportPoint(inputPositions[i])))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAddNewPoint(Vector3 here)
    {
        // cache inputPosition size
        int arraySize = inputPositions.Count;

        // if don't have any points already, then we have accept the first point
        if (arraySize == 0)
        {
            return true;
        }

        if (Vector3.Distance(here, inputPositions[arraySize - 1]) > preferredPointsDistance)
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

        UpdateLineHeader();

        UpdateEdgeCollider();
    }

    private void UpdateLineHeader()
    {
        if (lineHead == null)
        {
            return;
        }

        // if there are still two points on the line, the head object should be aligned towards the line
        if (inputPositions.Count >= 2 && HasAtLeaseOnePointInScreen())
        {
            // calculating the rotation for line head
            int tmpSize = inputPositions.Count;
            Vector3 direction = (inputPositions[tmpSize - 1] - inputPositions[tmpSize - 2]).normalized;
            float angle = Vector3.SignedAngle(new Vector3(0, 1, 0), direction, Vector3.forward);

            // setting position and rotation of line head

            toRotation = Quaternion.Euler(0, 0, angle);
            lineHead.transform.position = inputPositions[tmpSize - 1];
        }
        else
        {
            // line head should be destroyed because we don't have more than 2 points
            Destroy(lineHead);
        }
    }

    private void UpdateEdgeCollider()
    {
        List<Vector2> tmp = new List<Vector2>();
        tmp.Clear();

        //ignoring all Z parameters for edge collider 2D
        for (int i = 0; i < inputPositions.Count; i++)
        {
            tmp.Add(new Vector2(inputPositions[i].x, inputPositions[i].y));
        }

        edgeCollider2D.points = tmp.ToArray();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if we hit ANYTHING we will disable the collider and line should be destroyed
        edgeCollider2D.enabled = false;
        shouldDestroy = true;

        switch (other.gameObject.tag)
        {
            case "Obstacle":
                break;
            case "Enemy":
                // we perform an attack if there's any attacking component on our line
                Attack attack;
                if (TryGetComponent<Attack>(out attack))
                {
                    attack.DealDamage(other);
                }
                break;

            default:
                break;
        }
    }
}
