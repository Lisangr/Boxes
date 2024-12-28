using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public string gridTag = "Grid"; // Тег для поиска объекта Grid
    public float rotationSpeed = 100.0f; // Скорость вращения камеры
    public float zoomSpeed = 10.0f; // Скорость масштабирования
    public float minOrthographicSize = 5.0f; // Минимальный размер камеры
    public float maxOrthographicSize = 7.0f; // Максимальный размер камеры
    public float minDistance = 2.0f; // Минимальное расстояние до цели
    public float maxDistance = 50.0f; // Максимальное расстояние до цели

    private Transform target; // Точка, вокруг которой будет вращаться камера
    private Vector3 offset; // Вектор расстояния между камерой и целью
    private float distance; // Текущее расстояние до цели
    private bool targetInitialized = false; // Проверка, был ли найден объект Grid

    void Start()
    {
        // Инициализируем расстояние камеры (на случай, если target появится позже)
        offset = transform.position;
        distance = offset.magnitude;
        InvokeRepeating(nameof(FindAndSetTarget), 0f, 0.5f); // Периодически проверяем наличие объекта Grid
    }

    void Update()
    {
        if (!targetInitialized || target == null) return;

        // Проверяем удержание правой кнопки мыши или касания
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Вращение вокруг центра по оси X и Y
            Quaternion rotation = Quaternion.Euler(-rotationY, rotationX, 0);
            offset = rotation * offset;
        }

        // Масштабирование с помощью колеса мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (Camera.main.orthographic)
        {
            // Для ортографической камеры изменяем Size
            Camera.main.orthographicSize = Mathf.Clamp(
                Camera.main.orthographicSize - scroll,
                minOrthographicSize,
                maxOrthographicSize
            );
        }
        else
        {
            // Для перспективной камеры изменяем расстояние
            distance = Mathf.Clamp(distance - scroll, minDistance, maxDistance);
        }

        // Обновляем позицию камеры
        transform.position = target.position + offset.normalized * distance;
        transform.LookAt(target.position);
    }

    private void FindAndSetTarget()
    {
        if (targetInitialized) return; // Если цель уже установлена, ничего не делаем

        GameObject gridObject = GameObject.FindWithTag(gridTag);
        if (gridObject != null)
        {
            // Создаем объект для центра
            target = new GameObject("Camera Target").transform;

            // Рассчитываем центр объекта Grid
            Bounds gridBounds = CalculateBounds(gridObject.transform);
            target.position = gridBounds.center;

            // Инициализируем расстояние между камерой и центром
            offset = transform.position - target.position;
            distance = offset.magnitude;

            targetInitialized = true; // Цель успешно установлена
            CancelInvoke(nameof(FindAndSetTarget)); // Останавливаем проверки
        }
    }

    // Метод для вычисления границ объекта Grid
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
