using UnityEngine;

public class RandomRotateAndRemove : MonoBehaviour
{
    public float rayDistance = 50f;
    public int maxRecursions = 10;

    private GridMovementChecker gridMovementChecker;

    private void Awake()
    {
        gridMovementChecker = FindObjectOfType<GridMovementChecker>();
        if (gridMovementChecker == null)
        {
            Debug.LogError("GridMovementChecker not found in the scene!");
        }
    }

    [ContextMenu("Запустить обработку объектов")]
    public void StartProcessing()
    {
        Debug.Log("Начинаем обработку объектов...");
        ProcessNestedObjects(transform);
    }

    private void ProcessNestedObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ProcessNestedObjects(child);

            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider == null)
            {
                Debug.Log($"Пропускаем объект {child.name}, так как у него нет коллайдера.");
                continue;
            }

            RandomRotate(child);

            if (!ResolveDirectionConflicts(child))
            {
                Debug.LogWarning($"Объект {child.name} не удалось скорректировать, удаляем.");
                RemoveCubeFromScene(child.gameObject);
                continue;
            }

            if (IsCollidingWithTag(childCollider, "Player"))
            {
                Debug.Log($"Обнаружено пересечение с Player у объекта {child.name}. Удаляем.");
                RemoveCubeFromScene(child.gameObject);
            }
        }
    }

    private void RandomRotate(Transform obj)
    {
        int randomAngleX = Random.Range(0, 4) * 90;
        int randomAngleY = Random.Range(0, 4) * 90;
        int randomAngleZ = Random.Range(0, 4) * 90;

        obj.Rotate(Vector3.right, randomAngleX);
        obj.Rotate(Vector3.up, randomAngleY);
        obj.Rotate(Vector3.forward, randomAngleZ);

        Debug.Log($"Объект {obj.name} повернут случайным образом: X={randomAngleX}, Y={randomAngleY}, Z={randomAngleZ}");
    }

    private bool ResolveDirectionConflicts(Transform obj)
    {
        Debug.Log($"Начинаем проверку направлений для {obj.name}.");

        Ray ray = new Ray(obj.position, obj.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.transform;

            if (hitTransform == obj) continue;

            if (Vector3.Distance(obj.position, hitTransform.position) > rayDistance)
            {
                Debug.Log($"Объект {hitTransform.name} слишком далеко от {obj.name}, пропускаем.");
                continue;
            }

            Debug.Log($"Обнаружен объект {hitTransform.name} на пути луча от {obj.name}.");

            if (IsOppositeDirection(obj.forward, hitTransform.forward))
            {
                Debug.LogWarning($"Конфликт направлений между {obj.name} и {hitTransform.name}. Удаляем объект {hitTransform.name}.");
                RemoveCubeFromScene(hitTransform.gameObject);
            }
        }

        return true;
    }

    private bool IsOppositeDirection(Vector3 dir1, Vector3 dir2)
    {
        float dotProduct = Vector3.Dot(dir1, dir2);
        return dotProduct < -0.5f;
    }

    private bool IsCollidingWithTag(Collider collider, string tag)
    {
        Collider[] overlaps = Physics.OverlapBox(
            collider.bounds.center,
            collider.bounds.extents,
            collider.transform.rotation
        );

        foreach (Collider overlap in overlaps)
        {
            if (overlap.CompareTag(tag) && overlap.gameObject != collider.gameObject)
            {
                return true;
            }
        }

        return false;
    }
    private void RemoveCubeFromScene(GameObject cube)
    {
        Debug.Log($"Удаляем объект {cube.name} из сцены...");
        if (gridMovementChecker != null)
        {
            CubeMovement cubeMovement = cube.GetComponent<CubeMovement>();
            if (cubeMovement != null && gridMovementChecker.cubes.Contains(cubeMovement))
            {
                gridMovementChecker.cubes.Remove(cubeMovement);
                Debug.Log($"Объект {cube.name} удален из списка GridMovementChecker.");
            }
        }
        DestroyImmediate(cube);
    }
}























/*using UnityEngine;

public class RandomRotateAndRemove : MonoBehaviour
{
    [ContextMenu("Запустить обработку объектов")]
    public void StartProcessing()
    {
        Debug.Log("Начинаем обработку объектов...");
        ProcessNestedObjects(transform);
    }

    private void ProcessNestedObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Рекурсивно обрабатываем вложенные объекты
            ProcessNestedObjects(child);

            // Пропускаем объект, если у него нет Collider
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider == null)
            {
                Debug.Log($"Пропускаем объект {child.name}, так как у него нет коллайдера.");
                continue;
            }

            // Выполняем начальное случайное вращение
            RandomRotate(child);

            // Удаление объекта при конфликтах
            if (!ResolveDirectionConflicts(child))
            {
                Debug.LogWarning($"Объект {child.name} не удалось скорректировать, удаляем.");
                DestroyImmediate(child.gameObject);
                continue;
            }

            // Проверяем пересечения и удаляем объекты
            if (IsCollidingWithTag(childCollider, "Player"))
            {
                Debug.Log($"Обнаружено пересечение с Player у объекта {child.name}. Удаляем.");
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void RandomRotate(Transform obj)
    {
        // Генерация случайного вращения по всем трём осям
        int randomAngleX = Random.Range(0, 4) * 90;
        int randomAngleY = Random.Range(0, 4) * 90;
        int randomAngleZ = Random.Range(0, 4) * 90;

        // Применение вращения
        obj.Rotate(Vector3.right, randomAngleX);
        obj.Rotate(Vector3.up, randomAngleY);
        obj.Rotate(Vector3.forward, randomAngleZ);

        Debug.Log($"Объект {obj.name} повернут случайным образом: X={randomAngleX}, Y={randomAngleY}, Z={randomAngleZ}");
    }

    public float rayDistance = 50f;
    public int maxRecursions = 10;

    private bool ResolveDirectionConflicts(Transform obj)
    {
        Debug.Log($"Начинаем проверку направлений для {obj.name}.");

        Ray ray = new Ray(obj.position, obj.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.transform;

            // Игнорируем самого себя
            if (hitTransform == obj) continue;

            // Проверяем расстояние между объектами
            if (Vector3.Distance(obj.position, hitTransform.position) > rayDistance)
            {
                Debug.Log($"Объект {hitTransform.name} слишком далеко от {obj.name}, пропускаем.");
                continue;
            }

            Debug.Log($"Обнаружен объект {hitTransform.name} на пути луча от {obj.name}.");

            // Проверяем конфликт направления
            if (IsOppositeDirection(obj.forward, hitTransform.forward))
            {
                Debug.LogWarning($"Конфликт направлений между {obj.name} и {hitTransform.name}. Удаляем объект {hitTransform.name}.");
                DestroyImmediate(hitTransform.gameObject);
            }
        }

        return true; // Нет конфликтов
    }

    private bool IsOppositeDirection(Vector3 dir1, Vector3 dir2)
    {
        float dotProduct = Vector3.Dot(dir1, dir2);
        return dotProduct < -0.5f; // Проверка на противоположные направления
    }

    private bool IsCollidingWithTag(Collider collider, string tag)
    {
        Collider[] overlaps = Physics.OverlapBox(
            collider.bounds.center,
            collider.bounds.extents,
            collider.transform.rotation
        );

        foreach (Collider overlap in overlaps)
        {
            if (overlap.CompareTag(tag) && overlap.gameObject != collider.gameObject)
            {
                return true;
            }
        }

        return false;
    }
}*/