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

                try
                {
                    string textjson = courier.downloadHandler.text;
                    QuestionData textdata = JsonUtility.FromJson<QuestionData>(textjson);

                    onSuccess?.Invoke(textdata);
                }

                catch
                {
                    onError?.Invoke("Couldn't received Api data QuestionData -- translate error");

                }
              

            }
            else
            {
                Debug.Log("Couldn't received Api data Questions " + courier.error);
            }
        }
    
    }
}
