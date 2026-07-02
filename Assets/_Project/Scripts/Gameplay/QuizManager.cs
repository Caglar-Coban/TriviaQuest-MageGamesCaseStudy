// Gameplay/QuizManager.cs
using UnityEngine;

public enum QuizState { Loading, WaitingForAnswer, ShowingResult, Finished }

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private QuestionView questionView;
    [SerializeField] private QuestionTimer timer;
    [SerializeField] private ScoreManager scoreManager;

    private IApiService _apiService;
    private Question[] _questions;
    private int _currentIndex;
    private QuizState _state;

    private void Awake()
    {
        _apiService = new ApiService(config);
    }

    private void Start()
    {
        _state = QuizState.Loading;
        StartCoroutine(_apiService.GetQuestions(OnQuestionsLoaded, OnLoadError));
        timer.OnTimeUp += HandleTimeUp;
        questionView.OnAnswerSelected += HandleAnswerSelected;
    }

    private void OnDestroy()
    {
        timer.OnTimeUp -= HandleTimeUp;
        questionView.OnAnswerSelected -= HandleAnswerSelected;
    }

    private void OnQuestionsLoaded(QuestionData data)
    {
        _questions = data.questions;
        _currentIndex = 0;
        scoreManager.ResetScore();
        ShowQuestion();
    }

    private void OnLoadError(string error)
    {
        Debug.LogError($"Questions load failed: {error}");
    }

    private void ShowQuestion()
    {
        if (_currentIndex >= _questions.Length)
        {
            _state = QuizState.Finished;
            questionView.ShowGameOver(scoreManager.CurrentScore);
            return;
        }

        _state = QuizState.WaitingForAnswer;
        questionView.Display(_questions[_currentIndex]);
        timer.StartTimer(config.questionDuration);
    }

    private void HandleAnswerSelected(int selectedIndex)
    {
        if (_state != QuizState.WaitingForAnswer) return;

        timer.StopTimer();
        _state = QuizState.ShowingResult;

        Question current = _questions[_currentIndex];
        bool isCorrect = selectedIndex == current.CorrectAnswerIndex; // düzeltildi

        scoreManager.AddScore(isCorrect ? config.correctAnswerScore : config.wrongAnswerScore);
        questionView.ShowResult(selectedIndex, current.CorrectAnswerIndex, isCorrect, OnResultShown);
    }

    private void HandleTimeUp()
    {
        if (_state != QuizState.WaitingForAnswer) return;

        _state = QuizState.ShowingResult;
        scoreManager.AddScore(config.timeoutScore);
        questionView.ShowResult(-1, _questions[_currentIndex].CorrectAnswerIndex, false, OnResultShown); // düzeltildi
    }

    private void OnResultShown()
    {
        _currentIndex++;
        ShowQuestion();
    }
}