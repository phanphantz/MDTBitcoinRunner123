using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnTriggerer : OnTriggerReceiver
{
    void Awake() 
    {
        onTriggerEnter += SpawnObjectByMyTag;
    }

    private void OnDestroy() 
    {
        onTriggerEnter -= SpawnObjectByMyTag;
    }

    void SpawnObjectByMyTag()
    {
        SpawnObjectByGameObjectTag(gameObject);
    }

    static void SpawnObjectByGameObjectTag(GameObject gameObj)
    {
        switch (gameObj.tag)
        {
            case "CloudLayer":
                LevelSpawnManager.Instance.SpawnCloudLayer(LevelSpawnManager.PlaceLocation.Normal);
                break;

            case "CityBackgroundLayer":
                LevelSpawnManager.Instance.SpawnCityBackgroundLayer(LevelSpawnManager.PlaceLocation.Normal);
                break;

            case "CityLayer":
                LevelSpawnManager.Instance.SpawnCityLayer(LevelSpawnManager.PlaceLocation.Normal);
                break;

            case "ForegroundLayer":
                LevelSpawnManager.Instance.SpawnForegroundLayer(LevelSpawnManager.PlaceLocation.Normal);
                break;

            case "Obstacles":
                LevelSpawnManager.Instance.SpawnObstacles();
                break;
        }
    }

}
