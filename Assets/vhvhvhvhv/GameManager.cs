using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ������ ���ҽ� & ���� ��������������������������������������������������������������������������������������������������������
    [Header("������ & �̹���")]
    public GameObject cardPrefab;    // ī�� ������
    public Sprite[] cardImages;    // ī�� ���� �����ϴ� �̹��� �迭

    [Header("����")]
    public Transform deckArea;      // �� ����
    public Transform handArea;      // ���� ����
    public Transform mergeArea;     // ���� ����

    [Header("UI")]
    public Button drawButton;     // ��ο� ��ư
    public Button mergeButton;    // ���� ��ư
    public TextMeshProUGUI deckCountText;  // ���� �� �� ǥ�� �ؽ�Ʈ

    [Header("���� ����")]
    public float cardSpacing = 2f;  // ī�� ���� (����, ���� ���� ��� ���)
    public int maxHandSize = 6;  // �ִ� ���� ũ��
    public int maxMergeSize = 3;  // �ִ� ���� ���� ī�� ��

    [Header("�ʱ� �� ���� (����)")]
    public int[] predefinedDeck = new int[]
    {
        1,1,1,1,1,1,1,1,  // 1��8
        2,2,2,2,2,2,      // 2��6
        3,3,3,3,          // 3��4
        4,4               // 4��2
    };

    // ������ ��Ÿ�� ���� ��������������������������������������������������������������������������������������������������������
    [HideInInspector] public GameObject[] deckCards;   // �� ī�� GameObject
    [HideInInspector] public int deckCount;   // ���� ���� ī�� ��

    [HideInInspector] public GameObject[] handCards;   // ���� ī�� GameObject
    [HideInInspector] public int handCount;   // ���� ī�� ��

    [HideInInspector] public GameObject[] mergeCards;  // ���� ���� ī�� GameObject
    [HideInInspector] public int mergeCount;  // ���� ���� ī�� ��

    // ������ �ʱ�ȭ ������������������������������������������������������������������������������������������������������������������
    void Start()
    {
        deckCards = new GameObject[predefinedDeck.Length];
        handCards = new GameObject[maxHandSize];
        mergeCards = new GameObject[maxMergeSize];

        InitializeDeck();
        ShuffleDeck();
        UpdateDeckCountText();

        if (drawButton != null)
            drawButton.onClick.AddListener(OnDrawButtonClicked);

        if (mergeButton != null)
        {
            mergeButton.onClick.AddListener(OnMergeButtonClicked);
            mergeButton.interactable = false;  // ó���� ��Ȱ��ȭ
        }

        UpdateMergeButtonState();
    }

    void InitializeDeck()
    {
        deckCount = predefinedDeck.Length;
        for (int i = 0; i < deckCount; i++)
        {
            int value = predefinedDeck[i];
            int idx = Mathf.Clamp(value - 1, 0, cardImages.Length - 1);

            var cardObj = Instantiate(cardPrefab, deckArea.position, Quaternion.identity, deckArea);
            cardObj.SetActive(false);

            var comp = cardObj.GetComponent<Card>();
            if (comp != null)
                comp.InitCard(value, cardImages[idx]);

            deckCards[i] = cardObj;
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deckCount - 1; i++)
        {
            int j = Random.Range(i, deckCount);
            var tmp = deckCards[i];
            deckCards[i] = deckCards[j];
            deckCards[j] = tmp;
        }
    }

    void UpdateDeckCountText()
    {
        if (deckCountText != null)
            deckCountText.text = deckCount.ToString();
    }

    // ������ ���� ó�� ��������������������������������������������������������������������������������������������������������������
    void OnDrawButtonClicked()
    {
        DrawCardToHand();
    }

    public void DrawCardToHand()
    {
        if (handCount >= maxHandSize)
        {
            Debug.Log("���а� ���� á���ϴ�!");
            return;
        }
        if (deckCount <= 0)
        {
            Debug.Log("���� �� �̻� ī�尡 �����ϴ�.");
            return;
        }

        // ������ ī�� ������
        var drawn = deckCards[0];
        for (int i = 0; i < deckCount - 1; i++)
            deckCards[i] = deckCards[i + 1];
        deckCount--;
        UpdateDeckCountText();

        // ���з� �̵�
        drawn.SetActive(true);
        drawn.transform.SetParent(handArea);
        handCards[handCount] = drawn;
        handCount++;
        ArrangeHand();
    }

    public void ArrangeHand()
    {
        if (handCount == 0) return;
        float startX = -(handCount - 1) * cardSpacing / 2f;
        for (int i = 0; i < handCount; i++)
        {
            var c = handCards[i];
            if (c == null) continue;
            c.transform.position =
                handArea.position + new Vector3(startX + i * cardSpacing, 0, -0.005f * i);
        }
    }

    // ������ ���� ���� ���� ��������������������������������������������������������������������������������������������������
    public void AddCardToMerge(GameObject card)
    {
        if (mergeCount >= maxMergeSize) return;
        mergeCards[mergeCount++] = card;
        card.transform.SetParent(mergeArea);
        UpdateMergeButtonState();
        ArrangeMerge();
    }

    public void RemoveCardFromMerge(GameObject card)
    {
        for (int i = 0; i < mergeCount; i++)
        {
            if (mergeCards[i] == card)
            {
                mergeCards[i] = mergeCards[mergeCount - 1];
                mergeCards[mergeCount - 1] = null;
                mergeCount--;
                UpdateMergeButtonState();
                ArrangeMerge();
                return;
            }
        }
    }

    void UpdateMergeButtonState()
    {
        if (mergeButton != null)
            mergeButton.interactable = (mergeCount == 2 || mergeCount == 3);
    }

    public void ArrangeMerge()
    {
        if (mergeCount == 0) return;

        float startX = -(mergeCount - 1) * cardSpacing / 2f;
        for (int i = 0; i < mergeCount; i++)
        {
            if (mergeCards[i] == null) continue;
            Vector3 newPos = mergeArea.position
                           + new Vector3(startX + i * cardSpacing, 0, -0.005f);
            mergeCards[i].transform.position = newPos;
        }
    }

    // ������ ī�� ���� ���� ��������������������������������������������������������������������������������������������������
    void MergeCards()
    {
        if (mergeCount != 2 && mergeCount != 3)
        {
            Debug.Log("������ �Ϸ��� ī�尡 2�� �Ǵ� 3�� �ʿ��մϴ�!");
            return;
        }

        int firstValue = mergeCards[0].GetComponent<Card>().cardValue;
        for (int i = 1; i < mergeCount; i++)
        {
            var c = mergeCards[i].GetComponent<Card>();
            if (c == null || c.cardValue != firstValue)
            {
                Debug.Log("���� ������ ī�常 ������ �� �ֽ��ϴ�.");
                return;
            }
        }

        int newValue = firstValue + 1;
        if (newValue > cardImages.Length)
        {
            Debug.Log("�ִ� ī�� ���� �����߽��ϴ�.");
            return;
        }

        // 1) ���� ���� ī�� �����
        for (int i = 0; i < mergeCount; i++)
            if (mergeCards[i] != null)
                mergeCards[i].SetActive(false);

        // 2) �� ī�� ���� �� �ʱ�ȭ
        var newCard = Instantiate(cardPrefab, mergeArea.position, Quaternion.identity, mergeArea);
        var nc = newCard.GetComponent<Card>();
        if (nc != null)
            nc.InitCard(newValue, cardImages[newValue - 1]);

        // 3) ���� ���� ����
        for (int i = 0; i < maxMergeSize; i++)
            mergeCards[i] = null;
        mergeCount = 0;
        UpdateMergeButtonState();

        // 4) �� ī�� ���з� �̵�
        handCards[handCount] = newCard;
        handCount++;
        newCard.transform.SetParent(handArea);
        ArrangeHand();
    }

    // ������ [�߰�] ���п��� ���� �������� ī�� �̵� ����������������������������������������������������
    /// <summary>
    /// ����(GameObject[])���� ������ ī�带 ã�� �����ϰ�,
    /// mergeCards�� �߰��� �� ���� �� ��ư ���¸� �����մϴ�.
    /// </summary>
    public void MoveCardToMerge(GameObject card)
    {
        // 1) ���� ������ ���� á���� Ȯ��
        if (mergeCount >= maxMergeSize)
        {
            Debug.Log("���� ������ ���� á���ϴ�!");
            return;
        }

        // 2) ���п��� �ش� ī�� ã�� ����
        for (int i = 0; i < handCount; i++)
        {
            if (handCards[i] == card)
            {
                // �迭 �� ĭ�� ����
                for (int j = i; j < handCount - 1; j++)
                    handCards[j] = handCards[j + 1];

                handCards[handCount - 1] = null;  // ������ ���� ����
                handCount--;                     // ���� ���� ����
                ArrangeHand();                   // ���� ����
                break;
            }
        }

        // 3) ���� ������ ī�� �߰�
        mergeCards[mergeCount] = card;
        mergeCount++;

        // 4) �θ� ���� �������� ����
        card.transform.SetParent(mergeArea);

        // 5) ���� ���� ���� �� ��ư ���� ����
        ArrangeMerge();
        UpdateMergeButtonState();
    }

    // ������ ���� ��ư Ŭ�� �� ������������������������������������������������������������������������������������������������
    void OnMergeButtonClicked()
    {
        MergeCards();
    }
}