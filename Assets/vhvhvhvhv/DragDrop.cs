using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public bool isDragging = false;   // 드래그 중인지 판별
    public Vector3 startPosition;           // 드래그 시작 위치
    public Transform startParent;             // 드래그 시작 시 부모(영역)

    private GameManager gameManager;          // GameManager 참조

    void Start()
    {
        startPosition = transform.position;
        startParent = transform.parent;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<SpriteRenderer>().sortingOrder = 10;
    }

    void OnMouseUp()
    {
        isDragging = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;

        if (gameManager == null)
        {
            ReturnToOriginalPosition();
            return;
        }

        bool wasInMergeArea = startParent == gameManager.mergeArea;

        // 1) 머지 영역 먼저 검사
        if (IsOverArea(gameManager.mergeArea))
        {
            if (gameManager.mergeCount >= gameManager.maxMergeSize)
            {
                Debug.Log("머지 영역이 가득 찼습니다!");
                ReturnToOriginalPosition();
            }
            else
            {
                if (startParent == gameManager.handArea)
                    gameManager.MoveCardToMerge(gameObject);
                else
                    gameManager.ArrangeMerge();
            }
        }
        // 2) 손패 영역 검사
        else if (IsOverArea(gameManager.handArea))
        {
            if (wasInMergeArea)
            {
                // 머지 영역에서 온 카드: mergeCards 배열에서 제거 → 손패로 이동
                for (int i = 0; i < gameManager.mergeCount; i++)
                {
                    if (gameObject == gameManager.mergeCards[i])
                    {
                        for (int j = i; j < gameManager.mergeCount - 1; j++)
                            gameManager.mergeCards[j] = gameManager.mergeCards[j + 1];
                        gameManager.mergeCards[--gameManager.mergeCount] = null;

                        transform.SetParent(gameManager.handArea);
                        gameManager.handCards[gameManager.handCount++] = gameObject;

                        gameManager.ArrangeHand();
                        gameManager.ArrangeMerge();
                        break;
                    }
                }
            }
            else
            {
                // 이미 손패 영역에 있던 카드: 위치만 재정렬
                transform.SetParent(gameManager.handArea);
                gameManager.ArrangeHand();
            }
        }
        // 3) 그 외 영역이면 원위치
        else
        {
            ReturnToOriginalPosition();
        }

        // [추가] 원래 mergeArea에 있었다면 버튼 상태 업데이트
        if (wasInMergeArea && gameManager.mergeButton != null)
        {
            bool canMerge = (gameManager.mergeCount == 2 || gameManager.mergeCount == 3);
            gameManager.mergeButton.interactable = canMerge;
        }
    }

    void ReturnToOriginalPosition()
    {
        transform.position = startPosition;
        transform.SetParent(startParent);

        if (gameManager != null)
        {
            if (startParent == gameManager.handArea)
                gameManager.ArrangeHand();
            else if (startParent == gameManager.mergeArea)
                gameManager.ArrangeMerge();
        }
    }

    // 카드 위치(transform.position)가 지정된 영역의 Collider2D 안에 있는지 검사
    bool IsOverArea(Transform area)
    {
        if (area == null) return false;
        Collider2D col = area.GetComponent<Collider2D>();
        if (col == null) return false;
        return col.bounds.Contains(transform.position);
    }
}