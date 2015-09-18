using UnityEngine;
using System.Collections;

public class MixpanelDemo : MonoBehaviour 
{
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
			{"App Build", 1},
			{"Device Platform", SystemInfo.operatingSystem},
			{"Device Model", SystemInfo.deviceModel},
			{"Device System Version", Application.platform},
			{"App VR Enabled", UnityEngine.VR.VRSettings.enabled},
		});
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.A))
		{
			MixpanelLite.Mixpanel.Instance.Track("My Event", new Hashtable(){ 
				{"Key", "A"}
			});
		}
	}
}
