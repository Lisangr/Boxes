using UnityEngine;

public class ArrowsSpawner : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // ������ ��� ����������� ������� �������

    private void Update()
    {
        // ���������, ���� �� ������ ����� ������ ����
        if (Input.GetMouseButtonDown(0)) // 0 �������� ����� ������ ����
        {
            SpawnArrow();
        }
    }

    // ����� ��� ������ ������ �� ����������� �������
    public void SpawnArrow()
    {
        // �������� ������ �� ����
        ArrowMoving arrow = ArrowsPool.Instance.GetArrow();

        // ������ ������� ��� ������ � ��� ������ ��������� � ������������ �������
        Vector3 spawnPosition = GetCursorWorldPosition();
        arrow.transform.position = spawnPosition;

        // ����� ������ ����������� ������, ��������, "������" �� ���������
        arrow.transform.rotation = Quaternion.identity;
    }

    // ����� ��� ���������� ��������� ������� � ������� ������������
    private Vector3 GetCursorWorldPosition()
    {
        // �������� ������� ������� � ������������ ������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ����������, ���� �������� ��� (��������, �� ����� ��� ������ ������)
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; // ���������� ����� �����������
        }

        // ���� ��� ������ �� �����, ���������� ���������� � ��������� XZ �� �������� ������ (��������, 0)
        return ray.origin + ray.direction * 10f; // ����� �� ��������� ���������� �� ������
    }
}
