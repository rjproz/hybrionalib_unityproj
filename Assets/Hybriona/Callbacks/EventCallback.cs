/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  BooleanEvent.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/03/2018 20:39:38

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class EventCallback : HybSingleton<EventCallback> 
	{
		public delegate void OnBooleanEventExecuted();
		public delegate void OnGameObjectActiveStateChanged(bool enabled);

		private Dictionary<int,Coroutine> m_List = new Dictionary<int, Coroutine>();
		private WaitForEndOfFrame waitFrame;
		private int id_Counter = 0;	

		/// <summary>
		/// Add the specified eventFunc, loop and onBooleanEventExecuted.
		/// </summary>
		/// <param name="eventFunc">Event func.</param>
		/// <param name="loop">If set to <c>true</c> loop.</param>
		/// <param name="onBooleanEventExecuted">On boolean event executed.</param>
		public static int AddBoolean(System.Func<bool> eventFunc,bool loop,OnBooleanEventExecuted onBooleanEventExecuted)
		{
			return EventCallback.Instance.AddBooleanSession(eventFunc,loop,onBooleanEventExecuted);
		}

		/// <summary>
		/// Adds the state of the game object active.
		/// </summary>
		/// <returns>The game object active state.</returns>
		/// <param name="o">O.</param>
		/// <param name="loop">If set to <c>true</c> loop.</param>
		/// <param name="onGameObjectActiveStateChanged">On game object active state changed.</param>
		public static int AddGameObjectActiveState(GameObject o,bool loop,OnGameObjectActiveStateChanged onGameObjectActiveStateChanged)
		{
			return EventCallback.Instance.AddGameObjectActiveStateSession(o,loop,onGameObjectActiveStateChanged);
		}

		/// <summary>
		/// Remove the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void Remove(int id)
		{
			EventCallback.Instance.RemoveSession(id);
		}

		public void RemoveSession(int id)
		{
			if(m_List.ContainsKey(id))
			{
				m_List.Remove(id);
			}
		}




		#region BooleanEvent

		public int AddBooleanSession(System.Func<bool> eventFunc,bool loop,OnBooleanEventExecuted onBooleanEventExecuted)
		{
			id_Counter++;
			Coroutine routine = StartCoroutine(WaitForBooleanEvent(id_Counter,eventFunc,loop,onBooleanEventExecuted));
			m_List.Add(id_Counter,routine);
			return id_Counter;
		}


		private IEnumerator WaitForBooleanEvent(int id,System.Func<bool> eventFunc,bool loop,OnBooleanEventExecuted onBooleanEventExecuted)
		{
			if(waitFrame == null)
			{
				waitFrame = new WaitForEndOfFrame();
			}

			if(loop)
			{
				while(true)
				{
					if(eventFunc())
					{
						if(onBooleanEventExecuted != null)
						{
							onBooleanEventExecuted();
						}
					}
					yield return null;
				}
			}
			else
			{
				while(!eventFunc())
				{
					yield return null;
				}
				if(onBooleanEventExecuted != null)
				{
					onBooleanEventExecuted();
				}
				RemoveSession(id);
			}


		}

		#endregion

		#region GameObject Active State

		public int AddGameObjectActiveStateSession(GameObject o,bool loop,OnGameObjectActiveStateChanged onGameObjectActiveStateChanged)
		{
			id_Counter++;
			Coroutine routine = StartCoroutine(WaitForGameObjectActiveStateEvent(id_Counter,o,loop,onGameObjectActiveStateChanged));
			m_List.Add(id_Counter,routine);
			return id_Counter;
		}


		private IEnumerator WaitForGameObjectActiveStateEvent(int id,GameObject o,bool loop,OnGameObjectActiveStateChanged onGameObjectActiveStateChanged)
		{
			if(loop)
			{
				bool lastState = o.activeInHierarchy;
				while(true)
				{
					if(lastState != o.activeInHierarchy)
					{
						lastState = o.activeInHierarchy;
						if(onGameObjectActiveStateChanged != null)
						{
							onGameObjectActiveStateChanged(lastState);
						}
					}
					
					yield return null;
				}
			}
			else
			{
				bool lastState = o.activeInHierarchy;
				while(lastState == o.activeInHierarchy)
				{
					yield return null;
				}
				if(onGameObjectActiveStateChanged != null)
				{
					onGameObjectActiveStateChanged(o.activeInHierarchy);
				}

			}
		}
		#endregion

		private void Start () 
		{
			SetDontDestroyOnLoad();
			waitFrame = new WaitForEndOfFrame();
		}

	}
}
