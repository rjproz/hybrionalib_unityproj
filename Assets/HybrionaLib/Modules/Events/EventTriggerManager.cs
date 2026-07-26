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
        private Dictionary<ulong, EventTriggerData> activeLookup = new Dictionary<ulong, EventTriggerData>();
        private List<int> _removeBuffer = new List<int>();


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
            return AddTriggerEvent(triggerTimeElasped, false, conditionTrigger, completion);
        }

        public static ulong AddTriggerEvent(float triggerTimeElasped, bool timeScaleIndependent, System.Func<bool> conditionTrigger, System.Action completion)
        {
            EventTriggerData evenTriggerData = null;

            var inst = Instance;

            lock (readLock)
            {
                ulong newId = ++idCounter;
                if (inst.evenTriggerDataPool.Count == 0)
                {
                    evenTriggerData = new EventTriggerData();
                }
                else
                {
                    evenTriggerData = inst.evenTriggerDataPool.Dequeue();
                    evenTriggerData.Clean();
                }

                evenTriggerData.Id = newId;
                evenTriggerData.triggerTimeElasped = triggerTimeElasped;
                evenTriggerData.isTimeScaleIndependent = timeScaleIndependent;
                evenTriggerData.conditionTrigger = conditionTrigger;
                evenTriggerData.completionAction = completion;
                evenTriggerData.StartTracking();
                inst.activeEventTriggers.Add(evenTriggerData);
                inst.activeLookup.Add(newId, evenTriggerData);

#if UNITY_EDITOR
                inst.poolCount = inst.evenTriggerDataPool.Count;
                inst.activeCount = inst.activeEventTriggers.Count;
#endif
                return newId;
            }

        }

        public static void AbortEvent(ulong eventTriggerId)
        {
            var inst = Instance;
            lock (readLock)
            {
                if (inst.activeLookup.TryGetValue(eventTriggerId, out var eventTriggerData))
                {
                    eventTriggerData.isStopped = true;
                }
            }
        }

        private IEnumerator LoopProcess()
        {
            while (this != null)
            {
                _removeBuffer.Clear();

                for(int i = activeEventTriggers.Count - 1; i >=0; i--)
                {
                    var triggerData = activeEventTriggers[i];
                    if(triggerData.isStopped || triggerData.HasCompleted())
                    {
                        triggerData.completionAction?.Invoke();
                        _removeBuffer.Add(i);
                    }
                }

                if (_removeBuffer.Count > 0)
                {
                    lock (readLock)
                    {
                        for (int j = 0; j < _removeBuffer.Count; j++)
                        {
                            int i = _removeBuffer[j];
                            var triggerData = activeEventTriggers[i];
                            evenTriggerDataPool.Enqueue(triggerData);
                            activeLookup.Remove(triggerData.Id);
                            activeEventTriggers.RemoveAt(i);
                        }
                    }

#if UNITY_EDITOR
                    poolCount = evenTriggerDataPool.Count;
                    activeCount = activeEventTriggers.Count;
#endif
                }

                yield return null;
            }
        }

    }


}
