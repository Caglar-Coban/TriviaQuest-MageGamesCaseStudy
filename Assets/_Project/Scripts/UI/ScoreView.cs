using UnityEngine;
using TMPro;
using System.Threading;
using System.Threading.Tasks;
using System;
public class ScoreView : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public QuizManager quizManager;
    public GameConfig gameConfig;
    public float animationDuration;
    public int currentScore ;
    private CancellationTokenSource _cancellationTokenSource;



    public void Start()
    {
        animationDuration = gameConfig.resultDisplayDuration / 2f;
        currentScore = quizManager.score;
        scoreText.text = currentScore.ToString();
    }

    public void OnEnable()
    {
        quizManager.OnScoreChanged += AddScore;
    }

    public void OnDisable()
    {
        quizManager.OnScoreChanged -= AddScore;
    }
    public void AddScore(int score)
    {
        int targetScore = currentScore + score;

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new CancellationTokenSource();
        AnimateScoreChange(currentScore,targetScore, _cancellationTokenSource.Token);
        currentScore = targetScore;
    }

    private async void AnimateScoreChange(int startScore, int targetScore, CancellationToken token)
    {
        try{
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            token.ThrowIfCancellationRequested();

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            int displayedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
            scoreText.text = displayedScore.ToString();
            
            await Task.Yield();
        }
        scoreText.text = targetScore.ToString();
    }
        catch (OperationCanceledException)
        {
            
        }
        catch(Exception ex)
        {
            Debug.LogError($"Problem Occurred : {ex.Message}");
        }
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
