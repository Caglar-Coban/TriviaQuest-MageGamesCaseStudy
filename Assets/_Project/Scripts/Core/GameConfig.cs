using UnityEngine;

[CreateAssetMenu(fileName ="GameConfig", menuName = "Settings/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Score")]
    public int correctanswerscore = 10;
    public int wronganswerscore = -5;
    public int timeoutscore = -3;

    [Header("Timing")]
    public float questionduration = 20f;


    [Header("API")]
    public string leaderboardformaturl = "https://magegamessite.web.app/case1/leaderboard_page_{0}.json" ; 
    public string questionformaturl = "https://magegamessite.web.app/case1/questions.json" ; 
}

