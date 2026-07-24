using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
public class ApiService : IApiService
{
    private readonly GameConfig _config;

    public ApiService(GameConfig config){
        _config = config;
    }

    public async Task<LeaderboardPage> GetLeaderBoard(int page, CancellationToken token)
    {
        string url = string.Format(_config.leaderboardformaturl, page);

        using (UnityWebRequest courier = UnityWebRequest.Get(url))
        {
            var operation = courier.SendWebRequest();

            while (!operation.isDone)
            {
                if (token.IsCancellationRequested)
                {
                    courier.Abort();
                    token.ThrowIfCancellationRequested();
                }
                await Task.Yield();
            }

            if (courier.result != UnityWebRequest.Result.Success)
            {
                throw new Exception("API Connection Error");

            }

            if (courier.downloadHandler == null || string.IsNullOrEmpty(courier.downloadHandler.text))
            {
                throw new Exception("DownloadHandler text is null or empty.");
            }

            string textjson = SanitizeJson(courier.downloadHandler.text);
            return JsonUtility.FromJson<LeaderboardPage>(textjson);
        }
        
    }
    public async Task<QuestionData> GetQuestions(CancellationToken token)
    {

        string url = _config.questionformaturl;

        using (UnityWebRequest courier = UnityWebRequest.Get(url))
        {
            var operation = courier.SendWebRequest();


            while (!operation.isDone)
            {
                if (token.IsCancellationRequested)
                {
                    courier.Abort();
                    token.ThrowIfCancellationRequested();
                }
                await Task.Yield();
            }
            if (courier.result != UnityWebRequest.Result.Success)
            {
                throw new Exception("API Connection Error");

            }

            if (courier.downloadHandler == null || string.IsNullOrEmpty(courier.downloadHandler.text))
                {
                        throw new Exception("DownloadHandler text is null or empty.");
                }

            
            

            string textjson = SanitizeJson(courier.downloadHandler.text);
            return JsonUtility.FromJson<QuestionData>(textjson);
        }
    
    }

    public string SanitizeJson(string json)
    {
        json = json.Replace("\n", " ");
        json = json.Replace("\r", " ");
        return json;
    }
}
