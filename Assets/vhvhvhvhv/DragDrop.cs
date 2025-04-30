// ī�� �巡�� �� ��� ó���� ���� Ŭ����
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public bool isDragging = false;               // �巡�� ������ �Ǻ��ϴ� Bool ��
    public Vector3 startPosition;                 // �巡�� ���� ��ġ
    public Transform startParent;                 // �巡�� ���� �� �ִ� ���� (�θ�)

    private GameManager gameManager;              // ���� �Ŵ����� �����Ѵ�.

    void Start()
    {
        startPosition = transform.position;       // ���� ��ġ ����
        startParent = transform.parent;           // ���� �θ� ����

        gameManager = FindObjectOfType<GameManager>(); // ���� �Ŵ��� ã��
    }

    void Update()
    {
        if (isDragging) // �巡�� ���̰� ī�޶� ������ ��
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    // ���콺 Ŭ�� �� �巡�� ����
    void OnMouseDown()
    {
        isDragging = true;

        startPosition = transform.position;
        startParent = transform.parent;

        GetComponent<SpriteRenderer>().sortingOrder = 10; // ���� ǥ��
    }

    // ���콺 ��ư�� ������ ��
    void OnMouseUp()
    {
        isDragging = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;

        // ��� ��ġ Ȯ�� �� ó��
        if (gameManager != null && !IsOverArea(gameManager.handArea))
        {
            ReturnToOriginalPosition(); // ���� ���� �ƴ϶�� ���� �ڸ���
        }
        else
        {
            // �ʿ� �� ��� ���� ó�� �߰� ����
        }
    }

    // ���� ��ġ�� ���ư��� �Լ�
    void ReturnToOriginalPosition()
    {
        transform.position = startPosition;
        transform.SetParent(startParent);

        if (gameManager != null && startParent == gameManager.handArea)
        {
            gameManager.ArrangeHand();
        }
    }

    // ī�尡 Ư�� ���� ���� �ִ��� Ȯ��
    bool IsOverArea(Transform area)
    {
        if (area == null)
            return false;

        Collider2D areaCollider = area.GetComponent<Collider2D>();
        if (areaCollider == null)
            return false;

        return areaCollider.bounds.Contains(transform.position);
    }
}