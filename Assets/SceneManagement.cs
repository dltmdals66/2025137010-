using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���Ʒ��� �����̴� ��ֹ� Ŭ����
/// ���� ������ ���� ���� ���ε��� ������ ������մϴ�.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MovingObstacle : MonoBehaviour
{
    [Header("������ ����")]
    public float speed = 2f;       // �̵� �ӵ�
    public float amplitude = 1f;   // �̵� �ݰ�

    private Vector3 startPosition;

    void Start()
    {
        // �ʱ� ��ġ ����
        startPosition = transform.position;

        // Collider�� Ʈ���� ���� ����
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    void Update()
    {
        // �ð��� ���� ���Ʒ��� ������ �̵�
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        // �±� "Ball"�� �ƴ� ��� ����
        if (!other.CompareTag("Ball"))
            return;

        // ���� ���� �ٽ� �ε��Ͽ� ���� �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
