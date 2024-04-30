using UnityEngine;

namespace Hybriona
{
    public class EventTriggerData
    {
        public float triggerTimeElasped { get; set; }
        public bool isTimeScaleIndependent { get; set; }
        public System.Func<bool> conditionTrigger { get; set; }
        public System.Action completionAction { get; set; }
        internal ulong Id { get; set; }
        internal bool isStopped = false;

        private float timeTrackingStarted;
        
        internal void StartTracking()
        {
            isStopped = false;
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
                    return true;
                }

                if(triggerTimeElasped < 0)
                {
                    return false;
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

        internal void Clean()
        {
            completionAction = null;
            conditionTrigger = null;
        }
    }
}
