using UnityEngine;

public class Destroer : MonoBehaviour
{
    private void OnEnable()
    {
        CanvasButtons.OnDestroed += CanvasButtons_OnDestroed;
    }

    private void CanvasButtons_OnDestroed()
    {
        Destroy(gameObject);
        DestroyImmediate(gameObject);
    }

    private void OnDisable()
    {
        CanvasButtons.OnDestroed -= CanvasButtons_OnDestroed;
    }
}
