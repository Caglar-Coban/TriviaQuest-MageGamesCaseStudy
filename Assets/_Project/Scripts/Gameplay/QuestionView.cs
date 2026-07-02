// Gameplay/QuestionView.cs
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestionView : MonoBehaviour
{
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private AnswerButton[] answerButtons;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private float resultDisplayDuration = 1.2f;

    public event Action<int> OnAnswerSelected;

    private void Awake()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].Button.onClick.AddListener(() => OnAnswerSelected?.Invoke(index));
        }
    }

    public void Display(Question question)
    {
        gameOverPanel.SetActive(false);
        questionText.text = question.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].SetText(question.choices[i]);
            answerButtons[i].ResetColor();
            answerButtons[i].SetInteractable(true);
        }
    }

    public void ShowResult(int selectedIndex, int correctIndex, bool isCorrect, Action onComplete)
    {
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].SetInteractable(false);

        if (correctIndex >= 0 && correctIndex < answerButtons.Length)
            answerButtons[correctIndex].MarkCorrect();

        if (!isCorrect && selectedIndex >= 0 && selectedIndex < answerButtons.Length)
            answerButtons[selectedIndex].MarkWrong();

        StartCoroutine(WaitThenContinue(onComplete));
    }

    private IEnumerator WaitThenContinue(Action onComplete)
    {
        yield return new WaitForSeconds(resultDisplayDuration);
        onComplete?.Invoke();
    }

    public void ShowGameOver(int finalScore)
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Final Score: {finalScore}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneNames.GamePlay);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}