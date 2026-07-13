using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class TimerSliderView : MonoBehaviour
{
    public Slider timerSlider;
    public Timer timer;
    public GameConfig gameConfig;
    private Image fillImage;
    private float duration;
    private float percentage;
    public void UpdateSlider(float value)
    {
        timerSlider.value = value;
    }
    void Start()
    {
        duration = gameConfig.questionduration;
        timerSlider.maxValue = 1f;
        timerSlider.value = 1f;
        if (timerSlider.fillRect != null)
        {
            fillImage = timerSlider.fillRect.GetComponent<Image>();
        }
    }
    public void Update()
    {
        if (timer == null || fillImage == null) return;

        percentage = timer.remaningTime / duration;
        Debug.Log("Percentage: " + percentage);
        if(percentage>0.3F)
        {
            UpdateSlider(percentage);
            fillImage.color = Color.white;
        }
        else
        {
            UpdateSlider(percentage);
            fillImage.color = Color.red;
        }
    }


    
}
