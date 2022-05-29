using System;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using GameAnalyticsSDK;
using UnityEngine.UI;

public class FacebookScript : MonoBehaviour
{
    public static FacebookScript Instance;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            //Handle FB.Init
            FB.Init(() =>
            {
                FB.ActivateApp();
            });
        }
        
        GameAnalytics.Initialize();
    }

    public bool isInitialized()
    {
        if (FB.IsInitialized)
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }
        }
    }

    public void LogLevelCompleteEvent(int levelNumber)
    {
        if (isInitialized())
            FB.LogAppEvent("LevelComplete", levelNumber);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelNumber + "");
    }

    public void LogLevelStartEvent(int levelNumber)
    {
        if (isInitialized())
            FB.LogAppEvent("LevelStart", levelNumber);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelNumber + "");
    }

    public void LogReplayLevelEvent(int levelNumber)
    {
        if (isInitialized())
            FB.LogAppEvent("LevelReplay", levelNumber);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, levelNumber + "");
    }

    public void LogFailLevelEvent(int levelNumber)
    {
        if(isInitialized())
        {
            FB.LogAppEvent("LevelFail", levelNumber);
        }
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelNumber + "");
    }
}
