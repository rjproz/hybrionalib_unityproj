using UnityEngine;

namespace Hybriona
{
    /// <summary>
    /// Holds the state and configuration for a single event trigger.
    /// Internally managed by EventTriggerManager — do not create manually.
    /// </summary>
    public class EventTriggerData
    {
        /// <summary>Delay in seconds before the trigger completes. Set to -1 for condition-only triggers.</summary>
        internal float triggerDelayTime { get; set; }

        /// <summary>If true, uses real time (unscaled) instead of game time (scaled by Time.timeScale).</summary>
        internal bool isTimeScaleIndependent { get; set; }

        /// <summary>Optional condition that is checked each frame. Trigger completes when this returns true.</summary>
        internal System.Func<bool> conditionTrigger { get; set; }

        /// <summary>Callback for delay-only triggers (no condition parameter).</summary>
        internal System.Action completionAction { get; set; }

        /// <summary>Callback for condition triggers. Parameter: true = condition met, false = timed out.</summary>
        internal System.Action<bool> conditionCompletionAction { get; set; }
        internal ulong Id { get; set; }
        internal bool isStopped = false;
        internal bool conditionMet = false;

        private float timeTrackingStarted;
        
        internal void StartTracking()
        {
            isStopped = false;
            conditionMet = false;
            if (isTimeScaleIndependent)
            {
                timeTrackingStarted = Time.realtimeSinceStartup;
            }
            else
            {
                timeTrackingStarted = Time.time;
            }
        }


        internal bool HasCompleted()
        {
            if (conditionTrigger != null)
            {
                if (conditionTrigger.Invoke())
                {
                    conditionMet = true;
                    return true;
                }

                if(triggerDelayTime < 0)
                {
                    return false;
                }
            }

            if (isTimeScaleIndependent)
            {
                if (Time.realtimeSinceStartup - timeTrackingStarted >= triggerDelayTime)
                {
                    conditionMet = false;
                    return true;
                }
            }
            else
            {
                if (Time.time - timeTrackingStarted >= triggerDelayTime)
                {
                    conditionMet = false;
                    return true;
                }
            }

            return false;

        }

        internal void Clean()
        {
            completionAction = null;
            conditionCompletionAction = null;
            conditionTrigger = null;
            conditionMet = false;
        }
    }
}
