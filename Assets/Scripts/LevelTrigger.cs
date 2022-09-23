using UnityEngine;
using System.Collections;

public class LevelTrigger : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		var tryGetReceiver = other.GetComponent<OnTriggerReceiver>();
		if (tryGetReceiver != null)
			tryGetReceiver.NotifyOnTriggerEnter();
	}
}
