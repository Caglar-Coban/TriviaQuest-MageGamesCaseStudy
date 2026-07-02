// Data/Question.cs
using System;

[Serializable]
public class Question
{
    public string category;
    public string question;
    public string[] choices;
    public string answer; // "A", "B", "C" veya "D"

    // Harfi index'e çeviren yardımcı property
    public int CorrectAnswerIndex
    {
        get
        {
            switch (answer)
            {
                case "A": return 0;
                case "B": return 1;
                case "C": return 2;
                case "D": return 3;
                default: return -1;
            }
        }
    }
}

[Serializable]
public class QuestionData
{
    public Question[] questions;
}