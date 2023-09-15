/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TimeEvent.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/08/2018 13:53:53

*************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TimeEvent : HybSingleton<TimeEvent> 
	{
		private Queue<EventData> pool = new Queue<EventData>();
		private List<EventData> activeEvents = new List<EventData>();

		public void Add(float startDelay,System.Action onCompleted)
		{
			EventData eventData = null;
			if(pool.Count > 0)
            {
				eventData = pool.Dequeue();

			}
			else
            {
				eventData = new EventData();
            }
			eventData.Assign(startDelay, onCompleted);
			activeEvents.Add(eventData);
			//StartCoroutine(TimeEventProcess(startDelay, onCompleted));	
		}

		

        private void Awake()
        {
            for(int i=0;i<5;i++)
            {
				pool.Enqueue(new EventData());
            }
        }

        private void Update()
        {
            for(int i= activeEvents.Count-1; i >= 0;i--)
            {
				var currEvent = activeEvents[i];
				if(currEvent.HasComplete())
                {
					if (currEvent.callback != null)
					{
						currEvent.callback();
					}
					pool.Enqueue(currEvent);
					activeEvents.RemoveAt(i);
                }
            }
        }

		public class EventData
        {
			public float startDelay;
			public System.Action callback;

			public float timeStarted { get; private set; } = 0;
			public void Assign(float startDelay, System.Action onCompleted)
            {
				this.startDelay = startDelay;
				this.callback = onCompleted;
				this.timeStarted = Time.time;
            }

            public bool HasComplete()
            {
				return Time.time - timeStarted >= startDelay;
            }
        }
    }
}
