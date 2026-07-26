using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
    /// <summary>
    /// Manages timed and condition-based event triggers. 
    /// Auto-creates a singleton instance on first use with DontDestroyOnLoad.
    /// Thread-safe for adding and aborting triggers from any thread.
    /// </summary>
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
        private static readonly object readLock = new object();
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
                        DontDestroyOnLoad(instance.gameObject);
                        instance.StartCoroutine(instance.LoopProcess());
                    }
                }


                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }


        /// <summary>
        /// Triggers an action after a delay in seconds.
        /// </summary>
        /// <param name="triggerDelayTime">Time in seconds to wait before invoking the callback.</param>
        /// <param name="completion">Action to invoke when the delay elapses.</param>
        /// <returns>An ID that can be used to abort the trigger early.</returns>
        public static ulong AddTriggerEvent(float triggerDelayTime, System.Action completion)
        {
            return AddTriggerEvent(triggerDelayTime, false, completion);
        }

        /// <summary>
        /// Triggers an action when a condition is met (checked every frame).
        /// No timeout — runs until the condition returns true.
        /// </summary>
        /// <param name="conditionTrigger">Function that returns true when the trigger should fire.</param>
        /// <param name="completion">Action to invoke. Parameter: true = condition met.</param>
        /// <returns>An ID that can be used to abort the trigger early.</returns>
        public static ulong AddTriggerEvent(System.Func<bool> conditionTrigger, System.Action<bool> completion)
        {
            return AddTriggerEvent(-1, false, conditionTrigger, completion);
        }


        /// <summary>
        /// Triggers an action after a delay, with option to ignore Time.timeScale.
        /// </summary>
        /// <param name="triggerDelayTime">Time in seconds to wait before invoking the callback.</param>
        /// <param name="timeScaleIndependent">If true, uses real time instead of scaled game time.</param>
        /// <param name="completion">Action to invoke when the delay elapses.</param>
        /// <returns>An ID that can be used to abort the trigger early.</returns>
        public static ulong AddTriggerEvent(float triggerDelayTime, bool timeScaleIndependent, System.Action completion)
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
                evenTriggerData.triggerDelayTime = triggerDelayTime;
                evenTriggerData.isTimeScaleIndependent = timeScaleIndependent;
                evenTriggerData.conditionTrigger = null;
                evenTriggerData.completionAction = completion;
                evenTriggerData.conditionCompletionAction = null;
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

        /// <summary>
        /// Triggers an action after a timeout OR when a condition is met, whichever comes first.
        /// </summary>
        /// <param name="triggerTimeoutTime">Timeout in seconds. Set to -1 for condition-only (no timeout).</param>
        /// <param name="conditionTrigger">Function that returns true when the trigger should fire.</param>
        /// <param name="completion">Action to invoke. Parameter: true = condition met, false = timed out.</param>
        /// <returns>An ID that can be used to abort the trigger early.</returns>
        public static ulong AddTriggerEvent(float triggerTimeoutTime, System.Func<bool> conditionTrigger, System.Action<bool> completion)
        {
            return AddTriggerEvent(triggerTimeoutTime, false, conditionTrigger, completion);
        }

        /// <summary>
        /// Triggers an action after a timeout OR when a condition is met, with time scale control.
        /// When both condition and timeout are set, the trigger fires when either is satisfied (whichever first).
        /// </summary>
        /// <param name="triggerTimeoutTime">Timeout in seconds. Set to -1 for condition-only (no timeout).</param>
        /// <param name="timeScaleIndependent">If true, uses real time (Time.realtimeSinceStartup) instead of game time.</param>
        /// <param name="conditionTrigger">Condition checked each frame. If null, only the timeout is used.</param>
        /// <param name="completion">Action to invoke. Parameter: true = condition met, false = timed out.</param>
        /// <returns>An ID that can be used to abort the trigger early via AbortEvent.</returns>
        public static ulong AddTriggerEvent(float triggerTimeoutTime, bool timeScaleIndependent, System.Func<bool> conditionTrigger, System.Action<bool> completion)
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
                evenTriggerData.triggerDelayTime = triggerTimeoutTime;
                evenTriggerData.isTimeScaleIndependent = timeScaleIndependent;
                evenTriggerData.conditionTrigger = conditionTrigger;
                evenTriggerData.completionAction = null;
                evenTriggerData.conditionCompletionAction = completion;
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

        /// <summary>
        /// Aborts a running trigger by its ID. The completion callback will still be invoked.
        /// Safe to call from any thread.
        /// </summary>
        /// <param name="eventTriggerId">The ID returned by AddTriggerEvent.</param>
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
                        if (triggerData.conditionTrigger != null)
                            triggerData.conditionCompletionAction?.Invoke(triggerData.conditionMet);
                        else
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
