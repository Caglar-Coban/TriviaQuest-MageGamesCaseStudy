using UnityEngine;
using UnityEngine.AddressableAssets;
using TMPro; // TextMeshPro kütüphanesi eklendi

public class GameOverView : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText; // Inspector'dan bağlayacağımız text

    // Bu metodu Game Over ekranı yaratıldığında skoru yazdırmak için çağıracağız
    public void Setup(int finalScore)
    {
        if (scoreText != null)
        {
            // İstersen sadece skoru yaz, istersen başına "Score: " ekle
            scoreText.text = "Score: " + finalScore.ToString(); 
        }
    }

    public void OnRestartClicked()
    {
        // 1. Adım: Yepyeni bir Gameplay_Root yüklüyoruz. 
        // DİKKAT: "GameplayPrefab" yazan yere kendi Addressables key'ini yaz!
        Addressables.InstantiateAsync("GameplayPrefab").Completed += handle => 
        {
            // 2. Adım: Yeni oyun başarıyla yüklendiğinde, ESKİ oyunu (ve bu paneli) siliyoruz.
            GameObject oldGameplayRoot = this.transform.root.gameObject;
            if (oldGameplayRoot != null)
            {
                Addressables.ReleaseInstance(oldGameplayRoot);
            }
        };
    }

    public void OnReturnMainMenuClicked()
    {
        MainMenuManager mainMenu = FindAnyObjectByType<MainMenuManager>(FindObjectsInactive.Include);
        if (mainMenu != null && mainMenu.mainMenuCanvas != null)
        {
            mainMenu.mainMenuCanvas.SetActive(true);
        }

        GameObject gameplayRoot = this.transform.root.gameObject; 
        if (gameplayRoot != null)
        {
            Addressables.ReleaseInstance(gameplayRoot);
        }
    }
}