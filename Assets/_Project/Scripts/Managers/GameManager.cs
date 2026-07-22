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


    private bool _isLeaderboardLoading = false;
    private bool _leaderboardCancelled = false;

    private bool _isMainmenuLoading = false;
    private bool _mainmenuCancelled = false;

    private bool _isGameplayLoading = false;
    private bool _gameplayCancelled = false;

    private bool _isGameoverLoading = false;

    private bool _gameoverCancelled = false;

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
        _isMainmenuLoading = true;
        _mainMenuHandle = mainMenuRef.InstantiateAsync();
            _mainMenuHandle.Completed += handle =>
            {
                _isMainmenuLoading = false;
                if (_mainmenuCancelled)
                {
                    Addressables.ReleaseInstance(handle);
                    _mainmenuCancelled = false;
                    return;
                }

                if(handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _mainMenuInstance = handle.Result;
                }
                else
                {
                    Debug.LogError("Failed to load Main Menu: " + handle.OperationException);
                    Addressables.Release(handle);
                }
            };
        
    }
    public void ReleaseMainMenu()
    {
         if (_isMainmenuLoading)
        {
            _mainmenuCancelled= true;
            return;
        }
        Addressables.ReleaseInstance(_mainMenuHandle);
        _mainMenuInstance = null;
        
    }


    public void ShowLeaderboard()
    {
        _isLeaderboardLoading = true;
        _leaderboardHandle = leaderboardRef.InstantiateAsync(_mainMenuInstance.transform); 
        _leaderboardHandle.Completed += handle =>
        {
            _isLeaderboardLoading = false;

            if (_leaderboardCancelled)
            {
                Addressables.ReleaseInstance(handle);
                _leaderboardCancelled = false;
                return;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _leaderboardInstance = handle.Result;
                _leaderboardInstance.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogError("Failed to load Leaderboard: " + handle.OperationException);
                Addressables.Release(handle);
            }
        };
    }

    public void ReleaseLeaderboard()
    {
        if (_isLeaderboardLoading)
        {
            _leaderboardCancelled = true;
            return;
        }
        Addressables.ReleaseInstance(_leaderboardHandle);
        _leaderboardInstance = null;
    }

    public void ShowGamePlay()
    {
        _isGameplayLoading = true;
        _gamePlayHandle = gamePlayRef.InstantiateAsync();
        _gamePlayHandle.Completed += handle =>
        {
            _isGameplayLoading = false;
            if (_gameplayCancelled)
            {
                Addressables.ReleaseInstance(handle);
                _gameplayCancelled = false;
                return;
            }
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _gamePlayInstance = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load Game Play: " + handle.OperationException);
                Addressables.Release(handle);
            }
        };

        
    }
    
    public void ReleaseGamePlay()
    {
        if (_isGameplayLoading )
        {
            _gameplayCancelled = true;
            return;

        }
        Addressables.ReleaseInstance(_gamePlayHandle);
        _gamePlayInstance = null;
    }

    public void ShowGameOver(int score)
    {
        _isGameoverLoading = true;
        _gameOverHandle = gameOverRef.InstantiateAsync(_gamePlayInstance.transform);
        _gameOverHandle.Completed += handle =>
        {
            _isGameoverLoading = false;
            if (_gameoverCancelled)
            {
                Addressables.ReleaseInstance(handle);
                _gameoverCancelled = false;
                return;

            }
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _gameOverInstance = handle.Result;
                _gameOverInstance.transform.SetAsLastSibling();
                _gameOverInstance.GetComponent<GameOverUI>().SetScore(score);
            }
            else
            {
                Debug.LogError("Failed to load Game Over: " + handle.OperationException);
                Addressables.Release(handle);
            }
        };
    }
    public void ReleaseGameOver()
    {
        if (_isGameoverLoading)
        {
            _gameoverCancelled = true;
            return;
        }
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
