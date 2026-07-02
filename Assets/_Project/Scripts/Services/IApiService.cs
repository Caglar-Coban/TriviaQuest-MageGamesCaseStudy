// Services/IApiService.cs
using System;
using System.Collections;

public interface IApiService
{
    IEnumerator GetLeaderboard(int page, Action<LeaderboardPage> onSuccess, Action<string> onError);
    IEnumerator GetQuestions(Action<QuestionData> onSuccess, Action<string> onError);
}