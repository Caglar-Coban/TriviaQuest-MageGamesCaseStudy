// Gameplay/AnswerButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image background;

    private Color _defaultColor;

    public Button Button => button;

    private void Awake()
    {
        _defaultColor = background.color;
    }

    public void SetText(string text) => label.text = text;
    public void SetInteractable(bool value) => button.interactable = value;
    public void ResetColor() => background.color = _defaultColor;
    public void MarkCorrect() => background.color = Color.green;
    public void MarkWrong() => background.color = Color.red;
}