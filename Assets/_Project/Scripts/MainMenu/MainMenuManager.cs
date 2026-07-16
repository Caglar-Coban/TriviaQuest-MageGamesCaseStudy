using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{

    public void PlayButtonClicked()
    {
        GameManager.Instance.ReleaseMainMenu();
        GameManager.Instance.ShowGamePlay();
    }

    public void LeaderBoardButttonClicked()
    {
        GameManager.Instance.ShowLeaderboard();
    }
}
