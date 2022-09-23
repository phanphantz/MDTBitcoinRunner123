using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ResetTriggerer : OnTriggerReceiver
{
    void Awake() 
    {
        onTriggerEnter += ResetByMyTag;
    }

    private void OnDestroy() 
    {
        onTriggerEnter -= ResetByMyTag;
    }

    void ResetByMyTag()
    {
        ResetByGameObjectTag(gameObject);
    }

    private static void ResetByGameObjectTag(GameObject gameObj)
    {
        switch (gameObj.tag)
        {
            case "CloudLayer":
            case "CityBackgroundLayer":
            case "CityLayer":
            case "ForegroundLayer":
                LevelSpawnManager.Instance.ResetObject(gameObj.transform.parent.gameObject);
                break;

            case "Obstacles":
                gameObj.transform.parent.GetComponent<ObstacleManager>().DeactivateAllElements();
                LevelSpawnManager.Instance.ResetObject(gameObj.transform.parent.gameObject);
                break;
        }
    }

}
