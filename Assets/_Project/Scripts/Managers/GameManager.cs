using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class GameManager : MonoBehaviour
{
    public AssetReference mainMenuRef;
    public AssetReference leaderboardRef;
    public AssetReference gamePlayRef;
    public AssetReference gameOverRef;


    private AsyncOperationHandle<GameObject> _mainMenuHandle;
    private AsyncOperationHandle<GameObject> _leaderboardHandle;
    private AsyncOperationHandle<GameObject> _gamePlayHandle;
    private AsyncOperationHandle<GameObject> _gameOverHandle;

    private GameObject _mainMenuInstance;
    private GameObject _leaderboardInstance;
    private GameObject _gamePlayInstance;
    private GameObject _gameOverInstance;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        
        _mainMenuHandle = mainMenuRef.InstantiateAsync();
            _mainMenuHandle.Completed += handle =>
            {
                _mainMenuInstance = handle.Result;
            };
        
    }
    public void ReleaseMainMenu()
    {
        
        Addressables.ReleaseInstance(_mainMenuHandle);
        _mainMenuInstance = null;
        
    }


    public void ShowLeaderboard()
    {
        _leaderboardHandle = leaderboardRef.InstantiateAsync(_mainMenuInstance.transform);
        _leaderboardHandle.Completed += handle =>
        {
            _leaderboardInstance = handle.Result;
            _leaderboardInstance.transform.SetAsLastSibling();
        };
    }

    public void ReleaseLeaderboard()
    {
        Addressables.ReleaseInstance(_leaderboardHandle);
        _leaderboardInstance = null;
    }

    public void ShowGamePlay()
    {
        _gamePlayHandle = gamePlayRef.InstantiateAsync();
        _gamePlayHandle.Completed += handle =>
        {
            _gamePlayInstance = handle.Result;
        };

        
    }
    
    public void ReleaseGamePlay()
    {
        Addressables.ReleaseInstance(_gamePlayHandle);
        _gamePlayInstance = null;
    }

    public void ShowGameOver(int score)
    {
        _gameOverHandle = gameOverRef.InstantiateAsync(_gamePlayInstance.transform);
        _gameOverHandle.Completed += handle =>
        {
            _gameOverInstance = handle.Result;
            _gameOverInstance.transform.SetAsLastSibling();
            _gameOverInstance.GetComponent<GameOverUI>().SetScore(score);
        };
    }
    public void ReleaseGameOver()
    {
        Addressables.ReleaseInstance(_gameOverHandle);
        _gameOverInstance = null;
    }
    
    public void RestartGame()
    {
        ReleaseGameOver();
        ReleaseGamePlay();
        ShowGamePlay();
    }
    public void ReturnToMainMenu()
    {
        ReleaseGameOver();
        ReleaseGamePlay();
        ShowMainMenu();
    }
}
