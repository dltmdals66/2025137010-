using UnityEngine;

/// <summary>
/// 공이 이 파워업을 먹으면 다음 한 번만 발사력을 2배로 강화시켜줍니다.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class BouncePowerUp : MonoBehaviour
{
    [Header("파워업 설정")]
    public float multiplier = 2f;    // 발사력에 곱해질 배수

    void Start()
    {
        // BoxCollider를 트리거 모드로 설정
        var col = GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ball 태그를 가진 오브젝트에만 적용
        if (!other.CompareTag("Ball"))
            return;

        // SimpleBallController의 파워업 메서드 호출
        var controller = other.GetComponent<SimpleBallController>();
        if (controller != null)
        {
            controller.ApplyPowerMultiplier(multiplier);
        }

        // 파워업 오브젝트 제거
        Destroy(gameObject);
    }
}
