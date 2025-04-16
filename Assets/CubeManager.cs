using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public CubeGenerator[] generatedCubes = new CubeGenerator[5]; // 클래스 배열

    // 타이머 관련 변수
    public float timer = 0f;        // 시간 타이머 설정
    public float interval = 3f;     // 3초마다 실행

    void Start()
    {
        // 시작 시 필요한 초기화 코드 작성 가능
    }

    void Update()
    {
        timer += Time.deltaTime; // 프레임당 시간 누적

        if (timer >= interval)   // 일정 시간(3초)마다 실행
        {
            RandomizeCubeActivation(); // 함수 호출
            timer = 0f;                // 타이머 초기화
        }
    }

    // 각 큐브를 랜덤하게 활성화
    public void RandomizeCubeActivation()
    {
        for (int i = 0; i < generatedCubes.Length; i++) // 모든 큐브 순회
        {
            int randomNum = Random.Range(0, 2); // 0 또는 1 랜덤값

            if (randomNum == 1)
            {
                generatedCubes[i].GenCube(); // 1일 경우 큐브 생성
            }
        }
    }
}