using UnityEngine;
using TMPro;
public class LeaderBoardItem : MonoBehaviour
{
    public TMP_Text rank; 
    public TMP_Text nickname; 
    public TMP_Text score; 


    public void ShowData(LeaderBoardEntry LBEdata)
    {
        rank.text = $"#{LBEdata.rank}";
        nickname.text = LBEdata.nickname;
        score.text = LBEdata.score.ToString();


    }
}
