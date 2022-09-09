using UnityEngine;
using System.Collections;

public class GroundChecker : MonoBehaviour
{
	public string carName;
	public string busName;
	public string tankName;
	public string jeepName;

	[SerializeField] CarData[] cars;
	[SerializeField] CarData defaultData;

	void OnTriggerStay(Collider other)
	{
		foreach(var carData in cars)
		{
			if (other.transform.name.Contains(carData.name))
            {
                ApplyCarDataToPlayer(carData);
				break;
            }
        }
	}

    private static void ApplyCarDataToPlayer(CarData carData)
    {
        PlayerManager.Instance.minY = carData.minY;
        PlayerManager.Instance.isOnTheCar = carData.name == "Car";
        PlayerManager.Instance.isOnTheBus = carData.name == "Bus";
    }

    void OnTriggerExit(Collider collision)
	{
		ApplyCarDataToPlayer(defaultData);
	}

}
