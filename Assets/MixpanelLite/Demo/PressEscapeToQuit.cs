using UnityEngine;
public class PressEscapeToQuit : MonoBehaviour 
{
	void Update() 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
