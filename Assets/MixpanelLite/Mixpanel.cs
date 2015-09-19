using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MixpanelLite
{
	public class Mixpanel : MonoBehaviour 
	{
		public string mixpanelProjectToken = ""; 

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
			
			HttpEndpoint("track", data);
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
			
			HttpEndpoint("engage", data);
		}

		void HttpEndpoint(string endpoint, Hashtable data)
		{
			string urlTemplate = "https://api.mixpanel.com/{0}/?data={1}";
			string json = JSON.JsonEncode(data);
			string jsonBase64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
			string url = string.Format(urlTemplate, endpoint, jsonBase64);
			StartCoroutine(HttpGet (url));
		}

		IEnumerator HttpGet(string url)
		{
			WWW www = new WWW(url);
			yield return www; 
		}

	}
}