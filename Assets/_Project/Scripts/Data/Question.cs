using System;
using System.Diagnostics;

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
            else if (answer  == "D")
            {
                return 3;
            }
            else
            {
                return -1;
            }

        }
    } 
}

[Serializable]
public class QuestionData
{
    public Question[] questions;
}