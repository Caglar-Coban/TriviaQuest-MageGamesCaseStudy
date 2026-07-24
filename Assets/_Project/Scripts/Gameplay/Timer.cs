using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
public class Timer : MonoBehaviour
{
    public event Action<float> OnTimerTick;

    public event Action OnTimeUp;

    public float remaningTime;
    public float currentMaxDuration;

    private CancellationTokenSource _cancellationTokenSource;
    public void StartTimer(float duration)
    {
        StopTimer();
        currentMaxDuration = duration;
        _cancellationTokenSource = new CancellationTokenSource();
        RunTimer(duration, _cancellationTokenSource.Token);
        
    }

    public void StopTimer()
    {
        if (_cancellationTokenSource!= null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;


        }
    }
    public async void RunTimer(float duration,CancellationToken token)
    {
        try{
        float elapsedTime = 0f;
        remaningTime = duration;
        while (elapsedTime < duration)
        {
            token.ThrowIfCancellationRequested();
            elapsedTime += Time.deltaTime;
            remaningTime = Mathf.Max(0f, duration - elapsedTime);
            OnTimerTick?.Invoke(remaningTime);
            await Task.Yield();
        }
        OnTimeUp?.Invoke();
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
        StopTimer(); 
    }
}
