using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;
public class QuizManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public QuestionData questions;

    public List<Button> answerButtons;
    public int currentQuestionIndex = 0;
    public IApiService apiService;
    public GameConfig gameConfig;
    public Timer timer;
    public int score = 0;
    public int finalscore = 0;
    private IQuizState currentState;

    public GameObject finishedPanel;
    public TMP_Text finalScoreText;
    public event Action<int> OnScoreChanged;
    private CancellationTokenSource _cancellationTokenSource;
    private void Awake()
    {
        apiService = new ApiService(gameConfig); 
        
    }
    
    
    public async void Start()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            int capturedIndex = i; 
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(capturedIndex));
        }
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            ChangeState(new LoadingState());

            questions = await apiService.GetQuestions(_cancellationTokenSource.Token);
            
            ChangeState(new WaitingForAnswerState());
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Download cancelled");
        }
        catch(Exception ex)
        {
            Debug.LogError($"Problem Occurred : {ex.Message}");
        }

    }

    public void ChangeState(IQuizState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void DisplayQuestion(int index)
    {
        if (index < questions.questions.Length)
        {
            questionText.text = questions.questions[index].question;
        }
        else
        {
            Debug.Log("No more questions.");
        }
        
    }

    public void DisplayChoices(int index)
    {
        if (index < questions.questions.Length)
        {
            for (int i = 0; i < answerButtons.Count; i++)
            {
                if (i < questions.questions[index].choices.Length)
                {
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questions.questions[index].choices[i];
                    answerButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void ResetButtonColors()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }
    

    public void OnAnswerButtonClicked(int index)
    {
        currentState?.OnAnswerSelected(this, index);
    }

    public void UpdateScore(int scoreChange)
    {
        finalscore += scoreChange;
        OnScoreChanged?.Invoke(scoreChange);
    }

    private void OnDestroy()
    {
        if (_cancellationTokenSource != null)
        {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();  
        }
    
    }
}
