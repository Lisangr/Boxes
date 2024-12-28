using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GridGenerator : MonoBehaviour
{
    [System.Serializable]
    public class GridObject
    {
        public GameObject prefab; // ������ �������
        public Vector3Int sizeXYZ; // ������ ������� (������, �������, ������)
    }

    public int gridSizeX = 10; // ������ �����
    public int gridSizeZ = 10; // ������� �����
    public int gridSizeY = 5;  // ������ ����� (���������� ������)
    public float cellSize = 1.0f; // ������ ������ �����

    public GridObject[] objectsToPlace; // ������ �������� ��� ����������
    private bool[,,] occupiedCells; // ������ ������� �����
    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (objectsToPlace == null || objectsToPlace.Length == 0)
        {
            Debug.LogError("No objects to place!");
            return;
        }

        // ������� ���������� �������
        ClearPreviousLevel();

        // �������������� �����
        occupiedCells = new bool[gridSizeX, gridSizeY, gridSizeZ];

        // ��������� ��������
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

    // ����� ��� �������� ���� �������� ��������
    private void ClearPreviousLevel()
    {
        List<GameObject> childrenToDelete = new List<GameObject>();

        // ��������� ���� ����� � ������
        foreach (Transform child in transform)
        {
            childrenToDelete.Add(child.gameObject);
        }

        // ������� ������� ������
        foreach (GameObject child in childrenToDelete)
        {
#if UNITY_EDITOR
            DestroyImmediate(child); // �������� � ���������
#else
        Destroy(child); // �������� � ��������
#endif
        }

        Debug.Log("Previous level cleared.");
    }

    private void TryPlaceObject(int startX, int startY, int startZ)
    {
        // ������� ������ ���������� ��������
        List<GridObject> validObjects = new List<GridObject>();

        foreach (var gridObject in objectsToPlace)
        {
            if (CanPlaceObject(gridObject, startX, startY, startZ))
            {
                validObjects.Add(gridObject);
            }
        }

        // ���� ���� ���������� �������, �������� ��������� �� ������
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

        // �������� ������ �� �������
        if (startX + objectWidth > gridSizeX || startY + objectHeight > gridSizeY || startZ + objectDepth > gridSizeZ)
        {
            return false;
        }

        // �������� ��������� �����
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

        // ������������ ��������
        float offsetX = 0;
        float offsetZ = 0;

        // �������� ��� ������� 2x1x2
        if (objectWidth == 2 && objectDepth == 2)
        {
            offsetX = 0.5f * cellSize;
            offsetZ = 0.5f * cellSize;
        }
        // �������� ��� ������� 2x1
        else if (objectWidth == 2 && objectDepth == 1)
        {
            offsetX = 0.5f * cellSize;
        }
        // �������� ��� ������� 1x2
        else if (objectWidth == 1 && objectDepth == 2)
        {
            offsetZ = 0.5f * cellSize;
        }

        // ������������ ������� ������� � ������ ��������
        Vector3 position = new Vector3(
            startX * cellSize + offsetX,
            startY * cellSize,
            startZ * cellSize + offsetZ
        );

        GameObject newObject = null;

#if UNITY_EDITOR
        // ������ ������ � �������������� PrefabUtility
        newObject = PrefabUtility.InstantiatePrefab(gridObject.prefab, transform) as GameObject;
#else
    // � �������� ���������� ����������� Instantiate
    newObject = Instantiate(gridObject.prefab, position, Quaternion.identity, transform);
#endif

        if (newObject != null)
        {
            // ������������� ������� � �������
            newObject.transform.position = position;
            newObject.transform.rotation = Quaternion.identity;

            // ��������� ������ ������� �����
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