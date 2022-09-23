using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
	public List<GameObject> elementGameObjList = new List<GameObject>();

	void Start()
	{
		foreach (Transform child in transform)
		{
			if (child.name != "SpawnTriggerer" && child.name != "ResetTriggerer")
			{
				elementGameObjList.Add(child.gameObject);
				child.gameObject.SetActive(false);
			}
		}
	}

	public void DeactivateAllElements()
	{
		foreach (GameObject child in elementGameObjList)
		{
			Transform explosionParticle = child.transform.Find("ExplosionParticle");
			if (explosionParticle != null)
			{
				explosionParticle.GetComponent<ParticleSystem>().Stop();
			}

			child.SetActive(false);
		}
	}

	public void ActivateAllElements()
	{
		foreach (GameObject child in elementGameObjList)
		{
			child.SetActive(true);
			child.GetComponent<Renderer>().enabled = true;
			child.GetComponent<Collider>().enabled = true;
		}
	}

}
