using UnityEngine;
using Hybriona;
public class SampleEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
   
    void Start()
    {
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
        EventTriggerManager.AddTriggerEvent(2, () =>
        {
            Debug.Log("This should be called after 20 second but actual delay is 2");
        });

        EventTriggerManager.AddTriggerEvent(2,true, () =>
        {
            Debug.Log("This should be called after 2 second and delay is 2 because it is time scale independent");
        });
    }


    void CheckExtraCondition()
    {
        EventTriggerManager.AddTriggerEvent(triggerTimeElasped: 10,timeScaleIndependent: true, conditionTrigger: IsMouseClicked, () =>
        {
            Debug.Log("This should be called after 10 second or in user clicks mouse left button");
        });

        EventTriggerManager.AddTriggerEvent( conditionTrigger: IsMouseClicked, () =>
        {
            Debug.Log("This should be called when user clicks mouse left button");
        });
    }
   
    bool IsMouseClicked()
    {
        return Input.GetMouseButton(0);
    }

   
}