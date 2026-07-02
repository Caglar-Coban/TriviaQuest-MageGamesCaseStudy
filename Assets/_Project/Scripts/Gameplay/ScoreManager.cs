// Gameplay/ScoreManager.cs
using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<int, int> OnScoreChanged; // oldScore, newScore

    private int _score;
    public int CurrentScore => _score;

    public void AddScore(int amount)
    {
        int oldScore = _score;
        _score += amount;
        OnScoreChanged?.Invoke(oldScore, _score);
    }

    public void ResetScore()
    {
        _score = 0;
        OnScoreChanged?.Invoke(0, 0);
    }
}