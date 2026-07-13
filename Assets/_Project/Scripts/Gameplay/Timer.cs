using UnityEngine;
using System;
using System.Collections;
public class Timer : MonoBehaviour
{
    public event Action<float> OnTimerTick;

    public event Action OnTimeUp;


    private Coroutine timerCoroutine;
    public void StartTimer(float duration)
    {
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(RunTimer(duration));
        }
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
    public IEnumerator RunTimer(float duration)
    {
        float elapsedTime = 0f;
        float remaningTime = duration;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            remaningTime = Mathf.Max(0f, duration - elapsedTime);
            OnTimerTick?.Invoke(remaningTime);
            yield return null;
        }
        OnTimeUp?.Invoke();
    }
}
