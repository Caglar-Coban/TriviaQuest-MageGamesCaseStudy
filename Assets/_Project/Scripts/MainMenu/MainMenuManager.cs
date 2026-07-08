using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Transforms & UI")]
    [SerializeField] private Transform canvasBackgroundTransform;
    
    // GameOverView scriptinden erişebilmek için public yaptık ve sadece BİR TANE var.
    [Tooltip("Gameplay açıldığında gizlenecek Canvas'ı buraya sürükleyin")]
    public GameObject mainMenuCanvas; 

    [Header("Addressable Keys")]
    [SerializeField] private string gameplayPrefabKey = "GameplayPrefab"; 

    private LeaderboardPopup _activeLeaderboardPopup;
    private GameObject _activeGameplayInstance;

    public void OnPlayClicked()
    {
        StartCoroutine(LoadGameplayAsync());
    }

    private IEnumerator LoadGameplayAsync()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(gameplayPrefabKey);

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _activeGameplayInstance = handle.Result;

            GameManager.Instance.ReleaseMainMenu();
        }
        else
        {
            Debug.LogError($"Gameplay prefab yüklenemedi! Key'i kontrol et: {gameplayPrefabKey}");
        }
    }

    public void OnLeaderboardClicked()
    {
        if (_activeLeaderboardPopup != null)
        {
            _activeLeaderboardPopup.Open();
            return;
        }

        StartCoroutine(LoadAndOpenLeaderboard());
    }

    private IEnumerator LoadAndOpenLeaderboard()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync("LeaderboardPopup");

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject popupObj = handle.Result;
            
            // Obje yaratıldıktan sonra parent ataması yapıyoruz
            popupObj.transform.SetParent(canvasBackgroundTransform, false);

            _activeLeaderboardPopup = popupObj.GetComponent<LeaderboardPopup>();
            _activeLeaderboardPopup.Open();
        }
        else
        {
            Debug.LogError("LeaderboardPopup yüklenemedi!");
        }
    }

    private void OnDestroy()
    {
        if (_activeLeaderboardPopup != null)
        {
            Addressables.ReleaseInstance(_activeLeaderboardPopup.gameObject);
        }

       
    }
}