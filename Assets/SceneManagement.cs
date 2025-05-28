using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 위아래로 움직이는 장애물 클래스
/// 공이 닿으면 현재 씬을 리로드해 게임을 재시작합니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MovingObstacle : MonoBehaviour
{
    [Header("움직임 설정")]
    public float speed = 2f;       // 이동 속도
    public float amplitude = 1f;   // 이동 반경

    private Vector3 startPosition;

    void Start()
    {
        // 초기 위치 저장
        startPosition = transform.position;

        // Collider를 트리거 모드로 설정
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    void Update()
    {
        // 시간에 따라 위아래로 사인파 이동
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        // 태그 "Ball"가 아닌 경우 무시
        if (!other.CompareTag("Ball"))
            return;

        // 현재 씬을 다시 로드하여 게임 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
