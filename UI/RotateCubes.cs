using UnityEngine;
using UnityEngine.UI;
using YG;

public class RotateCubes : MonoBehaviour
{
    public Text starsText; // Текст для отображения золота
    public Button upgradeButton; // Кнопка для апгрейда
    public Image StarsADImage; // Изображение с рекламой
    public Text upgradeLVL; // Текст уровня апгрейда

    private int rotatorCounter; // Уровень апгрейда
    private int AdID = 3; // вращение и удаление
    private GridMovementChecker gridChecker; // Ссылка на GridMovementChecker
    [SerializeField]private RandomRotateAndRemove rotateAndRemoveScript; // Ссылка на компонент вращения и удаления
    private void OnEnable()
    {
        rotateAndRemoveScript = FindAnyObjectByType<RandomRotateAndRemove>();
        YandexGame.RewardVideoEvent += Rewarded;
        ResetRotatorCounter(); // Установка счетчика при включении
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    private void Awake()
    {
        ResetRotatorCounter();
    }

    private void ResetRotatorCounter()
    {
        rotatorCounter = 1; // Счетчик всегда равен 1 при старте
        CheckRecycledCounter();
        UpdateUI(); // Обновляем интерфейс сразу после изменения значения
    }

    private void CheckRecycledCounter()
    {
        if (rotatorCounter > 0)
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
        rotatorCounter++;
        UpdateUI();
        Debug.Log("Я увеличил уровень КОЛИЧЕСТВО БУСТЕРОВ за рекламу, текущий уровень " + rotatorCounter);
    }

    private void UpdateUI()
    {
        if (upgradeLVL != null)
        {
            upgradeLVL.text = rotatorCounter.ToString();
            Debug.Log("Текущий уровень АПГРЕЙДА ЗВЕЗДНОСТИ " + rotatorCounter);
        }
        else
        {
            Debug.LogError("UI Text component (upgradeLVL) is not assigned!");
        }
    }
    public void OnRotatorButtonClick()
    {
        // Проверяем, создан ли уровень
        if (rotateAndRemoveScript == null)
        {
            rotateAndRemoveScript = FindObjectOfType<RandomRotateAndRemove>();
            if (rotateAndRemoveScript == null)
            {
                Debug.LogError("RandomRotateAndRemove не найден в сцене!");
                return;
            }
        }

        // Проверяем наличие GridMovementChecker
        gridChecker = FindObjectOfType<GridMovementChecker>();
        if (gridChecker == null)
        {
            Debug.LogError("GridMovementChecker не найден в сцене!");
            return;
        }

        if (rotatorCounter > 0 && gridChecker != null)
        {
            rotatorCounter--;
            Debug.Log("Шаг 2: Обработка объектов (вращение и удаление)...");

            // Запуск обработки объектов
            rotateAndRemoveScript.StartProcessing();

            CheckRecycledCounter();
            UpdateUI();
        }
    }
}
