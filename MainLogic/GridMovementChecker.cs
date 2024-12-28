using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridMovementChecker : MonoBehaviour
{
    public int gridSizeX = 10;
    public int gridSizeY = 5;
    public int gridSizeZ = 10;
    public float cellSize = 1.0f;
    public GameObject counterText;
    public LayerMask obstacleLayer;
    public string playerTag = "Player";

    [SerializeField] private List<CubeMovement> cubes;
    [SerializeField] private GameObject winPanel;
    private int currentPoints;

    private bool isEventProcessing = false;

    private void OnEnable()=>        CubeMovement.OnCollected += CubeMovement_OnCollected;    

    private void OnDisable()=>        CubeMovement.OnCollected -= CubeMovement_OnCollected;    

    private void CubeMovement_OnCollected(CubeMovement cube)
    {
        if (isEventProcessing) return;

        isEventProcessing = true;

        currentPoints++;
        Debug.Log("Количество очков увеличено: " + currentPoints);
        UpdateCounterText();

        PlayerPrefs.SetInt("CurrentPoints", currentPoints);
        PlayerPrefs.Save();

        Debug.Log("Сохранено текущее состояние очков: " + currentPoints);

        if (cubes.Contains(cube))
        {
            cubes.Remove(cube);
            Debug.Log($"Cube {cube.name} removed from the list. Remaining: {cubes.Count}");

            if (cubes.Count == 0)
            {
                CheckForWinCondition();
            }
        }

        isEventProcessing = false;
    }

    public void UpdateCounterText()
    {
        int allPoints = PlayerPrefs.GetInt("Points");
        counterText.GetComponent<Text>().text = (allPoints + currentPoints).ToString();
    }

    void Start()
    {
        winPanel = FindInactiveObjectByName("WinPanel");
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        counterText = FindInactiveObjectByName("CounterText");
        PopulateCubesList();
        Debug.Log("Всего очков: " + PlayerPrefs.GetInt("Points"));
    }

    public void PopulateCubesList()=>        cubes = new List<CubeMovement>(GetComponentsInChildren<CubeMovement>());    
    
    public void RemoveCube()
    {
        if (cubes.Count > 0)
        {
            CubeMovement cube = cubes[0];
            cubes.RemoveAt(0);
            Destroy(cube.gameObject);
            Debug.Log($"Cube {cube.name} physically destroyed. Remaining cubes: {cubes.Count}");

            if (cubes.Count == 0)
            {
                CheckForWinCondition();
            }
        }
    }
    
    private void CheckForWinCondition()
    {
        Debug.Log("All cubes are removed. You win!");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.name == name && t.gameObject.scene.IsValid())
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
