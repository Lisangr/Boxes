using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rayDistance = 5f;
    public LayerMask obstacleLayer;
    public float destructionDelay = 3f;
    private bool isMoving = false;
    private Transform target;
    [SerializeField] private float correctionOffset = 0.12f;
    [SerializeField] private float collisionThreshold = 0.02f;
    private Vector3 startPosition;
    public float sphereRadius = 0.3f;
    public int gridSizeX = 10;
    public int gridSizeY = 5;
    public int gridSizeZ = 10;
    public ParticleSystem movementParticleSystem; // Система частиц для начала движения

    public delegate void DeathAction(CubeMovement cube);
    public static event DeathAction OnCollected;

    private bool hasStartedDestruction = false; // Флаг для предотвращения многократного уничтожения

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (IsObstacleInFront())
            {
                if (target != null)
                {
                    CheckAndHandleCollision();
                }
            }
            else
            {
                MoveForward();
            }
        }

        // Проверяем, вышел ли объект за пределы сетки
        if (IsOutOfGrid() && !hasStartedDestruction)
        {
            StartDestruction();
        }
    }

    private void OnMouseDown()
    {
        if (IsObstacleInFront())
        {
            Debug.Log("Obstacle detected in front.");
        }

        isMoving = true;

        // Запускаем систему частиц при начале движения
        if (movementParticleSystem != null)
        {
            ParticleSystem spawnedParticleSystem = Instantiate(movementParticleSystem, transform.position, Quaternion.identity);
            spawnedParticleSystem.Play();
            Destroy(spawnedParticleSystem.gameObject, spawnedParticleSystem.main.duration + spawnedParticleSystem.main.startLifetime.constantMax);
        }
    }

    private bool IsObstacleInFront()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, sphereRadius, out hit, rayDistance, obstacleLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                target = hit.collider.transform;
                return true;
            }
        }

        target = null;
        return false;
    }

    private void CheckAndHandleCollision()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= collisionThreshold)
        {
            transform.position = startPosition;
            isMoving = false;
            return;
        }

        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider != null)
        {
            float targetRadius = targetCollider.bounds.extents.magnitude;

            if (distanceToTarget <= targetRadius + correctionOffset)
            {
                isMoving = false;

                Vector3 directionAway = (transform.position - target.position).normalized;
                transform.position = target.position + directionAway * (targetRadius + correctionOffset);

                return;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private bool IsOutOfGrid()
    {
        Vector3 position = transform.position;
        return position.x < 0 || position.x >= gridSizeX ||
               position.y < 0 || position.y >= gridSizeY ||
               position.z < 0 || position.z >= gridSizeZ;
    }

    private void StartDestruction()
    {
        if (hasStartedDestruction) return; // Защита от многократного вызова
        hasStartedDestruction = true;       

        // Уничтожаем текущий объект
        Destroy(gameObject, destructionDelay);

        // Вызываем событие смерти
        OnCollected?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(target.position, 0.1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * rayDistance, sphereRadius);
    }
}