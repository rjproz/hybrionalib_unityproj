using UnityEngine;
using Hybriona;
public class SampleEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
   
    void Start()
    {
        // Simple delay: fires after 5 seconds (no parameter)
        EventTriggerManager.AddTriggerEvent(5,()=>
        {
            SlowDownTime();
            RunNewEvent();
            CheckExtraCondition();
        });
    }

    void SlowDownTime()
    {
        Debug.Log("Time slowed down after 5 seconds");
        Time.timeScale = .1f;
    }

    void RunNewEvent()
    {
        // Delay-only trigger (no condition, no parameter)
        EventTriggerManager.AddTriggerEvent(2, () =>
        {
            Debug.Log("This should be called after 2 seconds");
        });

        // Time scale independent delay (no condition, no parameter)
        EventTriggerManager.AddTriggerEvent(2,true, () =>
        {
            Debug.Log("This should be called after 2 real seconds");
        });
    }


    void CheckExtraCondition()
    {
        // Condition + timeout: fires when mouse clicked OR after 10 seconds
        // Callback receives bool: true = condition met, false = timed out
        EventTriggerManager.AddTriggerEvent(triggerTimeoutTime: 10,timeScaleIndependent: true, conditionTrigger: IsMouseClicked, (conditionMet) =>
        {
            if (conditionMet)
                Debug.Log("Mouse clicked! Condition was met.");
            else
                Debug.Log("Timed out after 10 seconds.");
        });

        // Condition only (no timeout): runs until mouse is clicked
        EventTriggerManager.AddTriggerEvent( conditionTrigger: IsMouseClicked, (conditionMet) =>
        {
            Debug.Log("Mouse clicked! Condition was met.");
        });
    }
   
    bool IsMouseClicked()
    {
        return Input.GetMouseButton(0);
    }

   
}