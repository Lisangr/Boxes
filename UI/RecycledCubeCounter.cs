using UnityEngine;
using UnityEngine.UI;
using YG;

public class RecycledCubeCounter : MonoBehaviour
{
    public Text starsText; // Текст для отображения золота
    public Button upgradeButton; // Кнопка для апгрейда
    public Image StarsADImage; // Изображение с рекламой
    public Text upgradeLVL; // Текст уровня апгрейда

    private int recycledCounter; // Уровень апгрейда
    private int AdID = 1; // Уровень звездности
    private GridMovementChecker gridChecker; // Ссылка на GridMovementChecker

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
        LoadRecycledCounter(); // Загрузка значения при включении
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    private void Awake()
    {
        LoadRecycledCounter();
        UpdateUI();
    }

    private void LoadRecycledCounter()
    {
        recycledCounter = PlayerPrefs.GetInt("RecycledCounter", 1);
        CheckRecycledCounter();
    }

    private void SaveRecycledCounter()
    {
        PlayerPrefs.SetInt("RecycledCounter", recycledCounter);
        PlayerPrefs.Save();
    }

    private void CheckRecycledCounter()
    {
        if (recycledCounter > 0)
        {
            upgradeButton.interactable = true; 
            StarsADImage.gameObject.SetActive(false);
            Debug.Log("StarsADImage отключен");
        }
        else
        {
            upgradeButton.interactable = false;
            StarsADImage.gameObject.SetActive(true);
        }
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // Игнорируем события с другим ID

        AddStarsLVL();
        upgradeButton.interactable = true;
        StarsADImage.gameObject.SetActive(false);
        Debug.Log("StarsADImage отключен при выдаче награды");
    }

    private void AddStarsLVL()
    {
        recycledCounter++;
        SaveRecycledCounter();
        UpdateUI();
        Debug.Log("Я увеличил уровень КОЛИЧЕСТВО БУСТЕРОВ за рекламу, текущий уровень " + recycledCounter);
    }

    private void UpdateUI()
    {
        upgradeLVL.text = recycledCounter.ToString();
        Debug.Log("Текущий уровень АПГРЕЙДА ЗВЕЗДНОСТИ " + recycledCounter);
    }

    public void OnRecycledButtonClick()
    {
        gridChecker = FindObjectOfType<GridMovementChecker>();
        if (gridChecker == null)
        {
            Debug.LogError("GridMovementChecker not found in the scene!");
        }

        if (recycledCounter > 0 && gridChecker != null)
        {
            recycledCounter--;
            gridChecker.RemoveCube();
            CheckRecycledCounter();
            SaveRecycledCounter();
            UpdateUI();
        }
    }
}
