using System;

[Serializable]
public class Question
{
    public string category;
    public string question;
    public string[] choices;
    public string answer;



    public int CorrectAnswerIndex
    {
        get
        {
            if (answer == "A")
            {
                return 0 ;
            }
            else if (answer == "B")
            {
                return 1 ;
            }
            else if (answer == "C")
            {
                return 2 ;
            }
            else
            {
                return 3;
            }

        }
    } 
}

[Serializable]
public class QuestionData
{
    public Question[] questions;
}