using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public bool isDragging = false;   // �巡�� ������ �Ǻ�
    public Vector3 startPosition;           // �巡�� ���� ��ġ
    public Transform startParent;             // �巡�� ���� �� �θ�(����)

    private GameManager gameManager;          // GameManager ����

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

        // 1) ���� ���� ���� �˻�
        if (IsOverArea(gameManager.mergeArea))
        {
            if (gameManager.mergeCount >= gameManager.maxMergeSize)
            {
                Debug.Log("���� ������ ���� á���ϴ�!");
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
        // 2) ���� ���� �˻�
        else if (IsOverArea(gameManager.handArea))
        {
            if (wasInMergeArea)
            {
                // ���� �������� �� ī��: mergeCards �迭���� ���� �� ���з� �̵�
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
                // �̹� ���� ������ �ִ� ī��: ��ġ�� ������
                transform.SetParent(gameManager.handArea);
                gameManager.ArrangeHand();
            }
        }
        // 3) �� �� �����̸� ����ġ
        else
        {
            ReturnToOriginalPosition();
        }

        // [�߰�] ���� mergeArea�� �־��ٸ� ��ư ���� ������Ʈ
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

    // ī�� ��ġ(transform.position)�� ������ ������ Collider2D �ȿ� �ִ��� �˻�
    bool IsOverArea(Transform area)
    {
        if (area == null) return false;
        Collider2D col = area.GetComponent<Collider2D>();
        if (col == null) return false;
        return col.bounds.Contains(transform.position);
    }
}