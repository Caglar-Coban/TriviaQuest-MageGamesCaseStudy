// Gameplay/QuestionTimer.cs
using System;
using System.Collections;
using UnityEngine;

public class QuestionTimer : MonoBehaviour
{
    public event Action OnTimeUp;
    public event Action<float, float> OnTick; // remaining, total

    private Coroutine _timerCoroutine;

    public void StartTimer(float duration)
    {
        StopTimer();
        _timerCoroutine = StartCoroutine(RunTimer(duration));
    }

    public void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private IEnumerator RunTimer(float duration)
    {
        float remaining = duration;
        while (remaining > 0f)
        {
            OnTick?.Invoke(remaining, duration);
            yield return null;
            remaining -= Time.deltaTime;
        }
        OnTick?.Invoke(0f, duration);
        OnTimeUp?.Invoke();
    }
}