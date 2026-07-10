using Unity.VisualScripting;
using UnityEngine;

public class LeaderBoardPopUp : MonoBehaviour
{
    public GameObject popupPanel;
    public void Open()
    {
        popupPanel.SetActive(true);



    }

    public void Close()
    {
        popupPanel.SetActive(false);


    }

    public void loadItems()
    {
        
    }

    public void removeItems()
    {
        
    }

}
