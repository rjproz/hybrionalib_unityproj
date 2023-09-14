//#if HYBRIONA_LIB_ENABLE_EVENT_TRIGGER
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

        public static void AddTriggerEvent(System.Func<bool> conditionTrigger, System.Action completion)
        {
            AddTriggerEvent(-1, false, conditionTrigger, completion);
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
                }


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
                        System.Action actionCached = triggerData.completionAction;
                        triggerData.Clean();
                        lock (readLock)
                        {
                            evenTriggerDataPool.Enqueue(triggerData);
                            activeEventTriggers.RemoveAt(i);
                        }
                        
#if UNITY_EDITOR
                        Instance.poolCount = Instance.evenTriggerDataPool.Count;
                        Instance.activeCount = Instance.activeEventTriggers.Count;
#endif
                        if (actionCached != null)
                        {
                            actionCached();
                        }
                       
                    }
                }
                yield return null;
            }
        }
        
    }

    
}
//#endif