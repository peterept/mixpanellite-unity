using UnityEngine;
using System.Collections;

namespace MixpanelLite
{
	public class MixpanelAppTrack : MonoBehaviour 
	{
		public int AppBuildNumber = 1;

		void Start () 
		{
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
			MixpanelLite.Mixpanel.Instance.Track("Launched", new Hashtable(){
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
			});
		}

	}
}