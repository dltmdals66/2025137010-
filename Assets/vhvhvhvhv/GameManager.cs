using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ─── 리소스 & 설정 ────────────────────────────────────────────────────
    [Header("프리팹 & 이미지")]
    public GameObject cardPrefab;    // 카드 프리팹
    public Sprite[] cardImages;    // 카드 값에 대응하는 이미지 배열

    [Header("영역")]
    public Transform deckArea;      // 덱 영역
    public Transform handArea;      // 손패 영역
    public Transform mergeArea;     // 머지 영역

    [Header("UI")]
    public Button drawButton;     // 드로우 버튼
    public Button mergeButton;    // 머지 버튼
    public TextMeshProUGUI deckCountText;  // 남은 덱 수 표시 텍스트

    [Header("게임 설정")]
    public float cardSpacing = 2f;  // 카드 간격 (손패, 머지 영역 모두 사용)
    public int maxHandSize = 6;  // 최대 손패 크기
    public int maxMergeSize = 3;  // 최대 머지 가능 카드 수

    [Header("초기 덱 구성 (값만)")]
    public int[] predefinedDeck = new int[]
    {
        1,1,1,1,1,1,1,1,  // 1×8
        2,2,2,2,2,2,      // 2×6
        3,3,3,3,          // 3×4
        4,4               // 4×2
    };

    // ─── 런타임 변수 ────────────────────────────────────────────────────
    [HideInInspector] public GameObject[] deckCards;   // 덱 카드 GameObject
    [HideInInspector] public int deckCount;   // 덱에 남은 카드 수

    [HideInInspector] public GameObject[] handCards;   // 손패 카드 GameObject
    [HideInInspector] public int handCount;   // 손패 카드 수

    [HideInInspector] public GameObject[] mergeCards;  // 머지 영역 카드 GameObject
    [HideInInspector] public int mergeCount;  // 머지 영역 카드 수

    // ─── 초기화 ─────────────────────────────────────────────────────────
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
            mergeButton.interactable = false;  // 처음엔 비활성화
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

    // ─── 손패 처리 ───────────────────────────────────────────────────────
    void OnDrawButtonClicked()
    {
        DrawCardToHand();
    }

    public void DrawCardToHand()
    {
        if (handCount >= maxHandSize)
        {
            Debug.Log("손패가 가득 찼습니다!");
            return;
        }
        if (deckCount <= 0)
        {
            Debug.Log("덱에 더 이상 카드가 없습니다.");
            return;
        }

        // 덱에서 카드 꺼내기
        var drawn = deckCards[0];
        for (int i = 0; i < deckCount - 1; i++)
            deckCards[i] = deckCards[i + 1];
        deckCount--;
        UpdateDeckCountText();

        // 손패로 이동
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

    // ─── 머지 영역 관리 ─────────────────────────────────────────────────
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

    // ─── 카드 머지 로직 ─────────────────────────────────────────────────
    void MergeCards()
    {
        if (mergeCount != 2 && mergeCount != 3)
        {
            Debug.Log("머지를 하려면 카드가 2개 또는 3개 필요합니다!");
            return;
        }

        int firstValue = mergeCards[0].GetComponent<Card>().cardValue;
        for (int i = 1; i < mergeCount; i++)
        {
            var c = mergeCards[i].GetComponent<Card>();
            if (c == null || c.cardValue != firstValue)
            {
                Debug.Log("같은 숫자의 카드만 머지할 수 있습니다.");
                return;
            }
        }

        int newValue = firstValue + 1;
        if (newValue > cardImages.Length)
        {
            Debug.Log("최대 카드 값에 도달했습니다.");
            return;
        }

        // 1) 기존 머지 카드 숨기기
        for (int i = 0; i < mergeCount; i++)
            if (mergeCards[i] != null)
                mergeCards[i].SetActive(false);

        // 2) 새 카드 생성 및 초기화
        var newCard = Instantiate(cardPrefab, mergeArea.position, Quaternion.identity, mergeArea);
        var nc = newCard.GetComponent<Card>();
        if (nc != null)
            nc.InitCard(newValue, cardImages[newValue - 1]);

        // 3) 머지 영역 비우기
        for (int i = 0; i < maxMergeSize; i++)
            mergeCards[i] = null;
        mergeCount = 0;
        UpdateMergeButtonState();

        // 4) 새 카드 손패로 이동
        handCards[handCount] = newCard;
        handCount++;
        newCard.transform.SetParent(handArea);
        ArrangeHand();
    }

    // ─── [추가] 손패에서 머지 영역으로 카드 이동 ──────────────────────────
    /// <summary>
    /// 손패(GameObject[])에서 지정된 카드를 찾아 제거하고,
    /// mergeCards에 추가한 후 영역 및 버튼 상태를 갱신합니다.
    /// </summary>
    public void MoveCardToMerge(GameObject card)
    {
        // 1) 머지 영역이 가득 찼는지 확인
        if (mergeCount >= maxMergeSize)
        {
            Debug.Log("머지 영역이 가득 찼습니다!");
            return;
        }

        // 2) 손패에서 해당 카드 찾아 제거
        for (int i = 0; i < handCount; i++)
        {
            if (handCards[i] == card)
            {
                // 배열 한 칸씩 당기기
                for (int j = i; j < handCount - 1; j++)
                    handCards[j] = handCards[j + 1];

                handCards[handCount - 1] = null;  // 마지막 슬롯 비우기
                handCount--;                     // 손패 개수 감소
                ArrangeHand();                   // 손패 정렬
                break;
            }
        }

        // 3) 머지 영역에 카드 추가
        mergeCards[mergeCount] = card;
        mergeCount++;

        // 4) 부모를 머지 영역으로 설정
        card.transform.SetParent(mergeArea);

        // 5) 머지 영역 정렬 및 버튼 상태 갱신
        ArrangeMerge();
        UpdateMergeButtonState();
    }

    // ─── 머지 버튼 클릭 시 ────────────────────────────────────────────────
    void OnMergeButtonClicked()
    {
        MergeCards();
    }
}