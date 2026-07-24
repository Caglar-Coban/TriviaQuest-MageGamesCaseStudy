using UnityEngine;

public class LoadingState : IQuizState
{
    public void Enter(QuizManager quizManager){}
    public void Exit(QuizManager quizManager){}
    public void OnAnswerSelected(QuizManager quizManager, int selectedAnswerIndex){}
}
