using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GridPrefabEditor : EditorWindow
{
    private int gridSize = 5; // ������ �����
    private Vector2 scrollPosition;
    private CellData[,,] gridLevels; // ��������� ���� ������� (3D ������: ������, ����, �������)
    private int currentLevel = 0;
    private int totalLevels = 1;
    private GameObject prefab; // ������� ������
    private Vector2Int prefabSize = Vector2Int.one; // ������ ������� (�� ��������� 1x1)

    [MenuItem("Tools/Grid Prefab Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridPrefabEditor>("Grid Prefab Editor");
    }

    private void OnEnable()
    {
        gridLevels = new CellData[1, gridSize, gridSize];
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Prefab Editor", EditorStyles.boldLabel);

        // ��������� �����
        gridSize = EditorGUILayout.IntField("Grid Size", gridSize);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        // ����� ������� �������
        EditorGUILayout.LabelField("Prefab Size:");
        prefabSize = new Vector2Int(
            EditorGUILayout.IntField("Width (X)", prefabSize.x),
            EditorGUILayout.IntField("Height (Y)", prefabSize.y)
        );

        if (GUILayout.Button("Reset Grid"))
        {
            ResetGrid();
        }

        // ����� ������
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Current Level: {currentLevel + 1}/{totalLevels}");
        if (GUILayout.Button("Next Level"))
        {
            NextLevel();
        }

        // �����
        EditorGUILayout.LabelField("Edit Grid:");
        DrawGrid();

        // ����������
        EditorGUILayout.Space();
        if (GUILayout.Button("Finish and Generate"))
        {
            GenerateGrid();
        }
    }

    private void ResetGrid()
    {
        gridLevels = new CellData[1, gridSize, gridSize];
        currentLevel = 0;
        totalLevels = 1;
    }

    private void NextLevel()
    {
        totalLevels++;
        currentLevel = totalLevels - 1;

        // ��������� ������ ��� ������ ������
        var newGridLevels = new CellData[totalLevels, gridSize, gridSize];
        for (int i = 0; i < gridLevels.GetLength(0); i++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    newGridLevels[i, x, y] = gridLevels[i, x, y];
                }
            }
        }
        gridLevels = newGridLevels;
    }

    private void DrawGrid()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        for (int y = 0; y < gridSize; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < gridSize; x++)
            {
                CellData currentCell = gridLevels[currentLevel, x, y];
                bool isOccupied = currentCell != null;

                // ������ ������
                bool newValue = GUILayout.Toggle(isOccupied, GUIContent.none, GUILayout.Width(20), GUILayout.Height(20));

                // ��������� ��������� ������
                if (newValue != isOccupied)
                {
                    if (newValue)
                        PlacePrefabOnGrid(x, y);
                    else
                        RemovePrefabFromGrid(x, y);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void PlacePrefabOnGrid(int startX, int startY)
    {
        // ���������, ����� ������ �� ������� �� ������� �����
        if (startX + prefabSize.x > gridSize || startY + prefabSize.y > gridSize)
        {
            Debug.LogWarning("Prefab placement exceeds grid bounds!");
            return;
        }

        // ������������� ������, ������� �������� ������
        for (int x = 0; x < prefabSize.x; x++)
        {
            for (int y = 0; y < prefabSize.y; y++)
            {
                gridLevels[currentLevel, startX + x, startY + y] = new CellData
                {
                    prefab = prefab,
                    size = prefabSize
                };
            }
        }
    }

    private void RemovePrefabFromGrid(int startX, int startY)
    {
        // �������� ������ �� ������
        gridLevels[currentLevel, startX, startY] = null;
    }

    private void GenerateGrid()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned!");
            return;
        }

        GameObject parent = new GameObject("Generated Grid");

        for (int level = 0; level < totalLevels; level++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    CellData cell = gridLevels[level, x, y];
                    if (cell != null && cell.prefab != null)
                    {
                        Vector3 position = new Vector3(x, level, y);
                        Instantiate(cell.prefab, position, Quaternion.identity, parent.transform);
                    }
                }
            }
        }

        Debug.Log("Grid generated successfully!");
    }

    // ����� ��� �������� ������ � ������
    private class CellData
    {
        public GameObject prefab; // ������ ������
        public Vector2Int size;  // ������ ���������� ������
    }
}
