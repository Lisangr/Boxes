using UnityEngine;

public class ArrowMoving : MonoBehaviour
{
    public float lifetime = 1f;

    private void OnEnable()
    {
        // ��������� ������ ��� �������� ������ � ���
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void ReturnToPool()
    {
        // ���������� ������ � ���
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
