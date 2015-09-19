using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MixpanelLite
{
	public class Mixpanel : MonoBehaviour 
	{
		#region Singleton Instance

		private static Mixpanel _instance;
		static public Mixpanel Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<Mixpanel>();
				}
				return _instance;
			}
		}

		#endregion

		public string mixpanelProjectToken = ""; 
		Queue<KeyValuePair<string, string> > EventsQueue = new Queue<KeyValuePair<string, string> >();
		bool EventsQueueIsProcessing = false;

		string _DistinctIdentifier = "";
		public string DistinctIdentifier 
		{
			get
			{
				if (string.IsNullOrEmpty(_DistinctIdentifier))
				{
					_DistinctIdentifier = PlayerPrefs.GetString(mixpanelProjectToken);
					if (string.IsNullOrEmpty(_DistinctIdentifier))
					{
						_DistinctIdentifier = System.Guid.NewGuid().ToString();
						PlayerPrefs.SetString(mixpanelProjectToken, _DistinctIdentifier);
						PlayerPrefs.Save();
					}
				}

				return _DistinctIdentifier;
			}
		}

		public void Track(string eventName, Hashtable properties = null)
		{
			if (string.IsNullOrEmpty(mixpanelProjectToken))
			{
				return;
			}

			Hashtable data = new Hashtable();
			data["event"] = eventName;
			
			if (properties == null)
			{
				properties = new Hashtable();
			}
			properties["token"] = mixpanelProjectToken;
			properties["distinct_id"] = DistinctIdentifier;
			data["properties"] = properties;
			QueueMessage("track", data);
		}

		public void IdentifySetOnce(Hashtable properties = null)
		{
			IdentifyUpdate("$set_once", properties);
		}

		public void IdentifySet(Hashtable properties = null)
		{
			IdentifyUpdate("$set", properties);
		}

		public void IdentifyAdd(Hashtable properties = null)
		{
			IdentifyUpdate("$add", properties);
		}

		void IdentifyUpdate(string operationName, Hashtable properties)
		{
			if (string.IsNullOrEmpty(mixpanelProjectToken))
			{
				return;
			}

			Hashtable data = new Hashtable();
			data["$token"] = mixpanelProjectToken;
			data["$distinct_id"] = DistinctIdentifier;
			data[operationName] = properties;
			QueueMessage("engage", data);
		}

		public int PendingMessagesCount
		{
			get 
			{
				return EventsQueue.Count;
			}
		}

		void QueueMessage(string endpoint, Hashtable data)
		{
			string json = JSON.JsonEncode(data);
			EventsQueue.Enqueue(new KeyValuePair<string, string>(endpoint, json));
			if (!EventsQueueIsProcessing)
			{
				StartCoroutine(HandleQueuedMessages());
			}
		}

		IEnumerator HandleQueuedMessages()
		{
			EventsQueueIsProcessing = true;
			while (EventsQueue.Count > 0)
			{
				KeyValuePair<string, string> EventMessage = EventsQueue.Peek();
				string urlTemplate = "https://api.mixpanel.com/{0}/?data={1}";
				string endpoint = EventMessage.Key;
				string json = EventMessage.Value;
				string jsonBase64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
				string url = string.Format(urlTemplate, endpoint, jsonBase64);
				WWW www = new WWW(url);
				yield return www;
				EventsQueue.Dequeue();
			}
			EventsQueueIsProcessing = false;
		}

	}
}