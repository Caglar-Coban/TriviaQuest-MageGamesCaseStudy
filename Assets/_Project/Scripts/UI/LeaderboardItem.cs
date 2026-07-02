// UI/LeaderboardItem.cs
using UnityEngine;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public void Setup(LeaderboardEntry entry)
    {
        rankText.text = $"#{entry.rank}";
        nameText.text = entry.nickname;
        scoreText.text = entry.score.ToString();
    }
}