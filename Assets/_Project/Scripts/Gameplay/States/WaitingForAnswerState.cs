using UnityEngine;

public class WaitingForAnswerState : IQuizState
{
    private QuizManager _quizManager;
    public void Enter(QuizManager quizManager)
    {
        _quizManager = quizManager;
        quizManager.timer.OnTimeUp += HandleTimeUp;
        quizManager.DisplayQuestion(quizManager.currentQuestionIndex);
        quizManager.DisplayChoices(quizManager.currentQuestionIndex);
        quizManager.ResetButtonColors();
        quizManager.timer.StartTimer(quizManager.gameConfig.questionduration);
    }

    public void Exit(QuizManager quizManager)
    {
        quizManager.timer.StopTimer();
        quizManager.timer.OnTimeUp -= HandleTimeUp;
    }

    public void OnAnswerSelected(QuizManager quizManager, int selectedAnswerIndex)
    {
        quizManager.ChangeState(new  ShowingResultState(selectedAnswerIndex));
    }

    public void HandleTimeUp()
    {
        _quizManager.ChangeState(new ShowingResultState(-1));
    }
}

