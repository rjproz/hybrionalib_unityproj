using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
    public class EventTriggerManager : MonoBehaviour
    {

        private Queue<EvenTriggerData> evenTriggerDataPool = new Queue<EvenTriggerData>();
        private List<EvenTriggerData> activeEventTriggers = new List<EvenTriggerData>();

        private static EventTriggerManager instance;

        protected static EventTriggerManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = GameObject.FindAnyObjectByType<EventTriggerManager>();

                    if(instance == null)
                    {
                        GameObject o = new GameObject("EventTriggerManager (DontDestroy)");
                        instance = o.AddComponent<EventTriggerManager>();
                        
                    }
                    DontDestroyOnLoad(instance.gameObject);

                    instance.StartCoroutine(instance.LoopProcess());
                }


                return instance;
            }
        }


        public static void AddTriggerEvent(float triggerTimeElasped, System.Action completion)
        {
            AddTriggerEvent(triggerTimeElasped, false, null, completion);
        }


        public static void AddTriggerEvent(float triggerTimeElasped, bool timeScaleIndependent, System.Action completion)
        {
            AddTriggerEvent(triggerTimeElasped, timeScaleIndependent, null, completion);
        }

        public static void AddTriggerEvent(float triggerTimeElasped, System.Func<bool> conditionTrigger,  System.Action completion)
        {
            AddTriggerEvent(triggerTimeElasped, conditionTrigger, completion);
        }

        public static void AddTriggerEvent(float triggerTimeElasped, bool timeScaleIndependent, System.Func<bool> conditionTrigger, System.Action completion)
        {
            EvenTriggerData evenTriggerData = null;
            if (Instance.evenTriggerDataPool.Count == 0)
            {
                evenTriggerData = new EvenTriggerData();
            }
            else
            {
                evenTriggerData = Instance.evenTriggerDataPool.Dequeue();
            }

            evenTriggerData.triggerTimeElasped = triggerTimeElasped;
            evenTriggerData.isTimeScaleIndependent = timeScaleIndependent;
            evenTriggerData.conditionTrigger = conditionTrigger;
            evenTriggerData.completionAction = completion;
            evenTriggerData.StartTracking();
            Instance.activeEventTriggers.Add(evenTriggerData);
        }


        private IEnumerator LoopProcess()
        {
            while(true)
            {
                for(int i = activeEventTriggers.Count - 1; i >=0; i--)
                {
                    var triggerData = activeEventTriggers[i];
                    if(triggerData.HasCompleted())
                    {
                        if(triggerData.completionAction != null)
                        {
                            triggerData.completionAction();
                        }
                        activeEventTriggers.RemoveAt(i);
                    }
                }
                yield return null;
            }
        }
        
    }

    public class EvenTriggerData
    {
        public float triggerTimeElasped { get; set; }
        public bool isTimeScaleIndependent { get; set; }
        public System.Func<bool> conditionTrigger { get; set; }
        public System.Action completionAction { get; set; }

        private float timeTrackingStarted;
        public void StartTracking()
        {
            if(isTimeScaleIndependent)
            {
                timeTrackingStarted = Time.realtimeSinceStartup;
            }
            else
            {
                timeTrackingStarted = Time.time;
            }
        }


        public bool HasCompleted()
        {
            if(conditionTrigger != null)
            {
                if(conditionTrigger.Invoke())
                {
                    return true;
                }
            }

            if (isTimeScaleIndependent)
            {
                return Time.realtimeSinceStartup - timeTrackingStarted >= triggerTimeElasped;
            }
            else
            {
                return Time.time - timeTrackingStarted >= triggerTimeElasped;
            }
           
        }
    }
}
