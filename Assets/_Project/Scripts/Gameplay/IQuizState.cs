public interface IQuizState
{
    void Enter(QuizManager quizManager);
    void Exit(QuizManager quizManager);
    void OnAnswerSelected(QuizManager quizManager, int selectedAnswerIndex);
}
