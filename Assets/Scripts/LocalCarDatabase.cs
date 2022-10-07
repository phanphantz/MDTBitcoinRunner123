using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Defective.JSON;
using UnityEngine;

public class LocalCarDatabase : CarDatabase
{
    [SerializeField] TextAsset jsonFile;

    public override void PrepareDatas()
    {
        var jsonObject = new JSONObject(jsonFile.text);
        foreach(var json in jsonObject.list)
        {
            var carName = "";
            json.GetField(ref carName, "name");

            var minY = 0;
            json.GetField(ref minY , "minY");

            var newCarData = new CarData();
            newCarData.name = carName;
            newCarData.minY = minY;

            carDataList.Add(newCarData);
        }
    }
    
}
