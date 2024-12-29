using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class CanvasButtons : MonoBehaviour
{
    public GameObject helpPanel;
    public GameObject settingsPanel;
    public GameObject menuPanel;
    public GameObject leadersPanel;
    public GameObject defeatPanel;
    public GameObject winPanel;
    public Text counterText; // Ссылка на текстовый объект для счетчика
    public GameObject startCanvas;
    public GameObject gameCanvas;
    public GameObject x2Button;
    public GameObject recycledButton;
    private int currentLevelIndex;
    public GameObject[] prefabs;
    public Transform spawnPoint;
    private int AdID = 2; //дубль награды
    private int points;
    private bool isRewardedDone = false;
    private int newPointsRezult;

    public delegate void DestroyAction();
    public static event DestroyAction OnDestroed;

    [SerializeField] private LevelManager levelManager;


    private void OnEnable()
    {
        UpdateCounterText(); // Обновление UI при старте

        YandexGame.RewardVideoEvent += Rewarded;
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // Игнорируем события с другим ID

        DoubleExpAndGold();
        x2Button.gameObject.SetActive(false);
    }
    private void DoubleExpAndGold()
    {
        int currentPoints = PlayerPrefs.GetInt("CurrentPoints", 0);
        currentPoints *= 2; // Удваиваем текущие очки

        PlayerPrefs.SetInt("CurrentPoints", currentPoints);
        PlayerPrefs.Save();
        points = currentPoints + PlayerPrefs.GetInt("Points", 0);
        UpdateCounterText();
    }

    private void UpdateCounterText()=>        counterText.text = points.ToString();
   
    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }
    private void Awake()
    {
        points = PlayerPrefs.GetInt("Points", 0);

        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }
        else
        {
            currentLevelIndex = 1; // Устанавливаем уровень по умолчанию
            PlayerPrefs.SetInt("Level", currentLevelIndex);
            PlayerPrefs.Save();
        }

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    private void Start()=>        OnCloseButtonClick();    

    public void OnCloseButtonClick()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(false);
        if (leadersPanel != null) leadersPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        UpdateX2ButtonState();
        recycledButton.SetActive(false);
    }

    public void ShowDefeatPanel()=>        defeatPanel.SetActive(true);
    public void OnHelpButtonClick()=>        helpPanel.SetActive(true);  
    public void OnSettingsButtonClick()=>        settingsPanel.SetActive(true);    
    public void OnLeadersButtonClick()=>        leadersPanel.SetActive(true);
    /*
    public void OnStartButtonClick()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }

        gameCanvas.SetActive(true);
        startCanvas.SetActive(false);

        // Инстанцируем префаб для текущего уровня
        GameObject instantiatedPrefab = InstantiatePrefabForCurrentLevel();

        // Попробуем найти LevelManager после инстанцирования
        levelManager = instantiatedPrefab.GetComponentInChildren<LevelManager>();

        if (levelManager != null)
        {
            // Восстанавливаем уровень из JSON
            levelManager.LoadLevelFromJSON();
        }
        else
        {
            Debug.LogWarning("LevelManager не найден в инстанцированном префабе!");
        }
    }
    */
    [SerializeField]private RandomRotateAndRemove rotateAndRemoveScript;
    public void OnStartButtonClick()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }

        gameCanvas.SetActive(true);
        startCanvas.SetActive(false);

        GameObject instantiatedPrefab = InstantiatePrefabForCurrentLevel();

        levelManager = instantiatedPrefab.GetComponentInChildren<LevelManager>();
        if (levelManager != null)
        {
            levelManager.LoadLevelFromJSON();
        }
        else
        {
            Debug.LogWarning("LevelManager не найден в инстанцированном префабе!");
        }

        // Проверяем наличие скрипта RandomRotateAndRemove после загрузки
        rotateAndRemoveScript = FindObjectOfType<RandomRotateAndRemove>();
        if (rotateAndRemoveScript == null)
        {
            Debug.LogError("RandomRotateAndRemove не найден после загрузки уровня!");
        }
    }

    public void OnGoToMenuButton()
    {
        DeactivateCurrentLevelPrefab();

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }
    public void OnExitButtonClick()
    {
        YandexGame.FullscreenShow();
        PlayerPrefs.SetInt("Level", currentLevelIndex + 1);

        int currentPoints = PlayerPrefs.GetInt("CurrentPoints", 0);
        int allPoints = PlayerPrefs.GetInt("Points", 0);
        newPointsRezult = currentPoints + allPoints;

        YandexGame.NewLeaderboardScores("Points", newPointsRezult);
        PlayerPrefs.SetInt("Points", newPointsRezult);
        PlayerPrefs.DeleteKey("CurrentPoints");
        PlayerPrefs.Save();        

        DeactivateCurrentLevelPrefab();
        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);

        isRewardedDone = false;
        UpdateX2ButtonState(); // Обновляем кнопку
        UpdateCounterText(); // Обновление UI при старте
    }

    public void OnPauseMenuClick()=>        menuPanel.SetActive(true);

    public void RestartCurrentScene()
    {
        YandexGame.FullscreenShow();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void RestartCurrentLevel()
    {
        YandexGame.FullscreenShow();
        DeactivateCurrentLevelPrefab();
        ResetRecycledCounter(); // Сброс recycledCounter
        UpdateCounterText(); // Обновление UI при старте
        InstantiatePrefabForCurrentLevel();

        // Восстанавливаем уровень из JSON после перезапуска
        levelManager = FindAnyObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.LoadLevelFromJSON();
        }
        else
        {
            Debug.LogWarning("LevelManager не назначен!");
        }
    }
    private GameObject InstantiatePrefabForCurrentLevel()
    {
        if (prefabs != null && prefabs.Length > 0 && spawnPoint != null)
        {
            int index = Mathf.Clamp(currentLevelIndex - 1, 0, prefabs.Length - 1);
            GameObject prefabToInstantiate = prefabs[index];

            if (prefabToInstantiate != null)
            {
                // Инстанцируем префаб
                GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, spawnPoint.position, spawnPoint.rotation);

                // Убираем "(Clone)" из имени объекта
                instantiatedPrefab.name = prefabToInstantiate.name;

                return instantiatedPrefab;
            }
        }
        Debug.LogWarning("Префаб для уровня не найден!");
        return null;
    }
    private void DeactivateCurrentLevelPrefab()
    {
        OnDestroed?.Invoke();      
    }
    private void UpdateX2ButtonState()
    {
        x2Button.SetActive(!isRewardedDone);
        UpdateCounterText(); // Обновление UI при старте
    }
    private void ResetRecycledCounter()
    {
        PlayerPrefs.SetInt("RecycledCounter", 1); // Сохраняем сброс
        PlayerPrefs.Save();
    }
}