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
    public float spacing = 30f;
    public RectTransform viewport;
    public int buffer = 1;
    public LeaderBoardItem itemPrefab;
    public ApiService apiService;
    public GameConfig gameConfig;
    public int pagenumber = 0;
    public ScrollRect scrollRect;
    public List<IEnumerator> cachedCoroutines = new List<IEnumerator>();
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
        
    }

    private void OnError(string message)
    {
        Debug.LogError(message);
    }

    public void Close()
    {
        entries.Clear();
        foreach (var item in pool)
        {
        Destroy(item.gameObject);
        }
        pool.Clear();
        content.anchoredPosition = Vector2.zero;
        popupPanel.SetActive(false);


    }

    public void RefreshVisibleItems(){

        int firstVisibleIndex = Mathf.FloorToInt(content.anchoredPosition.y / (itemHeight + spacing));

        int maxFirstIndex = Mathf.Max(0, entries.Count - pool.Count);

        firstVisibleIndex = Mathf.Clamp(firstVisibleIndex, 0, maxFirstIndex);

        for(int i = 0; i< pool.Count;i++){

            int dataIndex = firstVisibleIndex + i;

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
    }


    


    public void UpdateContentSize()
    {
        float contentHeight = entries.Count * (itemHeight + spacing);
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }



}
