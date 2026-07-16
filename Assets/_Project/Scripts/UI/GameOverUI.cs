using UnityEngine;
using TMPro;
public class GameOverUI : MonoBehaviour
{
    public TMP_Text finalScoreText;

    public void SetScore(int score)
    {
        finalScoreText.text = "Final Score: " + score;
    }
    public void RestartButtonClicked()
    {
        GameManager.Instance.RestartGame();
    }


    public void ReturnToMainMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}