using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public string gridTag = "Grid"; // ��� ��� ������ ������� Grid
    public float rotationSpeed = 100.0f; // �������� �������� ������
    public float zoomSpeed = 10.0f; // �������� ���������������
    public float minOrthographicSize = 5.0f; // ����������� ������ ������
    public float maxOrthographicSize = 7.0f; // ������������ ������ ������
    public float minDistance = 2.0f; // ����������� ���������� �� ����
    public float maxDistance = 50.0f; // ������������ ���������� �� ����

    private Transform target; // �����, ������ ������� ����� ��������� ������
    private Vector3 offset; // ������ ���������� ����� ������� � �����
    private float distance; // ������� ���������� �� ����
    private bool targetInitialized = false; // ��������, ��� �� ������ ������ Grid

    void Start()
    {
        // �������������� ���������� ������ (�� ������, ���� target �������� �����)
        offset = transform.position;
        distance = offset.magnitude;
        InvokeRepeating(nameof(FindAndSetTarget), 0f, 0.5f); // ������������ ��������� ������� ������� Grid
    }

    void Update()
    {
        if (!targetInitialized || target == null) return;

        // ��������� ��������� ������ ������ ���� ��� �������
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // �������� ������ ������ �� ��� X � Y
            Quaternion rotation = Quaternion.Euler(-rotationY, rotationX, 0);
            offset = rotation * offset;
        }

        // ��������������� � ������� ������ ����
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (Camera.main.orthographic)
        {
            // ��� ��������������� ������ �������� Size
            Camera.main.orthographicSize = Mathf.Clamp(
                Camera.main.orthographicSize - scroll,
                minOrthographicSize,
                maxOrthographicSize
            );
        }
        else
        {
            // ��� ������������� ������ �������� ����������
            distance = Mathf.Clamp(distance - scroll, minDistance, maxDistance);
        }

        // ��������� ������� ������
        transform.position = target.position + offset.normalized * distance;
        transform.LookAt(target.position);
    }

    private void FindAndSetTarget()
    {
        if (targetInitialized) return; // ���� ���� ��� �����������, ������ �� ������

        GameObject gridObject = GameObject.FindWithTag(gridTag);
        if (gridObject != null)
        {
            // ������� ������ ��� ������
            target = new GameObject("Camera Target").transform;

            // ������������ ����� ������� Grid
            Bounds gridBounds = CalculateBounds(gridObject.transform);
            target.position = gridBounds.center;

            // �������������� ���������� ����� ������� � �������
            offset = transform.position - target.position;
            distance = offset.magnitude;

            targetInitialized = true; // ���� ������� �����������
            CancelInvoke(nameof(FindAndSetTarget)); // ������������� ��������
        }
    }

    // ����� ��� ���������� ������ ������� Grid
    private Bounds CalculateBounds(Transform grid)
    {
        Renderer[] renderers = grid.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Grid does not contain any renderers!");
            return new Bounds(grid.position, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }
}
