using UnityEngine;

public class ArrowMoving : MonoBehaviour
{
    public float lifetime = 1f;

    private void OnEnable()
    {
        // Запускаем таймер для возврата стрелы в пул
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void ReturnToPool()
    {
        // Возвращаем стрелу в пул
        ArrowsPool.Instance.ReturnArrow(this);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }

}
