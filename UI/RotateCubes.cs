using UnityEngine;
using UnityEngine.UI;
using YG;

public class RotateCubes : MonoBehaviour
{
    public Text starsText; // ����� ��� ����������� ������
    public Button upgradeButton; // ������ ��� ��������
    public Image StarsADImage; // ����������� � ��������
    public Text upgradeLVL; // ����� ������ ��������

    private int rotatorCounter; // ������� ��������
    private int AdID = 3; // �������� � ��������
    private GridMovementChecker gridChecker; // ������ �� GridMovementChecker
    [SerializeField]private RandomRotateAndRemove rotateAndRemoveScript; // ������ �� ��������� �������� � ��������
    private void OnEnable()
    {
        rotateAndRemoveScript = FindAnyObjectByType<RandomRotateAndRemove>();
        YandexGame.RewardVideoEvent += Rewarded;
        ResetRotatorCounter(); // ��������� �������� ��� ���������
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    private void Awake()
    {
        ResetRotatorCounter();
    }

    private void ResetRotatorCounter()
    {
        rotatorCounter = 1; // ������� ������ ����� 1 ��� ������
        CheckRecycledCounter();
        UpdateUI(); // ��������� ��������� ����� ����� ��������� ��������
    }

    private void CheckRecycledCounter()
    {
        if (rotatorCounter > 0)
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
        rotatorCounter++;
        UpdateUI();
        Debug.Log("� �������� ������� ���������� �������� �� �������, ������� ������� " + rotatorCounter);
    }

    private void UpdateUI()
    {
        if (upgradeLVL != null)
        {
            upgradeLVL.text = rotatorCounter.ToString();
            Debug.Log("������� ������� �������� ���������� " + rotatorCounter);
        }
        else
        {
            Debug.LogError("UI Text component (upgradeLVL) is not assigned!");
        }
    }
    public void OnRotatorButtonClick()
    {
        // ���������, ������ �� �������
        if (rotateAndRemoveScript == null)
        {
            rotateAndRemoveScript = FindObjectOfType<RandomRotateAndRemove>();
            if (rotateAndRemoveScript == null)
            {
                Debug.LogError("RandomRotateAndRemove �� ������ � �����!");
                return;
            }
        }

        // ��������� ������� GridMovementChecker
        gridChecker = FindObjectOfType<GridMovementChecker>();
        if (gridChecker == null)
        {
            Debug.LogError("GridMovementChecker �� ������ � �����!");
            return;
        }

        if (rotatorCounter > 0 && gridChecker != null)
        {
            rotatorCounter--;
            Debug.Log("��� 2: ��������� �������� (�������� � ��������)...");

            // ������ ��������� ��������
            rotateAndRemoveScript.StartProcessing();

            CheckRecycledCounter();
            UpdateUI();
        }
    }
}
