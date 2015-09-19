using UnityEngine;
using System.Collections;

namespace MixpanelLite
{
	public class MixpanelAppTrack : MonoBehaviour 
	{
		public int AppBuildNumber = 1;
		float launchedTime;
		bool allowQuitting = false;

		void Awake ()
		{
			DontDestroyOnLoad(gameObject);
		}

		void Start () 
		{
			launchedTime = Time.realtimeSinceStartup;
			string LaunchDate = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
			MixpanelLite.Mixpanel.Instance.IdentifySetOnce(new Hashtable(){ 
				{"First Launched", LaunchDate}
			});
			MixpanelLite.Mixpanel.Instance.IdentifySet(new Hashtable(){ 
				{"Last Launched", LaunchDate}
			});
			MixpanelLite.Mixpanel.Instance.IdentifyAdd(new Hashtable(){ 
				{"Launch Count", 1}
			});
			MixpanelLite.Mixpanel.Instance.Track("Launched", AppProperties);
		}

		void OnApplicationQuit()
		{
			if (!allowQuitting)
			{
				Application.CancelQuit();

				Hashtable props = AppProperties;
				props.Add ("$duration", (Time.realtimeSinceStartup - launchedTime).ToString("0.0"));
				MixpanelLite.Mixpanel.Instance.Track("Exited", props);

				StartCoroutine(ApplicationQuitWhenMixpanelIsDone());
			}
		}

		// Call Application.Quit only when no more MixPanel messages to send
		IEnumerator ApplicationQuitWhenMixpanelIsDone()
		{
			while (MixpanelLite.Mixpanel.Instance.PendingMessagesCount > 0)
			{
				yield return new WaitForEndOfFrame();
			}
			allowQuitting = true;
			Application.Quit();
		}

		Hashtable AppProperties 
		{
			get
			{
				return new Hashtable(){
					{"App Name", Application.productName},
					{"App Identifier", Application.productName},
					{"App Version", Application.version},
					{"App Build", AppBuildNumber},
					{"Unity Version", Application.unityVersion},
					{"Unity Platform", Application.platform.ToString()},
					{"Device Model", SystemInfo.deviceModel},
					{"Device Type", SystemInfo.deviceType.ToString()},
					{"Device Platform", Application.platform},
					{"Device OS", SystemInfo.operatingSystem},
					{"Device Graphics Name", SystemInfo.graphicsDeviceName},
					{"Device Graphics Type", SystemInfo.graphicsDeviceType.ToString()},
					{"Device Graphics Vendor", SystemInfo.graphicsDeviceVendor},
					{"Device Graphics Version", SystemInfo.graphicsDeviceVersion},
					{"Device Graphics Memory", SystemInfo.graphicsMemorySize},
					{"Device Processor", SystemInfo.processorType},
					{"Device Memory", SystemInfo.systemMemorySize},
					{"VR Enabled", UnityEngine.VR.VRSettings.enabled},
					{"VR HMD", UnityEngine.VR.VRSettings.enabled ? UnityEngine.VR.VRSettings.loadedDevice.ToString() : ""}
				};
			}

		}

	}
}