using UnityEngine;

public class ArrowsSpawner : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; //  амера дл€ определени€ позиции курсора

    private void Update()
    {
        // ѕровер€ем, была ли нажата лева€ кнопка мыши
        if (Input.GetMouseButtonDown(0)) // 0 означает левую кнопку мыши
        {
            SpawnArrow();
        }
    }

    // ћетод дл€ спавна стрелы на координатах курсора
    public void SpawnArrow()
    {
        // ѕолучаем стрелу из пула
        ArrowMoving arrow = ArrowsPool.Instance.GetArrow();

        // «адаем позицию дл€ стрелы Ч она должна совпадать с координатами курсора
        Vector3 spawnPosition = GetCursorWorldPosition();
        arrow.transform.position = spawnPosition;

        // ћожно задать направление стрелы, например, "вперед" по умолчанию
        arrow.transform.rotation = Quaternion.identity;
    }

    // ћетод дл€ вычислени€ координат курсора в мировом пространстве
    private Vector3 GetCursorWorldPosition()
    {
        // ѕолучаем позицию курсора в пространстве экрана
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ќпредел€ем, куда попадает луч (например, на землю или другой объект)
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; // ¬озвращаем точку пересечени€
        }

        // ≈сли луч никуда не попал, используем координаты в плоскости XZ на заданной высоте (например, 0)
        return ray.origin + ray.direction * 10f; // —павн на некотором рассто€нии от камеры
    }
}
