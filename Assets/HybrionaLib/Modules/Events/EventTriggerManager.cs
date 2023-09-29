using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
    public class EventTriggerManager : MonoBehaviour
    {
        [SerializeField]
        public int poolCount;

        [SerializeField]
        public int activeCount;

        private Queue<EventTriggerData> evenTriggerDataPool = new Queue<EventTriggerData>();
        private List<EventTriggerData> activeEventTriggers = new List<EventTriggerData>();

        
        private static EventTriggerManager instance;
        private static object readLock = new object();
        private static ulong idCounter;
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


        public static ulong AddTriggerEvent(float triggerTimeElasped, System.Action completion)
        {
            return AddTriggerEvent(triggerTimeElasped, false, null, completion);
        }

        public static ulong AddTriggerEvent(System.Func<bool> conditionTrigger, System.Action completion)
        {
            return AddTriggerEvent(-1, false, conditionTrigger, completion);
        }


        public static ulong AddTriggerEvent(float triggerTimeElasped, bool timeScaleIndependent, System.Action completion)
        {
            return AddTriggerEvent(triggerTimeElasped, timeScaleIndependent, null, completion);
        }

        public static ulong AddTriggerEvent(float triggerTimeElasped, System.Func<bool> conditionTrigger,  System.Action completion)
        {
            return AddTriggerEvent(triggerTimeElasped, conditionTrigger, completion);
        }

        public static ulong AddTriggerEvent(float triggerTimeElasped, bool timeScaleIndependent, System.Func<bool> conditionTrigger, System.Action completion)
        {
            EventTriggerData evenTriggerData = null;

            lock (readLock)
            {
                if (Instance.evenTriggerDataPool.Count == 0)
                {
                    evenTriggerData = new EventTriggerData();
                }
                else
                {
                    evenTriggerData = Instance.evenTriggerDataPool.Dequeue();
                    evenTriggerData.Clean();
                }

                evenTriggerData.Id = ++idCounter;
                evenTriggerData.triggerTimeElasped = triggerTimeElasped;
                evenTriggerData.isTimeScaleIndependent = timeScaleIndependent;
                evenTriggerData.conditionTrigger = conditionTrigger;
                evenTriggerData.completionAction = completion;
                evenTriggerData.StartTracking();
                Instance.activeEventTriggers.Add(evenTriggerData);

#if UNITY_EDITOR
                Instance.poolCount = Instance.evenTriggerDataPool.Count;
                Instance.activeCount = Instance.activeEventTriggers.Count;
#endif
            }
            return evenTriggerData.Id;

        }

        public static void AbortEvent(ulong eventTriggerId)
        {
            var eventTriggerData = Instance.activeEventTriggers.FindLast(o => o.Id == eventTriggerId);
            if(eventTriggerData != null)
            {
                eventTriggerData.isStopped = true;
            }
        }

        private IEnumerator LoopProcess()
        {
            while(true)
            {
                for(int i = activeEventTriggers.Count - 1; i >=0; i--)
                {
                    var triggerData = activeEventTriggers[i];
                    if(triggerData.isStopped || triggerData.HasCompleted())
                    {
                        
                        if (triggerData.completionAction != null)
                        {
                            triggerData.completionAction();
                        }

                       
                        lock (readLock)
                        {
                            evenTriggerDataPool.Enqueue(triggerData);
                            activeEventTriggers.RemoveAt(i);
                        }
                        
#if UNITY_EDITOR
                        Instance.poolCount = Instance.evenTriggerDataPool.Count;
                        Instance.activeCount = Instance.activeEventTriggers.Count;
#endif
                        
                       
                    }
                }
                yield return null;
            }
        }
        
    }

    
}
