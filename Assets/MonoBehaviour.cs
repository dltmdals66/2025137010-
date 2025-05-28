using UnityEngine;

/// <summary>
/// ���� �� �Ŀ����� ������ ���� �� ���� �߻���� 2��� ��ȭ�����ݴϴ�.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class BouncePowerUp : MonoBehaviour
{
    [Header("�Ŀ��� ����")]
    public float multiplier = 2f;    // �߻�¿� ������ ���

    void Start()
    {
        // BoxCollider�� Ʈ���� ���� ����
        var col = GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ball �±׸� ���� ������Ʈ���� ����
        if (!other.CompareTag("Ball"))
            return;

        // SimpleBallController�� �Ŀ��� �޼��� ȣ��
        var controller = other.GetComponent<SimpleBallController>();
        if (controller != null)
        {
            controller.ApplyPowerMultiplier(multiplier);
        }

        // �Ŀ��� ������Ʈ ����
        Destroy(gameObject);
    }
}
