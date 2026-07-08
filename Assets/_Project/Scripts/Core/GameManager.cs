using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Addressable Keys")]
    [SerializeField] private string mainMenuPrefabKey = "MainMenuPrefab";

    private GameObject _activeMainMenuInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        if (_activeMainMenuInstance != null)
        {
            _activeMainMenuInstance.SetActive(true);
            return;
        }

        StartCoroutine(LoadMainMenuAsync());
    }

    private IEnumerator LoadMainMenuAsync()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(mainMenuPrefabKey);

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _activeMainMenuInstance = handle.Result;
        }
        else
        {
            Debug.LogError($"MainMenu prefab yüklenemedi! Key'i kontrol et: {mainMenuPrefabKey}");
        }
    }

    public void ReleaseMainMenu()
    {
        if (_activeMainMenuInstance != null)
        {
            Addressables.ReleaseInstance(_activeMainMenuInstance);
            _activeMainMenuInstance = null;
        }
    }
}