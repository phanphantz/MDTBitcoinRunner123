using UnityEngine;
using System.Collections;

public class GroundChecker : MonoBehaviour
{
	[SerializeField] CarDatabase carDatabase;
	
	void OnTriggerStay(Collider other)
	{
		var matchedCarData = carDatabase.GetCarDataByName(other.transform.name);
		if (matchedCarData != null)
			ApplyCarDataToPlayer(matchedCarData);
	}

    private static void ApplyCarDataToPlayer(CarData carData)
    {
        PlayerManager.Instance.minY = carData.minY;
        PlayerManager.Instance.isOnTheCar = carData.name == "Car";
        PlayerManager.Instance.isOnTheBus = carData.name == "Bus";
    }

    void OnTriggerExit(Collider collision)
	{
		ApplyCarDataToPlayer(carDatabase.GetDefaultData());
	}

}
