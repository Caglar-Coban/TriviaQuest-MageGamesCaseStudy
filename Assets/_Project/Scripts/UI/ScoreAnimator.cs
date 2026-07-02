// UI/ScoreAnimator.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreAnimator : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private float animDuration = 0.5f;

    private Coroutine _animCoroutine;

    private void OnEnable() => scoreManager.OnScoreChanged += AnimateScore;
    private void OnDisable() => scoreManager.OnScoreChanged -= AnimateScore;

    private void AnimateScore(int oldScore, int newScore)
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(CountAnimation(oldScore, newScore));
    }

    private IEnumerator CountAnimation(int from, int to)
    {
        float t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            int value = Mathf.RoundToInt(Mathf.Lerp(from, to, t / animDuration));
            scoreText.text = value.ToString();
            yield return null;
        }
        scoreText.text = to.ToString();
    }
}