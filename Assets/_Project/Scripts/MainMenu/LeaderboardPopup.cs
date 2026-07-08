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
    [Header("Virtualization")]
    [SerializeField] private float itemHeight = 100f;
    [SerializeField] private int bufferItems = 2;
    [SerializeField] private int pageSize = 10;
    

    private int poolSize; // artık runtime'da viewport'a göre hesaplanıyor

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
        CalculatePoolSize();
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
    }
    private void EnsurePagesForRange(int startIndex, int count)
    {
        int firstPage = Mathf.Max(0, startIndex / pageSize);
        int lastPage = (startIndex + count - 1) / pageSize;

        Debug.Log($"[Leaderboard] Görünen aralık: index {startIndex}-{startIndex + count - 1} → gerekli sayfalar: {firstPage}-{lastPage}");

        EvictOutsideRange(firstPage, lastPage);

        for (int page = firstPage; page <= lastPage; page++)
        {
            LoadPageIfNeeded(page);
        }
    }

    private void EvictOutsideRange(int firstVisiblePage, int lastVisiblePage)
    {
        if (_pageCache.Count == 0) return;

        var toRemove = new List<int>();
        foreach (var page in _pageCache.Keys)
        {
            if (page < firstVisiblePage || page > lastVisiblePage)
            {
                toRemove.Add(page);
            }
        }

        foreach (var page in toRemove)
        {
            _pageCache.Remove(page);
            Debug.Log($"[Leaderboard] 🗑 Page {page} cache'ten SİLİNDİ (artık görünen aralıkta değil: {firstVisiblePage}-{lastVisiblePage}).");
        }
    }

    private void LoadPageIfNeeded(int page)
    {
        if (page < 0) return;

        if (_pageCache.ContainsKey(page))
        {
            Debug.Log($"[Leaderboard] Page {page} zaten cache'te, tekrar çekilmiyor.");
            return;
        }

        if (_pagesLoading.Contains(page))
        {
            Debug.Log($"[Leaderboard] Page {page} zaten yükleniyor, tekrar istek atılmıyor.");
            return;
        }

        if (_hasReachedEnd && page * pageSize >= _totalKnownCount)
        {
            Debug.Log($"[Leaderboard] Page {page} son sayfadan sonrası, istek atılmadı (liste bitti).");
            return;
        }

        _pagesLoading.Add(page);

        Debug.Log($"[Leaderboard] >>> Page {page} için network isteği BAŞLADI (key: page={page}).");

        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        StartCoroutine(_apiService.GetLeaderboard(page,
            result => OnPageLoaded(page, result),
            error => OnPageError(page, error)));
    }

    private void OnPageLoaded(int page, LeaderboardPage result)
    {
        _pagesLoading.Remove(page);

        Debug.Log($"[Leaderboard] <<< Page {page} BAŞARIYLA geldi. {result.data.Length} eleman, is_last={result.is_last}");

        if (!result.is_last && result.data.Length != pageSize)
        {
            Debug.LogWarning($"Page {page}: API {result.data.Length} eleman döndürdü ama pageSize={pageSize}. Uyuşmazlık var!");
        }

        _pageCache[page] = result.data;

        if (result.is_last)
        {
            _hasReachedEnd = true;
            _totalKnownCount = page * pageSize + result.data.Length;
            UpdateContentHeight();
        }
        else if (_totalKnownCount == int.MaxValue)
        {
            UpdateContentHeight();
        }

        if (_pagesLoading.Count == 0 && loadingIndicator != null)
            loadingIndicator.SetActive(false);

        _lastFirstVisibleIndex = -1;
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

    private void CalculatePoolSize()
    {
        float viewportHeight = scrollRect.viewport.rect.height;

        // Viewport henüz 0 boyuttaysa (bazı layout durumlarında Awake'te olabilir) güvenli bir minimuma düş
        if (viewportHeight <= 0f)
        {
            Canvas.ForceUpdateCanvases();
            viewportHeight = scrollRect.viewport.rect.height;
        }

        int visibleCount = Mathf.CeilToInt(viewportHeight / itemHeight);
        poolSize = Mathf.Max(1, visibleCount) + bufferItems;
    }
}