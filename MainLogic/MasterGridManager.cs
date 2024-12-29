using System.Collections;
using UnityEngine;

public class MasterGridManager : MonoBehaviour
{
    public GridGenerator gridGenerator; // ������ �� ��������� ��������� �����
    public RandomRotateAndRemove rotateAndRemoveScript; // ������ �� ��������� �������� � ��������
    public GridMovementChecker movementChecker; // ������ �� ��������� �������� ������� ������
    public GameObject generatorGO;
    private int currentLevelIndex;

    [ContextMenu("Generate and Process Level")]
    public void GenerateAndProcessLevel()
    {
        if (gridGenerator == null || rotateAndRemoveScript == null || movementChecker == null)
        {
            Debug.LogError("�� ��� ���������� ���������� � ������-�������!");
            return;
        }

        // ��������� ������� �������
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");

            if (currentLevelIndex <= 59) 
            {
                Debug.LogWarning("������� ��� �� ��������� �� 3, ��������� �����������.");
                generatorGO.SetActive(false);
                return; // ���������� �������, ���� ������� ���� 3
            }
        }
        generatorGO.SetActive(true);
        StartCoroutine(GenerateAndProcessCoroutine());
    }

    private IEnumerator GenerateAndProcessCoroutine()
    {
        // ��� 1: ��������� �����
        Debug.Log("��� 1: ��������� ������...");
        gridGenerator.GenerateGrid();

        // ���� ����� �����, ����� ���������, ��� ������� ��������� � �����
        yield return null;

        // ��� 2: �������� � �������� ��������
        Debug.Log("��� 2: ��������� �������� (�������� � ��������)...");
        rotateAndRemoveScript.StartProcessing();

        // ��� 3: �������� � ��������� �������
        Debug.Log("��� 3: ��������� �������� � ������ ������...");
        movementChecker.PopulateCubesList();

        Debug.Log("��������� � ��������� ������ ���������!");
    }
}
