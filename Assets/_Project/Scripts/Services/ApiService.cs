using System;
using System.Collections;
using UnityEngine;

public class ApiService : IApiService
{
    private readonly GameConfig _config;

    public ApiService(GameConfig config){
        _config = config;
    }

    public IEnumerator GetLeaderBoard(int page, Action <LeaderboardPage> onSuccess, Action <string> onError)
    {
        yield return null;
    }
    public IEnumerator GetQuestions(Action <QuestionData> onSuccess, Action <string> onError)
    {
        yield return null;
    }
}
