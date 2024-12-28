using UnityEngine;
using UnityEngine.UI;
using YG;

public class RecycledCubeCounter : MonoBehaviour
{
    public Text starsText; // ����� ��� ����������� ������
    public Button upgradeButton; // ������ ��� ��������
    public Image StarsADImage; // ����������� � ��������
    public Text upgradeLVL; // ����� ������ ��������

    private int recycledCounter; // ������� ��������
    private int AdID = 1; // ������� ����������
    private GridMovementChecker gridChecker; // ������ �� GridMovementChecker

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
        LoadRecycledCounter(); // �������� �������� ��� ���������
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    private void Awake()
    {
        LoadRecycledCounter();
        UpdateUI();
    }

    private void LoadRecycledCounter()
    {
        recycledCounter = PlayerPrefs.GetInt("RecycledCounter", 1);
        CheckRecycledCounter();
    }

    private void SaveRecycledCounter()
    {
        PlayerPrefs.SetInt("RecycledCounter", recycledCounter);
        PlayerPrefs.Save();
    }

    private void CheckRecycledCounter()
    {
        if (recycledCounter > 0)
        {
            upgradeButton.interactable = true; 
            StarsADImage.gameObject.SetActive(false);
            Debug.Log("StarsADImage ��������");
        }
        else
        {
            upgradeButton.interactable = false;
            StarsADImage.gameObject.SetActive(true);
        }
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // ���������� ������� � ������ ID

        AddStarsLVL();
        upgradeButton.interactable = true;
        StarsADImage.gameObject.SetActive(false);
        Debug.Log("StarsADImage �������� ��� ������ �������");
    }

    private void AddStarsLVL()
    {
        recycledCounter++;
        SaveRecycledCounter();
        UpdateUI();
        Debug.Log("� �������� ������� ���������� �������� �� �������, ������� ������� " + recycledCounter);
    }

    private void UpdateUI()
    {
        upgradeLVL.text = recycledCounter.ToString();
        Debug.Log("������� ������� �������� ���������� " + recycledCounter);
    }

    public void OnRecycledButtonClick()
    {
        gridChecker = FindObjectOfType<GridMovementChecker>();
        if (gridChecker == null)
        {
            Debug.LogError("GridMovementChecker not found in the scene!");
        }

        if (recycledCounter > 0 && gridChecker != null)
        {
            recycledCounter--;
            gridChecker.RemoveCube();
            CheckRecycledCounter();
            SaveRecycledCounter();
            UpdateUI();
        }
    }
}
