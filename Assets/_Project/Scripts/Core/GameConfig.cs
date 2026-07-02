// Core/GameConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "TriviaGame/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Scoring")]
    public int correctAnswerScore = 10;
    public int wrongAnswerScore = -5;
    public int timeoutScore = -3;

    [Header("Timing")]
    public float questionDuration = 20f;

    [Header("API")]
    [Tooltip("{0} yerine sayfa numarası gelecek")]
    public string leaderboardUrlFormat = "https://magegamessite.web.app/case1/leaderboard_page_{0}.json";
    public string questionsUrl = "https://magegamessite.web.app/case1/questions.json";
}