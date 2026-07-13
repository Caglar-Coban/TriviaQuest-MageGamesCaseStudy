using UnityEngine;
using UnityEngine.UI;

public class TimerSliderView : MonoBehaviour
{
    [Header("References")]
    public Slider timerSlider;
    public Timer timer;
    
    private Image fillImage;

    void Start()
    {
        // 1. Slider atanmış mı kontrolü
        if (timerSlider == null)
        {
            Debug.LogError("HATA: TimerSliderView scriptinde 'timerSlider' boş! Lütfen Inspector'dan sürükleyin.");
            return;
        }

        // 2. Timer atanmış mı kontrolü
        if (timer == null)
        {
            Debug.LogError("HATA: TimerSliderView scriptinde 'timer' boş! Lütfen Inspector'dan sürükleyin.");
        }

        // 3. Fill Rect ve Image kontrolü
        if (timerSlider.fillRect != null)
        {
            fillImage = timerSlider.fillRect.GetComponent<Image>();
            if (fillImage == null)
            {
                Debug.LogError("HATA: Slider'ın 'Fill Rect' objesinde Image bileşeni YOK!");
            }
        }
        else
        {
            Debug.LogError("HATA: Slider bileşenindeki 'Fill Rect' kısmı BOŞ atanmış!");
        }

        timerSlider.maxValue = 1f;
        timerSlider.value = 1f;
    }

    void Update()
    {
        // Eğer yukarıdaki referanslardan biri bile yoksa, kod burada durur ve çubuk ASLA küçülmez!
        if (timer == null || fillImage == null || timerSlider == null) 
        {
            return; 
        }

        float maxDuration = timer.currentMaxDuration > 0f ? timer.currentMaxDuration : 1f;
        float percentage = timer.remaningTime / maxDuration;
        
        timerSlider.value = percentage;

        if (percentage > 0.3f)
        {
            fillImage.color = Color.white;
        }
        else
        {
            float lerpFactor = Mathf.InverseLerp(0.3f, 0f, percentage);
            fillImage.color = Color.Lerp(Color.white, Color.red, lerpFactor);
        }
    }
}