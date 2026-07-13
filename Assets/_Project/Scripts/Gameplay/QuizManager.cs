using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
public class QuizManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public QuestionData questions;

    public List<Button> answerButtons;
    public int currentQuestionIndex = 0;
    public IApiService apiService;
    public GameConfig gameConfig;
    private void Awake()
    {
        apiService = new ApiService(gameConfig); 
        
    }
    
    public void Start()
    {
        StartCoroutine(apiService.GetQuestions(OnQuestionsLoaded, OnError));
    }
    public void OnQuestionsLoaded(QuestionData questionData)
    {
        questions = questionData;
        DisplayQuestion(currentQuestionIndex);
        DisplayChoices(currentQuestionIndex);
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

    public void SubmitAnswer(int index)
    {
        if (index == questions.questions[currentQuestionIndex].CorrectAnswerIndex)
        {
            Debug.Log("Correct answer!");
            CorrectAnswer(index);
        }
        else
        {
            Debug.Log("Incorrect answer.");
            IncorrectAnswer(index);
        }
    }

    public void CorrectAnswer(int index)
    {
        answerButtons[index].GetComponent<Image>().color = Color.green;
        NextQuestion();
    }

    public void IncorrectAnswer(int index)
    {
        answerButtons[index].GetComponent<Image>().color = Color.red;
        answerButtons[questions.questions[currentQuestionIndex].CorrectAnswerIndex].GetComponent<Image>().color = Color.green;
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (currentQuestionIndex < questions.questions.Length -1)
        {
            currentQuestionIndex++;
            DisplayQuestion(currentQuestionIndex);
            DisplayChoices(currentQuestionIndex);
            ResetButtonColors();
            
        }
        else
        {
            Debug.Log("Quiz completed.");
        }


    }

    public void ResetButtonColors()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }
    
    }
