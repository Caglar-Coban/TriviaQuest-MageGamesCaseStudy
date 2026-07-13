using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FinishedState : IQuizState
{
    public void Enter(QuizManager quizManager)
    {
        quizManager.finishedPanel.SetActive(true);
        quizManager.finalScoreText.text = "Final Score: " + quizManager.score.ToString();
    }

    public void Exit(QuizManager quizManager)
    {
        
    }
    public void OnAnswerSelected(QuizManager quizManager, int selectedAnswerIndex)
    {
        
    }
}
