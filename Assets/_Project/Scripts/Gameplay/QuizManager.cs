using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
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
    private IQuizState currentState;

    public GameObject finishedPanel;
    public TMP_Text finalScoreText;
    public event Action<int> OnScoreChanged;
    private void Awake()
    {
        apiService = new ApiService(gameConfig); 
        
    }
    
    
    public void Start()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            int capturedIndex = i; 
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(capturedIndex));
        }
        StartCoroutine(apiService.GetQuestions(OnQuestionsLoaded, OnError));
    }

    public void ChangeState(IQuizState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }
    public void OnQuestionsLoaded(QuestionData questionData)
    {
        questions = questionData;
        ChangeState(new WaitingForAnswerState());
    }

    public void OnError(string message)
    {
        Debug.LogError(message);
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
    

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartQuiz()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnAnswerButtonClicked(int index)
    {
        currentState?.OnAnswerSelected(this, index);
    }

    public void UpdateScore(int scoreChange)
    {
        OnScoreChanged?.Invoke(scoreChange);
    }
    
    
}
