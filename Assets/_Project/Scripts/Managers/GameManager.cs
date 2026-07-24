using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;


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


    private CancellationTokenSource _mainMenuCts;
    private CancellationTokenSource _leaderboardCts;
    private CancellationTokenSource _gamePlayCts;
    private CancellationTokenSource _gameOverCts;

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
    //-------------------------------------------------------------------------------------------
    public async void ShowMainMenu()
    {
        if (_mainMenuHandle.IsValid())
        {
            return;
        }
        _mainMenuCts = new CancellationTokenSource();
        try
        {
            _mainMenuHandle = mainMenuRef.InstantiateAsync();
            _mainMenuInstance = await _mainMenuHandle.Task;


            _mainMenuCts.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            Addressables.ReleaseInstance(_mainMenuHandle);
            _mainMenuInstance = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Main Menu yüklenemedi: {ex.Message}");
            if (_mainMenuHandle.IsValid())
            {
                Addressables.Release(_mainMenuHandle);
            } 
        }
        
    }
    public void ReleaseMainMenu()
    {
        _mainMenuCts?.Cancel();
        _mainMenuCts?.Dispose();
        _mainMenuCts = null;

        if (_mainMenuHandle.IsValid())
        {
            Addressables.ReleaseInstance(_mainMenuHandle);
            _mainMenuInstance = null;
        }
    
        
    }

    //-------------------------------------------------------------------------------------------
    public async void ShowLeaderboard()
    {
        if (_leaderboardHandle.IsValid()) return;

        _leaderboardCts = new CancellationTokenSource();

        try
        {
            _leaderboardHandle = leaderboardRef.InstantiateAsync(_mainMenuInstance.transform);
            _leaderboardInstance = await _leaderboardHandle.Task;

            _leaderboardCts.Token.ThrowIfCancellationRequested();

            _leaderboardInstance.transform.SetAsLastSibling();
        }
        catch (OperationCanceledException)
        {
            Addressables.ReleaseInstance(_leaderboardHandle);
            _leaderboardInstance = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Leaderboard yüklenemedi: {ex.Message}");
            if (_leaderboardHandle.IsValid()) Addressables.Release(_leaderboardHandle);
        }
    }

    public void ReleaseLeaderboard()
    {
        _leaderboardCts?.Cancel();
        _leaderboardCts?.Dispose();
        _leaderboardCts = null;

        if (_leaderboardHandle.IsValid())
        {
            Addressables.ReleaseInstance(_leaderboardHandle);
            _leaderboardInstance = null;
        }
    }


    //-------------------------------------------------------------------------------------------


    public async void ShowGamePlay()
    {
        if (_gamePlayHandle.IsValid()) return;

        _gamePlayCts = new CancellationTokenSource();

        try
        {
            _gamePlayHandle = gamePlayRef.InstantiateAsync();
            _gamePlayInstance = await _gamePlayHandle.Task;

            _gamePlayCts.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            Addressables.ReleaseInstance(_gamePlayHandle);
            _gamePlayInstance = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Game Play yüklenemedi: {ex.Message}");
            if (_gamePlayHandle.IsValid()) Addressables.Release(_gamePlayHandle);
        }
    }

    public void ReleaseGamePlay()
    {
        _gamePlayCts?.Cancel();
        _gamePlayCts?.Dispose();
        _gamePlayCts = null;

        if (_gamePlayHandle.IsValid())
        {
            Addressables.ReleaseInstance(_gamePlayHandle);
            _gamePlayInstance = null;
        }
    }

    //-------------------------------------------------------------------------------------------

    public async void ShowGameOver(int score)
    {
        if (_gameOverHandle.IsValid()) return;

        _gameOverCts = new CancellationTokenSource();

        try
        {
            _gameOverHandle = gameOverRef.InstantiateAsync(_gamePlayInstance.transform);
            _gameOverInstance = await _gameOverHandle.Task;

            _gameOverCts.Token.ThrowIfCancellationRequested();

            _gameOverInstance.transform.SetAsLastSibling();
            _gameOverInstance.GetComponent<GameOverUI>().SetScore(score);
        }
        catch (OperationCanceledException)
        {
            Addressables.ReleaseInstance(_gameOverHandle);
            _gameOverInstance = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Game Over yüklenemedi: {ex.Message}");
            if (_gameOverHandle.IsValid()) Addressables.Release(_gameOverHandle);
        }
    }

    public void ReleaseGameOver()
    {
        _gameOverCts?.Cancel();
        _gameOverCts?.Dispose();
        _gameOverCts = null;

        if (_gameOverHandle.IsValid())
        {
            Addressables.ReleaseInstance(_gameOverHandle);
            _gameOverInstance = null;
        }
    }



    //-------------------------------------------------------------------------------------------


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
