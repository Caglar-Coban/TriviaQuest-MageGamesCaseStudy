using UnityEngine;
using TMPro;
using System.Collections;
public class ScoreView : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public QuizManager quizManager;
    public GameConfig gameConfig;
    public float animationDuration;
    public int currentScore ;
    public Coroutine currentAnimation;

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

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimateScoreChange(currentScore, targetScore));
        currentScore = targetScore;
    }

    private IEnumerator AnimateScoreChange(int startScore, int targetScore)
    {
        
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            int displayedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
            scoreText.text = displayedScore.ToString();
            yield return null;
        }
        scoreText.text = targetScore.ToString();
        
    }
}
