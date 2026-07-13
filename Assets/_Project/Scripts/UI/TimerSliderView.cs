using UnityEngine;
using UnityEngine.UI;
public class TimerSliderView : MonoBehaviour
{
    public Slider timerSlider;
    public Timer timer;
    private Image fillImage;

    void Start()
    {
        timerSlider.maxValue = 1f;
        timerSlider.value = 1f;

        if (timerSlider.fillRect != null)
        {
            fillImage = timerSlider.fillRect.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (timer != null)
            timer.OnTimerTick += HandleTick;
    }

    private void OnDisable()
    {
        if (timer != null)
            timer.OnTimerTick -= HandleTick;
    }

    private void HandleTick(float remainingTime)
    {
        if (timer.currentMaxDuration > 0)
        {
            float normalizedValue = remainingTime / timer.currentMaxDuration;
            timerSlider.value = normalizedValue;

            if (fillImage != null && normalizedValue < 0.3f)
            {
                float lerpFactor = Mathf.InverseLerp(0.3f, 0f, normalizedValue);
                fillImage.color = Color.Lerp(Color.white, Color.red, lerpFactor);
            }
            else if (fillImage != null)
            {
                fillImage.color = Color.white;
            }
        }
    }
}