// MainMenu/LeaderboardPopup.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private LeaderboardItem itemPrefab;
    [SerializeField] private GameConfig config;
    [SerializeField] private GameObject loadingIndicator;

    [Header("Virtualization")]
    [SerializeField] private float itemHeight = 100f;
    [SerializeField] private int poolSize = 12; // ekranda görünen + buffer
    [SerializeField] private int pageSize = 10; // API'nin bir sayfada döndürdüğü kayıt sayısı

    // Recycled pool: sabit sayıda GameObject
    private readonly List<RectTransform> _pooledItems = new List<RectTransform>();
    private readonly List<LeaderboardItem> _pooledScripts = new List<LeaderboardItem>();

    // Veri cache: sayfa numarası -> o sayfanın verileri
    private readonly Dictionary<int, LeaderboardEntry[]> _pageCache = new Dictionary<int, LeaderboardEntry[]>();
    private readonly HashSet<int> _pagesLoading = new HashSet<int>();

    private IApiService _apiService;
    private int _totalKnownCount = int.MaxValue; // is_last gelene kadar bilinmez
    private bool _hasReachedEnd = false;
    private int _lastFirstVisibleIndex = -1;

    private void Awake()
    {
        _apiService = new ApiService(config);
        popupRoot.SetActive(false);
        CreatePool();
    }

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }

    // --- Pool Oluşturma (bir kere, sabit sayıda) ---
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            LeaderboardItem item = Instantiate(itemPrefab, content);
            RectTransform rt = item.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);

            item.gameObject.SetActive(false);
            _pooledItems.Add(rt);
            _pooledScripts.Add(item);
        }
    }

    public void Open()
    {
        popupRoot.SetActive(true);

        // Reset state
        _pageCache.Clear();
        _pagesLoading.Clear();
        _hasReachedEnd = false;
        _totalKnownCount = int.MaxValue;
        _lastFirstVisibleIndex = -1;

        scrollRect.verticalNormalizedPosition = 1f;
        content.anchoredPosition = Vector2.zero;

        LoadPageIfNeeded(0);
    }

    public void Close()
    {
        UnityEngine.AddressableAssets.Addressables.ReleaseInstance(this.gameObject);
    }

    // --- Scroll Değiştiğinde: Hangi item'lar görünür hesapla, recycle et ---
    private void OnScrollChanged(Vector2 _)
    {
        RefreshVisibleItems();
    }

    private void RefreshVisibleItems()
    {
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollY = content.anchoredPosition.y; // 0'dan başlayıp aşağı indikçe artar (pozitif)

        // GÜNCELLEME: -1 yaparak 1 item üstte, 1 item altta yedek (buffer) bırakıyoruz
        int firstVisibleIndex = Mathf.Max(0, Mathf.FloorToInt(scrollY / itemHeight) - 1); 

        if (firstVisibleIndex == _lastFirstVisibleIndex) 
        {
            // Yine de yeni sayfa gerekebilir, kontrol et
            EnsurePagesForRange(firstVisibleIndex, poolSize);
            return;
        }
        _lastFirstVisibleIndex = firstVisibleIndex;

        for (int slot = 0; slot < _pooledItems.Count; slot++)
        {
            int dataIndex = firstVisibleIndex + slot;

            if (_hasReachedEnd && dataIndex >= _totalKnownCount)
            {
                _pooledScripts[slot].gameObject.SetActive(false);
                continue;
            }

            RectTransform rt = _pooledItems[slot];
            rt.anchoredPosition = new Vector2(0, -dataIndex * itemHeight);

            LeaderboardEntry entry = GetEntryAt(dataIndex);
            if (entry != null)
            {
                // Veri var, normal gösterim
                _pooledScripts[slot].gameObject.SetActive(true);
                _pooledScripts[slot].Setup(entry);
            }
            else
            {
                // ÇÖZÜM B: Veri henüz yok (yükleniyor) - gizlemek yerine placeholder göster
                _pooledScripts[slot].gameObject.SetActive(true);
                _pooledScripts[slot].ShowLoadingState(); 
            }
        }

        EnsurePagesForRange(firstVisibleIndex, poolSize);
    }

    private void EnsurePagesForRange(int startIndex, int count)
{
    int firstPage = Mathf.Max(0, startIndex / pageSize);
    // +1 buffer'ı kaldırıp, tam olarak kapsanan son sayfayı buluyoruz:
    int lastPage = (startIndex + count - 1) / pageSize; 

    for (int page = firstPage; page <= lastPage; page++)
    {
        LoadPageIfNeeded(page);
    }
}

    private void LoadPageIfNeeded(int page)
    {
        if (page < 0) return;
        if (_pageCache.ContainsKey(page)) return;
        if (_pagesLoading.Contains(page)) return;
        if (_hasReachedEnd && page * pageSize >= _totalKnownCount) return;

        _pagesLoading.Add(page);

        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        StartCoroutine(_apiService.GetLeaderboard(page, 
            result => OnPageLoaded(page, result), 
            error => OnPageError(page, error)));
    }

    private void OnPageLoaded(int page, LeaderboardPage result)
    {
        _pagesLoading.Remove(page);
        _pageCache[page] = result.data;

        if (result.is_last)
        {
            _hasReachedEnd = true;
            _totalKnownCount = page * pageSize + result.data.Length;
            UpdateContentHeight();
        }
        else if (_totalKnownCount == int.MaxValue)
        {
            // Henüz son sayfa bilinmiyor, content'i "tahmini" büyüklükte tut
            UpdateContentHeight();
        }

        if (_pagesLoading.Count == 0 && loadingIndicator != null)
            loadingIndicator.SetActive(false);

        _lastFirstVisibleIndex = -1; // zorla yenile
        RefreshVisibleItems();
    }

    private void OnPageError(int page, string error)
    {
        _pagesLoading.Remove(page);
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);

        Debug.LogError($"Leaderboard page {page} load failed: {error}");
    }

    private LeaderboardEntry GetEntryAt(int dataIndex)
    {
        int page = dataIndex / pageSize;
        int indexInPage = dataIndex % pageSize;

        if (_pageCache.TryGetValue(page, out var entries) && indexInPage < entries.Length)
        {
            return entries[indexInPage];
        }
        return null;
    }

    private void UpdateContentHeight()
    {
        // ÇÖZÜM C: İlk sayfa yüklenirken tahmini boşluğu azalt (pageSize / 2)
        int estimatedCount = _hasReachedEnd 
            ? _totalKnownCount 
            : (_pageCache.Count * pageSize) + (pageSize / 2); 

        float height = estimatedCount * itemHeight;
        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
    }
}