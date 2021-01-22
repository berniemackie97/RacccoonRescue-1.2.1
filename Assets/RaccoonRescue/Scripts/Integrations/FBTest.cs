using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if FACEBOOK

using Facebook.Unity;

public class FBTest : MonoBehaviour
{

    public string obj = "object";
    public string objType = "object";
    public string objRequest = "object";
    public string readRequest = "me/objects/object?fields=description";
    public string saveObjectLine = "{\"fb:app_id\":\"1040909022611487\",\"og:type\":\"object\",\"og:title\":\"level scores\",\"og:description\":\"111\"}";

    void OnGUI()
    {
        if (GUILayout.Button("Save"))
        {
            SaveScores();
        }
        if (GUILayout.Button("Read"))
        {
            ReadScores();
        }
        if (GUILayout.Button("Delete"))
        {
            DeleteScores();
        }

    }

    public void SaveScores()
    {
        int score = 10000;

        var scoreData =
                new Dictionary<string, string>() { { "score", score.ToString() } };

        string value = "1000";
        //for (int i = 0; i < 100; i++)
        //{
        //    value += "10000 ";
        //}
        //print(value);
        IDictionary<string, string> dic =
           new Dictionary<string, string>();
        //dic.Add(obj, "http://samples.ogp.me/768772476466403");
        //dic.Add(obj, saveObjectLine);
        FB.API("/me/scores", HttpMethod.POST, APICallBack, scoreData);
        //FB.API(objRequest, HttpMethod.POST, RequestCallback, dic);
        //"object": "{\"fb:app_id\":\"302184056577324\",\"og:type\":\"object\",\"og:url\":\"Put your own URL to the object here\",\"og:title\":\"Sample Object\",\"og:image\":\"https:\\\/\\\/s-static.ak.fbcdn.net\\\/images\\\/devsite\\\/attachment_blank.png\"}"

    }

    public void ReadScores()
    {

        FB.API(readRequest, HttpMethod.GET, RequestCallback);
    }

    private void RequestCallback(IGraphResult result)
    {
        if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log(result.RawResult);

            var resultDictionary = result.ResultDictionary;
            if (resultDictionary.ContainsKey("data"))
            {
                var dataArray = (List<object>)resultDictionary["data"];

                if (dataArray.Count > 0)
                {
                    for (int i = 0; i < dataArray.Count; i++)
                    {
                        var firstGroup = (Dictionary<string, object>)dataArray[i];
                        foreach (KeyValuePair<string, object> entry in firstGroup)
                        {
                            print(entry.Key + " " + entry.Value);
                        }
                        //print(firstGroup["id"] + " " + firstGroup["title"]);
                    }
                    //this.gamerGroupCurrentGroup = (string)firstGroup["id"];
                }
            }
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log(result.Error);

        }
    }

    void DeleteScores()
    {
        FB.API(objRequest, HttpMethod.GET, DeleteCallback);

    }

    private void DeleteCallback(IGraphResult result)
    {
        if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log(result.RawResult);

            var resultDictionary = result.ResultDictionary;
            if (resultDictionary.ContainsKey("data"))
            {
                var dataArray = (List<object>)resultDictionary["data"];

                if (dataArray.Count > 0)
                {
                    for (int i = 0; i < dataArray.Count; i++)
                    {
                        var firstGroup = (Dictionary<string, object>)dataArray[i];
                        FB.API((string)firstGroup["id"], HttpMethod.DELETE, APICallBack);
                    }
                    //this.gamerGroupCurrentGroup = (string)firstGroup["id"];
                }
            }
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log(result.Error);

        }
    }

    public void APICallBack(IGraphResult result)
    {
        Debug.Log(result);
    }

}
#endif
