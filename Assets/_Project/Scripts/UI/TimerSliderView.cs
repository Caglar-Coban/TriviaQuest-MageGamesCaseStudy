// UI/TimerSliderView.cs
using UnityEngine;
using UnityEngine.UI;

public class TimerSliderView : MonoBehaviour
{
    [SerializeField] private QuestionTimer timer;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage; // Slider > Fill Area > Fill

    [Header("Colors")]
    [SerializeField] private Color fullTimeColor = Color.white;
    [SerializeField] private Color lowTimeColor = new Color(1f, 0.361f, 0.361f);
    [SerializeField] private float lowTimeThreshold = 0.3f;

    private void OnEnable() => timer.OnTick += HandleTick;
    private void OnDisable() => timer.OnTick -= HandleTick;

    private void HandleTick(float remaining, float total)
    {
        float ratio = remaining / total;

        slider.value = ratio;

        if (ratio <= lowTimeThreshold)
        {
            float t = ratio / lowTimeThreshold;
            fillImage.color = Color.Lerp(lowTimeColor, fullTimeColor, t);
        }
        else
        {
            fillImage.color = fullTimeColor;
        }
    }
}