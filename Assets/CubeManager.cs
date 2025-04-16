using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public CubeGenerator[] generatedCubes = new CubeGenerator[5]; // Ŭ���� �迭

    // Ÿ�̸� ���� ����
    public float timer = 0f;        // �ð� Ÿ�̸� ����
    public float interval = 3f;     // 3�ʸ��� ����

    void Start()
    {
        // ���� �� �ʿ��� �ʱ�ȭ �ڵ� �ۼ� ����
    }

    void Update()
    {
        timer += Time.deltaTime; // �����Ӵ� �ð� ����

        if (timer >= interval)   // ���� �ð�(3��)���� ����
        {
            RandomizeCubeActivation(); // �Լ� ȣ��
            timer = 0f;                // Ÿ�̸� �ʱ�ȭ
        }
    }

    // �� ť�긦 �����ϰ� Ȱ��ȭ
    public void RandomizeCubeActivation()
    {
        for (int i = 0; i < generatedCubes.Length; i++) // ��� ť�� ��ȸ
        {
            int randomNum = Random.Range(0, 2); // 0 �Ǵ� 1 ������

            if (randomNum == 1)
            {
                generatedCubes[i].GenCube(); // 1�� ��� ť�� ����
            }
        }
    }
}