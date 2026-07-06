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
        // Gerçek veriler geldiğinde textleri doldur ve görünürlüğü normale (%100) çek
        rankText.text = $"#{entry.rank}";
        nameText.text = entry.nickname;
        scoreText.text = entry.score.ToString();

        rankText.alpha = 1f;
        nameText.alpha = 1f;
        scoreText.alpha = 1f;
    }

    public void ShowLoadingState()
    {
        // Veri beklenirken geçici placeholder metinleri göster
        rankText.text = "-";
        nameText.text = "Yükleniyor...";
        scoreText.text = "-";

        // Yüklenme hissini artırmak için metinleri yarı saydam yap
        rankText.alpha = 0.4f;
        nameText.alpha = 0.4f;
        scoreText.alpha = 0.4f;
    }
}