using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneStateHandler : MonoBehaviour
{

    public GameObject winUI;
    public GameObject LoseUI;


    // Update is called once per frame
    void Start()
    {
        if (GameState.isWin)
        {
            winUI.SetActive(true);
            LoseUI.SetActive(false);
        }
        else
        {
            winUI.SetActive(false);
            LoseUI.SetActive(true);
        }

        GameState.isWin = false;
        
    }
}
