using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class LeaderBoardPopUp : MonoBehaviour
{
    public GameObject popupPanel;
    public RectTransform content;
    public List<LeaderBoardEntry> entries;
    public List<LeaderBoardItem> pool;
    public float itemHeight = 100f;
    public float spacing = 0f;
    public RectTransform viewport;
    public int buffer = 1;
    public LeaderBoardItem itemPrefab;
    public ApiService apiService;
    public GameConfig gameConfig;
    
    public ScrollRect scrollRect;


    public int lastvisibleindex;
    private int firstDataIndex;
    private bool isLoading = false;
    public int pagenumber = 0;
    private bool isLastPage = false;
    

    private int lastVisibleToUser;
    private int firstVisibleToUser;
    
    
    private void Awake()
    {
        apiService = new ApiService(gameConfig); 
    }
    public void Open()
    {
        popupPanel.SetActive(true);
       

        StartCoroutine(apiService.GetLeaderBoard(pagenumber, OnPageLoaded, OnError));
        

    }

    private void OnPageLoaded(LeaderboardPage page)
    {
       
        entries.AddRange(page.data);
        UpdateContentSize();
        EnsurePoolSize();
        RefreshVisibleItems();
        isLoading = false;
        isLastPage = page.is_last;
    }

    private void OnError(string message)
    {
        Debug.LogError(message);
        isLoading = false;
    }

    public void Close()
    {
        entries.Clear();
        foreach (var item in pool)
        {
        Destroy(item.gameObject);
        }
        pagenumber = 0;
        pool.Clear();
        content.anchoredPosition = Vector2.zero;
        popupPanel.SetActive(false);


    }

    public void RefreshVisibleItems(){

        int rawFirstIndex = Mathf.FloorToInt(content.anchoredPosition.y / (itemHeight + spacing));

        int maxFirstIndex = Mathf.Max(0, entries.Count - pool.Count);

        firstDataIndex = Mathf.Clamp(rawFirstIndex, 0, maxFirstIndex);

        for(int i = 0; i< pool.Count;i++){

            int dataIndex = firstDataIndex + i;

            float y = -dataIndex * (itemHeight + spacing);

            if (dataIndex < entries.Count) {   
                pool[i].gameObject.SetActive(true);
                pool[i].ShowData(entries[dataIndex]);
                pool[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y); 
                
            } 
            else{   
                pool[i].gameObject.SetActive(false);
                
            }
        }

        int visibleItemCount = Mathf.CeilToInt(viewport.rect.height / (itemHeight + spacing));

        int maxVisibleStart = Mathf.Max(0, entries.Count - visibleItemCount);
        firstVisibleToUser = Mathf.Clamp(rawFirstIndex + buffer, 0, maxVisibleStart);
        lastVisibleToUser = Mathf.Min(firstVisibleToUser + visibleItemCount - 1, entries.Count - 1);
        CheckLoadMore();

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
        Debug.Log("User sees first visible index: " + (firstVisibleToUser + 1) + " and last visible index: " + (lastVisibleToUser + 1));
    }


    


    public void UpdateContentSize()
    {
        float contentHeight = entries.Count * (itemHeight + spacing);
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    public void CheckLoadMore()
    {
         lastvisibleindex = firstDataIndex + pool.Count - 1;

    if (lastvisibleindex  + 1 >= entries.Count && !isLoading  && !isLastPage)
    {
        isLoading = true;
        pagenumber++;
        StartCoroutine(apiService.GetLeaderBoard(pagenumber, OnPageLoaded, OnError));
        

    }

    
    }

    

}
