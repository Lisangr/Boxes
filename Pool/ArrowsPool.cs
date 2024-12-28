using System.Collections.Generic;
using UnityEngine;

public class ArrowsPool : MonoBehaviour
{
    public static ArrowsPool Instance;

    // Префаб стрелы
    [SerializeField] private ArrowMoving arrowPrefab;
    [SerializeField] private int poolSize = 10;

    // Очередь для пула стрел
    private Queue<ArrowMoving> arrowPool = new Queue<ArrowMoving>();

    // Объект для хранения стрел в иерархии (Bullets)
    [SerializeField] private Transform bulletsParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            // Создаем экземпляры префаба стрелы и отключаем их
            ArrowMoving arrow = Instantiate(arrowPrefab, bulletsParent);  // Создаем стрелу как дочерний объект bulletsParent
            arrow.gameObject.SetActive(false);
            arrowPool.Enqueue(arrow);
        }
    }

    // Получаем стрелу из пула
    public ArrowMoving GetArrow()
    {
        if (arrowPool.Count > 0)
        {
            ArrowMoving arrow = arrowPool.Dequeue();
            arrow.gameObject.SetActive(true);
            return arrow;
        }
        else
        {
            // Если стрел в пуле не хватает, создаем новую
            ArrowMoving arrow = Instantiate(arrowPrefab, bulletsParent);  // Создаем стрелу как дочерний объект bulletsParent
            return arrow;
        }
    }

    // Возвращаем стрелу в пул
    public void ReturnArrow(ArrowMoving arrow)
    {
        arrow.gameObject.SetActive(false);
        arrow.transform.SetParent(bulletsParent);  // Возвращаем стрелу как дочерний объект bulletsParent
        arrowPool.Enqueue(arrow);
    }
}
