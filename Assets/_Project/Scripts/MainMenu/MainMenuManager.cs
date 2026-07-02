// MainMenu/MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LeaderboardPopup leaderboardPopup;

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(SceneNames.GamePlay);
    }

    public void OnLeaderboardClicked()
    {
        leaderboardPopup.Open();
    }
}