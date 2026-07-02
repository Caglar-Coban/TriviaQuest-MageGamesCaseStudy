// Services/ApiService.cs
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiService : IApiService
{
    private readonly GameConfig _config;

    public ApiService(GameConfig config)
    {
        _config = config;
    }

    public IEnumerator GetLeaderboard(int page, Action<LeaderboardPage> onSuccess, Action<string> onError)
    {
        string url = string.Format(_config.leaderboardUrlFormat, page);
        yield return Get(url, onSuccess, onError);
    }

    public IEnumerator GetQuestions(Action<QuestionData> onSuccess, Action<string> onError)
    {
        yield return Get(_config.questionsUrl, onSuccess, onError);
    }

    private IEnumerator Get<T>(string url, Action<T> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            try
            {
                // UTF-8 encoding'i garantiye almak için (Türkçe karakter bozulmalarını önler)
                string json = Encoding.UTF8.GetString(request.downloadHandler.data);

                // JSON içindeki ham satır sonu/tab karakterlerini temizle (parse hatasını önler)
                json = SanitizeJson(json);

                T data = JsonUtility.FromJson<T>(json);
                onSuccess?.Invoke(data);
            }
            catch (Exception e)
            {
                onError?.Invoke($"Parse error: {e.Message}");
            }
        }
    }

    private string SanitizeJson(string json)
    {
        return json
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("\t", " ");
    }
}