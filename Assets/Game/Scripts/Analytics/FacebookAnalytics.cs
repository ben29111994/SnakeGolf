using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class FacebookAnalytics : MonoBehaviour
{
    public bool isEnable;

    private static FacebookAnalytics instance;

    public static FacebookAnalytics Instance
    {
        get
        {
            return instance != null ? instance : GameObject.FindObjectOfType<FacebookAnalytics>();
        }
    }

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        if (isEnable == false) return;

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    // Unity will call OnApplicationPause(false) when an app is resumed
    // from the background
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
    public void LogGame_startEvent(double valToSum)
    {
        if (isEnable == false) return;

        FB.LogAppEvent(
            "game_start",
            (float)valToSum
        );
    }
    public void LogGame_endEvent(double valToSum)
    {
        if (isEnable == false) return;

        FB.LogAppEvent(
            "game_end",
            (float)valToSum
        );
    }

    public void LogGame_end_endlessEvent(double valToSum)
    {
        if (isEnable == false) return;

        FB.LogAppEvent(
            "game_end_endless",
            (float)valToSum
        );
    }

    public void LogGame_end_levelsEvent(double valToSum)
    {
        if (isEnable == false) return;

        FB.LogAppEvent(
            "game_end_levels",
            (float)valToSum
        );
    }

    public void LogReward_adsEvent(double valToSum)
    {
        if (isEnable == false) return;

        FB.LogAppEvent(
            "reward_ads",
            (float)valToSum
        );
    }

    public void LogInterstitial_adsEvent(double valToSum)
    {
        if (isEnable == false) return;

        FB.LogAppEvent(
            "interstitial_ads",
            (float)valToSum
        );
    }
}