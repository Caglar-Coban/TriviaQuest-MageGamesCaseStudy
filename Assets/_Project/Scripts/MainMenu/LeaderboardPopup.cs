using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
using System;

public class LeaderBoardPopUp : MonoBehaviour
{
    public RectTransform content;
    public List<LeaderBoardItem> pool;
    public float itemHeight = 100f;
    public float spacing = 0f;
    public RectTransform viewport;
    public int buffer = 1;
    public LeaderBoardItem itemPrefab;
    public ApiService apiService;
    public GameConfig gameConfig;
    
    public ScrollRect scrollRect;

    public GameManager gameManager;
    public int lastvisibleindex;
    private int firstDataIndex;
    private bool isLoading = false;
    public int pagenumber = 0;
    private bool isLastPage = false;

    public Dictionary<int, LeaderBoardEntry[]> pageCache = new Dictionary<int, LeaderBoardEntry[]>();
    private HashSet<int> loadingPages = new HashSet<int>();
    public int pageSize = 10;
    public int totalItemCount = 0;
    

    private int lastVisibleToUser;
    private int firstVisibleToUser;
    

    private CancellationTokenSource _cancellationTokenSource;
    
    private void Awake()
    {
        apiService = new ApiService(gameConfig); 
        content.anchoredPosition = Vector2.zero;
        firstVisibleToUser = 0;
        lastVisibleToUser = 0;

        _cancellationTokenSource = new CancellationTokenSource();

        CheckAndLoadVisiblePages();
    }
    public void CloseButtonClicked()
    {

        if(_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
        GameManager.Instance.ReleaseLeaderboard();
    }

    private void OnDestroy()
    {
        if (_cancellationTokenSource != null)
        {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();  
        }
    
    }
    private void OnPageLoaded(int loadedPageNum, LeaderboardPage page)
    {
        loadingPages.Remove(loadedPageNum);
        pageCache[loadedPageNum] = page.data;

        Debug.Log("[Leaderboard] Page " + loadedPageNum + " loaded and added to cache");
        isLastPage = page.is_last;
        
        if (isLastPage) 
        {
            totalItemCount = (loadedPageNum * pageSize) + page.data.Length;
        } 
        else 
        {
            totalItemCount = Mathf.Max(totalItemCount, (loadedPageNum + 1) * pageSize + pageSize);
        }
        
        UpdateContentSize();
        EnsurePoolSize();
        RefreshVisibleItems();
    }
    public void RefreshVisibleItems()
    {
        int rawFirstIndex = Mathf.FloorToInt(content.anchoredPosition.y / (itemHeight + spacing));

        int safeTotalCount = Mathf.Max(0, totalItemCount);
        int maxFirstIndex = Mathf.Max(0, safeTotalCount - pool.Count);
        firstDataIndex = Mathf.Clamp(rawFirstIndex, 0, maxFirstIndex);

        for(int i = 0; i < pool.Count; i++)
        {
            int dataIndex = firstDataIndex + i;
            float y = -dataIndex * (itemHeight + spacing);

            int pageIndex = dataIndex / pageSize;
            int localIndex = dataIndex % pageSize;

            pool[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y); 

            if (pageCache.ContainsKey(pageIndex) && localIndex < pageCache[pageIndex].Length) 
            {  
                pool[i].gameObject.SetActive(true);
                pool[i].ShowData(pageCache[pageIndex][localIndex]); 
            } 
            else if (dataIndex < totalItemCount) 
            {  
                pool[i].gameObject.SetActive(true);
            }
            else 
            {
                pool[i].gameObject.SetActive(false);
            }
        }

        int visibleItemCount = Mathf.CeilToInt(viewport.rect.height / (itemHeight + spacing));

        
        int maxVisibleStart = Mathf.Max(0, safeTotalCount - visibleItemCount);
        firstVisibleToUser = Mathf.Clamp(rawFirstIndex, 0, maxVisibleStart); 
        
        
        if (safeTotalCount == 0) {
            lastVisibleToUser = Mathf.Max(0, visibleItemCount - 1);
        } else {
            lastVisibleToUser = Mathf.Min(firstVisibleToUser + visibleItemCount - 1, safeTotalCount - 1);
        }

        CheckAndLoadVisiblePages();
    }


    public void EnsurePoolSize()
    {
        int viewportHeight = Mathf.RoundToInt(viewport.rect.height);
        int visibleCount = Mathf.CeilToInt(viewportHeight / (itemHeight + spacing)) + buffer * 2;
        while (pool.Count < visibleCount)
        {
            LeaderBoardItem newItem = Instantiate(itemPrefab, content);
            pool.Add(newItem);
        }
    }


    public void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    public void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScroll);
    }
    
    public void OnScroll(Vector2 position)
    {
        RefreshVisibleItems();
        Debug.Log("User sees index: " + (firstVisibleToUser + 1) + " - " + (lastVisibleToUser + 1));
    }


    public void UpdateContentSize()
    {
        float contentHeight = totalItemCount * (itemHeight + spacing);
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    public void CheckAndLoadVisiblePages()
    {
       
        int startPage = firstVisibleToUser / pageSize;
        int endPage = lastVisibleToUser / pageSize;

        HashSet<int> neededPages = new HashSet<int>();

        for (int p = startPage; p <= endPage; p++)
        {
            if (isLastPage && (p * pageSize) >= totalItemCount) break;
            neededPages.Add(p);
        }

       
        foreach (int page in neededPages)
        {
            if (!pageCache.ContainsKey(page) && !loadingPages.Contains(page))
            {
                loadingPages.Add(page);
                Debug.Log($"[Leaderboard] + page {page} loading...");
                LoadPageData(page);
            }
        }

        
        List<int> pagesToRemove = new List<int>();
        foreach (int cachedPage in pageCache.Keys)
        {
            if (cachedPage < startPage|| cachedPage > endPage)
            {
                pagesToRemove.Add(cachedPage);
            }
        }

        foreach (int page in pagesToRemove)
        {
            pageCache.Remove(page);
            Debug.Log($"page {page} deleted from cache");
        }
    }

    private async void LoadPageData(int page)
    {
        try
        {
            LeaderboardPage pagedata = await apiService.GetLeaderBoard(page,_cancellationTokenSource.Token);
            OnPageLoaded(page,pagedata);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Download cancelled");
        }
        catch(Exception ex)
        {
            Debug.LogError($"Problem Occurred : {ex.Message}");
            loadingPages.Remove(page);
        }
    }
}