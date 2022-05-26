using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Enemies in the level")]
    [Tooltip("Enemies that user should destroy in this level")]
    [SerializeField] public List<BugBehaviour> bugs;

    private bool isWin = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CheckEnemyCounts() == 0 && isWin == false) // we won
        {                    
            // we won
            isWin = true;

            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            Debug.Log($"{SceneManager.sceneCountInBuildSettings}, {nextSceneIndex}");
            if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }            
        }
    }

    private int CheckEnemyCounts()
    {
        int EnemyCounts = 0;
        foreach (BugBehaviour bug in bugs)
        {
            if (bug != null) // we found a bug
            {
                EnemyCounts++;
            }
        }
        return EnemyCounts;
    }
}
