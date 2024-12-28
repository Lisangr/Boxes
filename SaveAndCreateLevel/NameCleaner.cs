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
        Debug.Log("�������� ������� ��� ��������� ��������...");
        ProcessNestedObjects(transform);
        Debug.Log("������� ���������.");
    }

    [ContextMenu("Delete All Nested Objects")]
    public void DeleteAllNestedObjects()
    {
        Debug.Log("�������� �������� ���� ��������� ��������...");
        RemoveNestedObjects(transform);
        Debug.Log("��� ��������� ������� �������.");
    }

    private void ProcessNestedObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // ���������� ������������ ��������� �������
            ProcessNestedObjects(child);

            // ������� ����� � ������� �� �����
            string oldName = child.name;
            string newName = RemoveNumberInBrackets(oldName);

            if (oldName != newName)
            {
                child.name = newName;
                Debug.Log($"��� ��������: \"{oldName}\" -> \"{newName}\"");
            }
        }
    }

    private void RemoveNestedObjects(Transform parent)
    {
        // �������� ���� ����� � ������
        var children = new List<Transform>();
        foreach (Transform child in parent)
        {
            children.Add(child);
        }

        // ������� ���� �����
        foreach (var child in children)
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabInstance(child.gameObject))
            {
                // ��������� ����� � ��������
                PrefabUtility.UnpackPrefabInstance(child.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
#endif
            DestroyImmediate(child.gameObject);
        }
    }

    private string RemoveNumberInBrackets(string name)
    {
        // ������� ����� � �������, ��������: "1x1 3 (1)" -> "1x1 3"
        return Regex.Replace(name, @"\s*\(\d+\)$", "");
    }
}
