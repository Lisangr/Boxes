#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class NameCleaner : MonoBehaviour
{
    [ContextMenu("Clean Names")]
    public void CleanNames()
    {
        Debug.Log("Начинаем очистку имён вложенных объектов...");
        ProcessNestedObjects(transform);
        Debug.Log("Очистка завершена.");
    }

    [ContextMenu("Delete All Nested Objects")]
    public void DeleteAllNestedObjects()
    {
        Debug.Log("Начинаем удаление всех вложенных объектов...");
        RemoveNestedObjects(transform);
        Debug.Log("Все вложенные объекты удалены.");
    }

    private void ProcessNestedObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Рекурсивно обрабатываем вложенные объекты
            ProcessNestedObjects(child);

            // Убираем номер в скобках из имени
            string oldName = child.name;
            string newName = RemoveNumberInBrackets(oldName);

            if (oldName != newName)
            {
                child.name = newName;
                Debug.Log($"Имя изменено: \"{oldName}\" -> \"{newName}\"");
            }
        }
    }

    private void RemoveNestedObjects(Transform parent)
    {
        // Собираем всех детей в список
        var children = new List<Transform>();
        foreach (Transform child in parent)
        {
            children.Add(child);
        }

        // Удаляем всех детей
        foreach (var child in children)
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabInstance(child.gameObject))
            {
                // Разрываем связь с префабом
                PrefabUtility.UnpackPrefabInstance(child.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
#endif
            DestroyImmediate(child.gameObject);
        }
    }

    private string RemoveNumberInBrackets(string name)
    {
        // Убираем текст в скобках, например: "1x1 3 (1)" -> "1x1 3"
        return Regex.Replace(name, @"\s*\(\d+\)$", "");
    }
}
