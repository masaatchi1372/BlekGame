using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Enemies in the level")]
    [Tooltip("Enemies that user should destroy in this level")]
    [SerializeField] public List<ObjectBehaviour> enemies;

    private bool isWin = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // we defeated all enemies
        if (CheckEnemyCounts() == 0 && isWin == false) // we won
        {                    
            isWin = true;
            // int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            // Debug.Log($"{SceneManager.sceneCountInBuildSettings}, {nextSceneIndex}");
            // if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
            // {
            //     SceneManager.LoadScene(nextSceneIndex);
            // }
        }
    }

    private int CheckEnemyCounts()
    {
        int enemyCounts = 0;
        foreach (ObjectBehaviour bug in enemies)
        {
            if (bug != null) // we found a bug
            {
                enemyCounts++;
            }
        }

        return enemyCounts;
    }
}
