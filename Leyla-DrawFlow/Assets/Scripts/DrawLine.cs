using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    #region Header Line Prefab
    [Header("LINE PREFAB")]
    #endregion
    #region Tooltip
    [Tooltip("should be populated with a prefab with LineRendere and EdgeCollider2D")]
    #endregion
    // The line prefab that we're going to be using in order to instantiate the user drawn path
    public GameObject linePrefab;

    private GameObject currentLine;
    private List<GameObject> lineGameObjectsList;
    private Queue<GameObject> deletionQueue;

    private void Start()
    {
        // initiating
        lineGameObjectsList = new List<GameObject>();
        deletionQueue = new Queue<GameObject>();
    }

    void Update()
    {
        // on right mouse button all lines would be destroyed
        if (Input.GetMouseButtonDown(1))
        {
            if (lineGameObjectsList.Count > 0)
            {
                DestroyLines();
            }
            currentLine = null;
        }

        // caching currentLine if one exist
        Line currentLineComponent = null;
        if (currentLine != null)
        {
            currentLineComponent = currentLine.GetComponent<Line>();
        }

        // check for left mouse button or touch
        if (Input.GetMouseButtonDown(0))
        {
            // Initiating the line
            CreateLine();
        }

        // on Drag
        if (Input.GetMouseButton(0))
        {
            // if point distance is enough, we'll add a new point to the currentLine
            Vector2 tempInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (currentLineComponent != null && currentLineComponent.CanAddNewPoint(tempInput))
            {
                currentLineComponent.AddNewPoint(tempInput);
            }
        }

        // on release
        if (Input.GetMouseButtonUp(0))
        {

            if (currentLine != null)
            {
                // now we activate the edgecollider on the line
                EdgeCollider2D edgeCollider2D;
                currentLine.TryGetComponent<EdgeCollider2D>(out edgeCollider2D);
                edgeCollider2D.enabled = true;

                // drawing is finished and we add the line to the lineList
                lineGameObjectsList.Add(currentLine);

                // we set the lastDrawnTime to current timestamp since game startup
                currentLineComponent.lastDrawnTime = Time.realtimeSinceStartup;
            }

            // the line is created and there's no drawing in process
            currentLine = null;
        }

        // each line should be continue its flow
        if (lineGameObjectsList.Count > 0)
        {
            foreach (GameObject line in lineGameObjectsList)
            {
                if (line != null)
                {
                    Line lineComponent = line.GetComponent<Line>();

                    // if there's still one point on the line which user can see (it's in the screen) we should continue the line flow
                    if (lineComponent.inputPositions.Count == 1 || !lineComponent.HasAtLeaseOnePointInScreen())
                    {
                        deletionQueue.Enqueue(line);
                        continue;
                    }

                    // if we should destroy the line
                    if (lineComponent.shouldDestroy)
                    {
                        // check the line points removal speed
                        if (Time.realtimeSinceStartup - lineComponent.lastPointRemovalTime > lineComponent.lineRemovalSpeed)
                        {
                            lineComponent.RemoveFirstPoint();
                        }
                    }
                    else // otherwise will continue the flow
                    {
                        lineComponent.ContinueLineFlow();
                    }
                }
            }
        }

        ClearDeletionQueue();
    }

    // Starting line creation process
    private void CreateLine()
    {
        // initiating current line with our prefab with no vertex on its lineRenderer
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        // now we activate the edgecollider on the line
        EdgeCollider2D edgeCollider2D;
        currentLine.TryGetComponent<EdgeCollider2D>(out edgeCollider2D);
        edgeCollider2D.enabled = false;
    }

    private void ClearDeletionQueue()
    {
        for (int i = 0; i < deletionQueue.Count; i++)
        {
            var tmp = deletionQueue.Dequeue();
            Destroy(tmp);
            lineGameObjectsList.Remove(tmp);
        }
    }

    // destroy all line in lineGameObjectsList
    private void DestroyLines()
    {
        foreach (GameObject line in lineGameObjectsList)
        {
            if (line != null)
            {
                deletionQueue.Enqueue(line);
            }
        }
    }
}