using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LeaderBoardPopUp leaderboardpopup ;
    public void PlayButtonClicked()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void LeaderBoardButttonClicked()
    {
        leaderboardpopup.Open();
    }
}
