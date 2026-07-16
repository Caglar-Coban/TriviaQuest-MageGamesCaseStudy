using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FinishedState : IQuizState
{
    public void Enter(QuizManager quizManager)
    {
        GameManager.Instance.ShowGameOver(quizManager.finalscore);
    }

    public void Exit(QuizManager quizManager)
    {
        
    }
    public void OnAnswerSelected(QuizManager quizManager, int selectedAnswerIndex)
    {
        
    }
}
