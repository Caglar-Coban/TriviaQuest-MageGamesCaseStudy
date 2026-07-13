using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiService : IApiService
{
    private readonly GameConfig _config;

    public ApiService(GameConfig config){
        _config = config;
    }

    public IEnumerator GetLeaderBoard(int page, Action <LeaderboardPage> onSuccess, Action <string> onError)
    {
        string url = string.Format(_config.leaderboardformaturl, page);

        using (UnityWebRequest courier = UnityWebRequest.Get(url))
        {
            yield return courier.SendWebRequest();

            if (courier.result == UnityWebRequest.Result.Success)
            {

                try
                {
                    string textjson = courier.downloadHandler.text;

                    LeaderboardPage pagedata= JsonUtility.FromJson<LeaderboardPage>(textjson);


                    onSuccess?.Invoke(pagedata);

                }
                catch
                {
                    onError?.Invoke("Couldn't received Api data LeaderBoard -- translate error");
                }
                
                

            }
            else
            {
                Debug.Log("Couldn't received Api data LeaderBoard " + courier.error);
            }
        }
        
    }
    public IEnumerator GetQuestions(Action <QuestionData> onSuccess, Action <string> onError)
    {

        string url = _config.questionformaturl;

        using (UnityWebRequest courier = UnityWebRequest.Get(url))
        {
            yield return courier.SendWebRequest();

            if (courier.result == UnityWebRequest.Result.Success)
            {
                QuestionData textdata = null;
                try
                {
                    if (courier.downloadHandler == null || string.IsNullOrEmpty(courier.downloadHandler.text))
                    {
                        throw new Exception("DownloadHandler text is null or empty.");
                    }
                    
                    string textjson = courier.downloadHandler.text;
                    textjson = SanitizeJson(textjson);
                    textdata = JsonUtility.FromJson<QuestionData>(textjson);

                    
                }

                catch (Exception e)
                {
                    Debug.LogError("PARSE ERROR: " + e.Message);
                    onError?.Invoke("Couldn't received Api data QuestionData -- translate error");

                }
                onSuccess?.Invoke(textdata);

            }
            else
            {
                Debug.Log("Couldn't received Api data Questions " + courier.error);
            }
        }
    
    }

    public string SanitizeJson(string json)
    {
        json = json.Replace("\n", " ");
        json = json.Replace("\r", " ");
        return json;
    }
}
