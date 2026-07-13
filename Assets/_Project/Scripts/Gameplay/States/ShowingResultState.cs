using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ShowingResultState : IQuizState
{
    private int selectedAnswerIndex;
    private QuizManager _quizManager;
    public ShowingResultState(int selectedAnswerIndex)
    {
        this.selectedAnswerIndex = selectedAnswerIndex;
        
    }

    public void Enter(QuizManager quizManager)
    {
        _quizManager = quizManager;
        quizManager.timer.OnTimeUp += HandleTimeUp;
        int correctAnswerIndex = quizManager.questions.questions[quizManager.currentQuestionIndex].CorrectAnswerIndex;
        if (selectedAnswerIndex == -1)
        {
            Debug.Log("No answer selected. No Answer Penalty applied.");
            quizManager.UpdateScore(quizManager.gameConfig.timeoutscore);
        }
        else if (selectedAnswerIndex != correctAnswerIndex)
        {
            quizManager.answerButtons[selectedAnswerIndex].GetComponent<Image>().color = Color.red;
            Debug.Log("Wrong answer selected. Wrong Answer Penalty applied.");
            quizManager.UpdateScore(quizManager.gameConfig.wronganswerscore);
        }
        else
        {
            Debug.Log("Correct answer selected. Correct Answer Reward applied.");
            quizManager.UpdateScore(quizManager.gameConfig.correctanswerscore);
        }
        quizManager.answerButtons[correctAnswerIndex].GetComponent<Image>().color = Color.green;

        quizManager.timer.StartTimer(quizManager.gameConfig.resultDisplayDuration);
        
    }

    public void Exit(QuizManager quizManager)
    {
        quizManager.timer.StopTimer();
        quizManager.timer.OnTimeUp -= HandleTimeUp;
    }
    public void OnAnswerSelected(QuizManager quizManager, int selectedAnswerIndex)
    {
        
    }
    public void HandleTimeUp()
    {
        if (_quizManager.currentQuestionIndex < _quizManager.questions.questions.Length - 1)
        {
            _quizManager.currentQuestionIndex++;
            _quizManager.ChangeState(new WaitingForAnswerState());
        }
        else
        {
            _quizManager.ChangeState(new FinishedState());
        }
    }
    
}
