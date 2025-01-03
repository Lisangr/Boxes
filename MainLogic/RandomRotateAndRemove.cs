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

    [ContextMenu("��������� ��������� ��������")]
    public void StartProcessing()
    {
        Debug.Log("�������� ��������� ��������...");
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
                Debug.Log($"���������� ������ {child.name}, ��� ��� � ���� ��� ����������.");
                continue;
            }

            RandomRotate(child);

            if (!ResolveDirectionConflicts(child))
            {
                Debug.LogWarning($"������ {child.name} �� ������� ���������������, �������.");
                RemoveCubeFromScene(child.gameObject);
                continue;
            }

            if (IsCollidingWithTag(childCollider, "Player"))
            {
                Debug.Log($"���������� ����������� � Player � ������� {child.name}. �������.");
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

        Debug.Log($"������ {obj.name} �������� ��������� �������: X={randomAngleX}, Y={randomAngleY}, Z={randomAngleZ}");
    }

    private bool ResolveDirectionConflicts(Transform obj)
    {
        Debug.Log($"�������� �������� ����������� ��� {obj.name}.");

        Ray ray = new Ray(obj.position, obj.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.transform;

            if (hitTransform == obj) continue;

            if (Vector3.Distance(obj.position, hitTransform.position) > rayDistance)
            {
                Debug.Log($"������ {hitTransform.name} ������� ������ �� {obj.name}, ����������.");
                continue;
            }

            Debug.Log($"��������� ������ {hitTransform.name} �� ���� ���� �� {obj.name}.");

            if (IsOppositeDirection(obj.forward, hitTransform.forward))
            {
                Debug.LogWarning($"�������� ����������� ����� {obj.name} � {hitTransform.name}. ������� ������ {hitTransform.name}.");
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
        Debug.Log($"������� ������ {cube.name} �� �����...");
        if (gridMovementChecker != null)
        {
            CubeMovement cubeMovement = cube.GetComponent<CubeMovement>();
            if (cubeMovement != null && gridMovementChecker.cubes.Contains(cubeMovement))
            {
                gridMovementChecker.cubes.Remove(cubeMovement);
                Debug.Log($"������ {cube.name} ������ �� ������ GridMovementChecker.");
            }
        }
        DestroyImmediate(cube);
    }
}























/*using UnityEngine;

public class RandomRotateAndRemove : MonoBehaviour
{
    [ContextMenu("��������� ��������� ��������")]
    public void StartProcessing()
    {
        Debug.Log("�������� ��������� ��������...");
        ProcessNestedObjects(transform);
    }

    private void ProcessNestedObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // ���������� ������������ ��������� �������
            ProcessNestedObjects(child);

            // ���������� ������, ���� � ���� ��� Collider
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider == null)
            {
                Debug.Log($"���������� ������ {child.name}, ��� ��� � ���� ��� ����������.");
                continue;
            }

            // ��������� ��������� ��������� ��������
            RandomRotate(child);

            // �������� ������� ��� ����������
            if (!ResolveDirectionConflicts(child))
            {
                Debug.LogWarning($"������ {child.name} �� ������� ���������������, �������.");
                DestroyImmediate(child.gameObject);
                continue;
            }

            // ��������� ����������� � ������� �������
            if (IsCollidingWithTag(childCollider, "Player"))
            {
                Debug.Log($"���������� ����������� � Player � ������� {child.name}. �������.");
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void RandomRotate(Transform obj)
    {
        // ��������� ���������� �������� �� ���� ��� ����
        int randomAngleX = Random.Range(0, 4) * 90;
        int randomAngleY = Random.Range(0, 4) * 90;
        int randomAngleZ = Random.Range(0, 4) * 90;

        // ���������� ��������
        obj.Rotate(Vector3.right, randomAngleX);
        obj.Rotate(Vector3.up, randomAngleY);
        obj.Rotate(Vector3.forward, randomAngleZ);

        Debug.Log($"������ {obj.name} �������� ��������� �������: X={randomAngleX}, Y={randomAngleY}, Z={randomAngleZ}");
    }

    public float rayDistance = 50f;
    public int maxRecursions = 10;

    private bool ResolveDirectionConflicts(Transform obj)
    {
        Debug.Log($"�������� �������� ����������� ��� {obj.name}.");

        Ray ray = new Ray(obj.position, obj.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.transform;

            // ���������� ������ ����
            if (hitTransform == obj) continue;

            // ��������� ���������� ����� ���������
            if (Vector3.Distance(obj.position, hitTransform.position) > rayDistance)
            {
                Debug.Log($"������ {hitTransform.name} ������� ������ �� {obj.name}, ����������.");
                continue;
            }

            Debug.Log($"��������� ������ {hitTransform.name} �� ���� ���� �� {obj.name}.");

            // ��������� �������� �����������
            if (IsOppositeDirection(obj.forward, hitTransform.forward))
            {
                Debug.LogWarning($"�������� ����������� ����� {obj.name} � {hitTransform.name}. ������� ������ {hitTransform.name}.");
                DestroyImmediate(hitTransform.gameObject);
            }
        }

        return true; // ��� ����������
    }

    private bool IsOppositeDirection(Vector3 dir1, Vector3 dir2)
    {
        float dotProduct = Vector3.Dot(dir1, dir2);
        return dotProduct < -0.5f; // �������� �� ��������������� �����������
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