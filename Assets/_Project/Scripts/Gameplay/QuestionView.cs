// Gameplay/QuestionView.cs
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class QuestionView : MonoBehaviour
{
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private AnswerButton[] answerButtons;
    [SerializeField] private Transform gameOverParent; // Canvas_GameOverPanel referansı
    [SerializeField] private float resultDisplayDuration = 1.2f;

    public event Action<int> OnAnswerSelected;

    private GameObject _gameOverInstance;
    private TMP_Text _finalScoreText;

    private void Awake()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].Button.onClick.AddListener(() => OnAnswerSelected?.Invoke(index));
        }
    }

    private void OnDestroy()
    {
        if (_gameOverInstance != null)
        {
            Addressables.ReleaseInstance(_gameOverInstance);
        }
    }

    public void Display(Question question)
    {
        if (_gameOverInstance != null)
            _gameOverInstance.SetActive(false);

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
        StartCoroutine(ShowGameOverRoutine(finalScore));
    }

    // Örnek Game Over çağırma Coroutine'i
    // İçeriye int (tam sayı) bir skor veya bool bir durum geliyorsa buraya eklemelisin.
    // Ben örnek olarak "int score" yazdım, 77. satırda ne gidiyorsa onun tipini yazmalısın.
    private System.Collections.IEnumerator ShowGameOverRoutine(int score) // Parametre olarak final skoru aldığını varsayıyorum
    {
        var handle = UnityEngine.AddressableAssets.Addressables.InstantiateAsync("GameOverPanel"); 
        
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            GameObject gameOverObj = handle.Result;
            gameOverObj.transform.SetParent(this.transform.root, false); 
            
            // YENİ EKLENEN KISIM: Skoru Game Over paneline gönderiyoruz
            GameOverView view = gameOverObj.GetComponent<GameOverView>();
            if (view != null)
            {
                view.Setup(score); // final score'u panele iletiyoruz
            }
        }
        else
        {
            Debug.LogError("GameOverPanel yüklenemedi!");
        }
    }

    public void RestartGame()
    {
        StartCoroutine(LoadSceneAsync(SceneNames.GamePlay));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(LoadSceneAsync(SceneNames.MainMenu));
    }

    private IEnumerator LoadSceneAsync(string sceneAddress)
    {
        AsyncOperationHandle<SceneInstance> handle =
            Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single);

        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Scene load failed: {sceneAddress}");
        }
    }
}