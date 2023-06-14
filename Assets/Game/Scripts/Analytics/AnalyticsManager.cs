using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance;

    public bool isEnable;

    [HideInInspector] public EventType eventType;

    public enum EventType
    {
        StartEvent,
        EndEvent
    }

    private void Awake()
    {
        instance = this;
    }

    public void CallEvent(EventType _eventType)
    {
        if (isEnable == false) return;

        StartCoroutine(C_CallEvent(_eventType));
    }

    private IEnumerator C_CallEvent(EventType _eventType)
    {
        yield return new WaitForSeconds(1.0f);

        switch (_eventType)
        {
            case EventType.StartEvent:
                FacebookAnalytics.Instance.LogGame_startEvent(1);
                GameAnalyticsManager.Instance.Log_StartLevel();
                break;
            case EventType.EndEvent:
                FacebookAnalytics.Instance.LogGame_endEvent(1);
                GameAnalyticsManager.Instance.Log_EndLevel();
                break;
        }
    }
}
