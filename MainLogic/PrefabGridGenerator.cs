using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GridGenerator : MonoBehaviour
{
    [System.Serializable]
    public class GridObject
    {
        public GameObject prefab; // Префаб объекта
        public Vector3Int sizeXYZ; // Размер объекта (ширина, глубина, высота)
    }

    public int gridSizeX = 10; // Ширина сетки
    public int gridSizeZ = 10; // Глубина сетки
    public int gridSizeY = 5;  // Высота сетки (количество ярусов)
    public float cellSize = 1.0f; // Размер ячейки сетки

    public GridObject[] objectsToPlace; // Массив объектов для размещения
    private bool[,,] occupiedCells; // Массив занятых ячеек
    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (objectsToPlace == null || objectsToPlace.Length == 0)
        {
            Debug.LogError("No objects to place!");
            return;
        }

        // Удаляем предыдущие объекты
        ClearPreviousLevel();

        // Инициализируем сетку
        occupiedCells = new bool[gridSizeX, gridSizeY, gridSizeZ];

        // Генерация объектов
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    if (!occupiedCells[x, y, z])
                    {
                        TryPlaceObject(x, y, z);
                    }
                }
            }
        }

        Debug.Log("Grid generation complete.");
    }

    // Метод для удаления всех дочерних объектов
    private void ClearPreviousLevel()
    {
        List<GameObject> childrenToDelete = new List<GameObject>();

        // Сохраняем всех детей в список
        foreach (Transform child in transform)
        {
            childrenToDelete.Add(child.gameObject);
        }

        // Удаляем каждого ребёнка
        foreach (GameObject child in childrenToDelete)
        {
#if UNITY_EDITOR
            DestroyImmediate(child); // Удаление в редакторе
#else
        Destroy(child); // Удаление в рантайме
#endif
        }

        Debug.Log("Previous level cleared.");
    }

    private void TryPlaceObject(int startX, int startY, int startZ)
    {
        // Создаем список подходящих объектов
        List<GridObject> validObjects = new List<GridObject>();

        foreach (var gridObject in objectsToPlace)
        {
            if (CanPlaceObject(gridObject, startX, startY, startZ))
            {
                validObjects.Add(gridObject);
            }
        }

        // Если есть подходящие объекты, выбираем случайный из списка
        if (validObjects.Count > 0)
        {
            GridObject selectedObject = validObjects[Random.Range(0, validObjects.Count)];
            PlaceObject(selectedObject, startX, startY, startZ);
        }
    }

    private bool CanPlaceObject(GridObject gridObject, int startX, int startY, int startZ)
    {
        int objectWidth = gridObject.sizeXYZ.x;
        int objectHeight = gridObject.sizeXYZ.y;
        int objectDepth = gridObject.sizeXYZ.z;

        // Проверка выхода за границы
        if (startX + objectWidth > gridSizeX || startY + objectHeight > gridSizeY || startZ + objectDepth > gridSizeZ)
        {
            return false;
        }

        // Проверка занятости ячеек
        for (int x = 0; x < objectWidth; x++)
        {
            for (int y = 0; y < objectHeight; y++)
            {
                for (int z = 0; z < objectDepth; z++)
                {
                    if (occupiedCells[startX + x, startY + y, startZ + z])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
    private void PlaceObject(GridObject gridObject, int startX, int startY, int startZ)
    {
        int objectWidth = gridObject.sizeXYZ.x;
        int objectHeight = gridObject.sizeXYZ.y;
        int objectDepth = gridObject.sizeXYZ.z;

        // Рассчитываем смещение
        float offsetX = 0;
        float offsetZ = 0;

        // Смещение для кубиков 2x1x2
        if (objectWidth == 2 && objectDepth == 2)
        {
            offsetX = 0.5f * cellSize;
            offsetZ = 0.5f * cellSize;
        }
        // Смещение для кубиков 2x1
        else if (objectWidth == 2 && objectDepth == 1)
        {
            offsetX = 0.5f * cellSize;
        }
        // Смещение для кубиков 1x2
        else if (objectWidth == 1 && objectDepth == 2)
        {
            offsetZ = 0.5f * cellSize;
        }

        // Рассчитываем позицию объекта с учетом смещения
        Vector3 position = new Vector3(
            startX * cellSize + offsetX,
            startY * cellSize,
            startZ * cellSize + offsetZ
        );

        GameObject newObject = null;

#if UNITY_EDITOR
        // Создаём объект с использованием PrefabUtility
        newObject = PrefabUtility.InstantiatePrefab(gridObject.prefab, transform) as GameObject;
#else
    // В рантайме используем стандартный Instantiate
    newObject = Instantiate(gridObject.prefab, position, Quaternion.identity, transform);
#endif

        if (newObject != null)
        {
            // Устанавливаем позицию и ротацию
            newObject.transform.position = position;
            newObject.transform.rotation = Quaternion.identity;

            // Обновляем массив занятых ячеек
            for (int x = 0; x < objectWidth; x++)
            {
                for (int y = 0; y < objectHeight; y++)
                {
                    for (int z = 0; z < objectDepth; z++)
                    {
                        occupiedCells[startX + x, startY + y, startZ + z] = true;
                    }
                }
            }

            Debug.Log($"Placed {gridObject.prefab.name} at ({startX}, {startY}, {startZ}) with offset ({offsetX}, 0, {offsetZ})");
        }
        else
        {
            Debug.LogError($"Failed to spawn prefab: {gridObject.prefab.name}");
        }
    }
}