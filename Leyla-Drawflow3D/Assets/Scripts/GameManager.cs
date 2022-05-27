using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isLevel = false;
    [Header("Enemies in the level")]
    [Tooltip("Enemies that user should destroy in this level")]
    [SerializeField] public List<BugBehaviour> bugs;

    // Start is called before the first frame update
    void Start()
    {
        if (isLevel)
        {
            GameState.isWin = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLevel && CheckEnemyCounts() == 0 && GameState.isWin == false) // we won
        {
            // we won
            GameState.isWin = true;
            GameState.userLevel++;

            PlayerPrefs.SetInt("level", GameState.userLevel);
            PlayerPrefs.Save();

            gotoScene("ResultScene");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void gotoScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void playButton()
    {
        GameState.userLevel = PlayerPrefs.GetInt("level");

        if (GameState.userLevel > 0)
        {
            if (Application.CanStreamedLevelBeLoaded("Level " + GameState.userLevel))
            {
                SceneManager.LoadScene("Level " + GameState.userLevel);
            }
        }
        else
        {
            GameState.userLevel = 1;
            PlayerPrefs.SetInt("level", GameState.userLevel);
            PlayerPrefs.Save();

            gotoNextScene();
        }
    }

    public void gotoPreviousScene()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        Debug.Log($"{SceneManager.sceneCountInBuildSettings}, {previousSceneIndex}");
        if (SceneManager.sceneCountInBuildSettings > previousSceneIndex)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
    }

    public void gotoLevel()
    {
        string nextSceneName = "Level " + GameState.userLevel;
        Debug.Log($"{nextSceneName}");
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void gotoNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
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
