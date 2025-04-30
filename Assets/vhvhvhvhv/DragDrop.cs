// 카드 드래그 앤 드롭 처리를 위한 클래스
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public bool isDragging = false;               // 드래그 중인지 판별하는 Bool 값
    public Vector3 startPosition;                 // 드래그 시작 위치
    public Transform startParent;                 // 드래그 시작 시 있던 영역 (부모)

    private GameManager gameManager;              // 게임 매니저를 참조한다.

    void Start()
    {
        startPosition = transform.position;       // 시작 위치 저장
        startParent = transform.parent;           // 시작 부모 저장

        gameManager = FindObjectOfType<GameManager>(); // 게임 매니저 찾기
    }

    void Update()
    {
        if (isDragging) // 드래그 중이고 카메라가 존재할 때
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    // 마우스 클릭 시 드래그 시작
    void OnMouseDown()
    {
        isDragging = true;

        startPosition = transform.position;
        startParent = transform.parent;

        GetComponent<SpriteRenderer>().sortingOrder = 10; // 위로 표시
    }

    // 마우스 버튼을 놓았을 때
    void OnMouseUp()
    {
        isDragging = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;

        // 드롭 위치 확인 후 처리
        if (gameManager != null && !IsOverArea(gameManager.handArea))
        {
            ReturnToOriginalPosition(); // 손패 위가 아니라면 원래 자리로
        }
        else
        {
            // 필요 시 드롭 성공 처리 추가 가능
        }
    }

    // 원래 위치로 돌아가는 함수
    void ReturnToOriginalPosition()
    {
        transform.position = startPosition;
        transform.SetParent(startParent);

        if (gameManager != null && startParent == gameManager.handArea)
        {
            gameManager.ArrangeHand();
        }
    }

    // 카드가 특정 영역 위에 있는지 확인
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