using UnityEngine;
using System.Collections;
using Defective.JSON;
using UnityEngine.Networking;

public class ServerCarDatabase : CarDatabase
{
    [SerializeField] string url;
    public override void PrepareDatas()
    {
        StartCoroutine(DownloadCarDatabase());
    }

    IEnumerator DownloadCarDatabase()
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        var downloadedText = webRequest.downloadHandler.text;
        var jsonObject = new JSONObject(downloadedText);
        
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
