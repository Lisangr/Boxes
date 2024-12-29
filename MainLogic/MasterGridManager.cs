using System.Collections;
using UnityEngine;

public class MasterGridManager : MonoBehaviour
{
    public GridGenerator gridGenerator; // Ссылка на компонент генерации сетки
    public RandomRotateAndRemove rotateAndRemoveScript; // Ссылка на компонент вращения и удаления
    public GridMovementChecker movementChecker; // Ссылка на компонент проверки условий победы
    public GameObject generatorGO;
    private int currentLevelIndex;

    [ContextMenu("Generate and Process Level")]
    public void GenerateAndProcessLevel()
    {
        if (gridGenerator == null || rotateAndRemoveScript == null || movementChecker == null)
        {
            Debug.LogError("Не все компоненты подключены к мастер-скрипту!");
            return;
        }

        // Проверяем текущий уровень
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");

            if (currentLevelIndex <= 59) 
            {
                Debug.LogWarning("Уровень ЕЩЕ НЕ ПЕРЕВАЛИЛ за 3, генерация остановлена.");
                generatorGO.SetActive(false);
                return; // Остановить процесс, если уровень выше 3
            }
        }
        generatorGO.SetActive(true);
        StartCoroutine(GenerateAndProcessCoroutine());
    }

    private IEnumerator GenerateAndProcessCoroutine()
    {
        // Шаг 1: Генерация сетки
        Debug.Log("Шаг 1: Генерация уровня...");
        gridGenerator.GenerateGrid();

        // Ждем конца кадра, чтобы убедиться, что объекты появились в сцене
        yield return null;

        // Шаг 2: Вращение и удаление объектов
        Debug.Log("Шаг 2: Обработка объектов (вращение и удаление)...");
        rotateAndRemoveScript.StartProcessing();

        // Шаг 3: Проверка и установка кубиков
        Debug.Log("Шаг 3: Настройка движения и логики победы...");
        movementChecker.PopulateCubesList();

        Debug.Log("Генерация и обработка уровня завершена!");
    }
}
