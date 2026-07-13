using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public LeaderBoardPopUp LeaderBoardPopup ;
    public void PlayButtonClicked()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void LeaderBoardButttonClicked()
    {
        LeaderBoardPopup.Open();
    }
}
