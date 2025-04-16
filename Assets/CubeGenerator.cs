using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public GameObject cubePrefab;     // 생성할 큐브 프리팹
    public int totalCubes = 10;       // 총 생성할 큐브 개수
    public float cubeSpacing = 1.0f;  // 큐브 간격

    void Start()
    {
        GenCube(); // 함수를 호출한다.
    }

    public void GenCube()
    {
        Vector3 myPosition = transform.position; // 스크립트가 붙은 오브젝트의 위치 (x,y,z)

        // 첫 번째 큐브 생성 (내 위치에)
        GameObject firstCube = Instantiate(cubePrefab, myPosition, Quaternion.identity);

        for (int i = 1; i < totalCubes; i++) // 나머지 큐브를 생성
        {
            // 내 위치에서 z축으로 일정 간격 떨어진 위치에 생성
            Vector3 position = new Vector3(myPosition.x, myPosition.y, myPosition.z + (i * cubeSpacing));
            Instantiate(cubePrefab, position, Quaternion.identity); // 큐브 생성
        }
    }
}