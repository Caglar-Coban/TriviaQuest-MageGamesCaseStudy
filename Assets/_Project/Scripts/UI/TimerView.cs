// UI/TimerView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerView : MonoBehaviour
{
    [SerializeField] private QuestionTimer timer;
    [SerializeField] private Image radialFill;       // TimerRing Image
    [SerializeField] private TMP_Text timerText;      // ortadaki sayı (opsiyonel)

    [Header("Colors")]
    [SerializeField] private Color fullTimeColor = Color.white;
    [SerializeField] private Color lowTimeColor = new Color(1f, 0.361f, 0.361f); // #FF5C5C
    [SerializeField] private float lowTimeThreshold = 0.3f; // son %30'da kırmızıya geçsin

    private void OnEnable() => timer.OnTick += HandleTick;
    private void OnDisable() => timer.OnTick -= HandleTick;

    private void HandleTick(float remaining, float total)
    {
        float ratio = remaining / total; // 1 -> 0

        radialFill.fillAmount = ratio;

        // Renk geçişi: sadece son %30'da beyazdan kırmızıya
        if (ratio <= lowTimeThreshold)
        {
            float t = ratio / lowTimeThreshold; // 1 -> 0 (threshold içinde)
            radialFill.color = Color.Lerp(lowTimeColor, fullTimeColor, t);
        }
        else
        {
            radialFill.color = fullTimeColor;
        }

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remaining).ToString();
    }
}