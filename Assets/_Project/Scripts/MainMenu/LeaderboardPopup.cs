// MainMenu/LeaderboardPopup.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentParent;
    [SerializeField] private LeaderboardItem itemPrefab;
    [SerializeField] private GameConfig config;
    [SerializeField] private GameObject loadingIndicator;

    [Header("Infinite Scroll")]
    [Tooltip("Scroll pozisyonu bu değerin altına inince yeni sayfa yüklenir (0 = en alt, 1 = en üst)")]
    [SerializeField] private float loadMoreThreshold = 0.2f;

    private IApiService _apiService;
    private readonly List<LeaderboardItem> _pooledItems = new List<LeaderboardItem>();

    private int _nextPage = 0;
    private bool _isLoading = false;
    private bool _isLastPage = false;

    private void Awake()
    {
        _apiService = new ApiService(config);
        popupRoot.SetActive(false);
    }

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }

    public void Open()
    {
        popupRoot.SetActive(true);

        // Reset state
        _nextPage = 0;
        _isLastPage = false;
        _isLoading = false;
        ClearList();

        LoadNextPage();
    }

    public void Close()
    {
        popupRoot.SetActive(false);
    }

    private void OnScrollChanged(Vector2 normalizedPos)
    {
        if (_isLoading || _isLastPage) return;

        // normalizedPos.y: 1 = en üstte, 0 = en altta
        if (scrollRect.verticalNormalizedPosition <= loadMoreThreshold)
        {
            LoadNextPage();
        }
    }

    private void LoadNextPage()
    {
        if (_isLoading || _isLastPage) return;

        _isLoading = true;
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        StartCoroutine(_apiService.GetLeaderboard(_nextPage, OnLeaderboardLoaded, OnError));
    }

    private void OnLeaderboardLoaded(LeaderboardPage result)
    {
        _isLoading = false;
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);

        _isLastPage = result.is_last;
        _nextPage = result.page + 1;

        AppendItems(result.data);
    }

    private void OnError(string error)
    {
        _isLoading = false;
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);

        Debug.LogError($"Leaderboard load failed: {error}");
    }

    // Yeni gelen verileri listenin SONUNA ekler (üzerine yazmaz)
    private void AppendItems(LeaderboardEntry[] entries)
    {
        foreach (var entry in entries)
        {
            LeaderboardItem item = GetOrCreateItem();
            item.gameObject.SetActive(true);
            item.Setup(entry);
        }
    }

    private LeaderboardItem GetOrCreateItem()
    {
        // Havuzda kullanılmayan (inactive) bir item var mı bak
        foreach (var pooled in _pooledItems)
        {
            if (!pooled.gameObject.activeSelf)
                return pooled;
        }

        LeaderboardItem newItem = Instantiate(itemPrefab, contentParent);
        _pooledItems.Add(newItem);
        return newItem;
    }

    private void ClearList()
    {
        foreach (var item in _pooledItems)
        {
            item.gameObject.SetActive(false);
        }

        scrollRect.verticalNormalizedPosition = 1f; // en üste sar
    }
}