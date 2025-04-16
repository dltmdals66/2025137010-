using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    public float moveSpeed = 5.0f; // ť�� �̵� �ӵ�

    void Update()
    {
        transform.Translate(0.0f, 0.0f, -moveSpeed * Time.deltaTime); // Z�� ���̳ʽ� �������� �̵�

        if (transform.position.z < -20) // ť�갡 z�� -20 ���Ϸ� ������ Ȯ��
        {
            Destroy(gameObject); // �ڱ� �ڽ� ����
        }
    }
}
