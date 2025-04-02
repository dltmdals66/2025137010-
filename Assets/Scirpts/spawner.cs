using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject coinPrefabs;
    public GameObject MissilePefabs;


    [Header("스폰 타이밍 설정")]
    public float minSawnnlnterval = 0.5f;
    public float maxSpawnlnterval = 0.2f;

    [Header("동전 스폰 확률 설정")]
    [Range(0, 100)]
    public int coinSpawnChance = 50;
    public float timer = 0.0f;
    public float nextSpawnTime;


    // Start is called before the first frame update
    void Start()
    {
        SetNextSpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > nextSpawnTime)
        {
            SpawnObiect();
            timer = 0.0f;
            SetNextSpawnTime();
        }

     
    }

    void SpawnObiect()
    {
        Transform spawnTranaform = transform;
        Instantiate(coinPrefabs, spawnTranaform.position, spawnTranaform.rotation);

        int randomValue = Random.Range(0, 100);
        if (randomValue < coinSpawnChance)
        {
            Instantiate(coinPrefabs, spawnTranaform.position, spawnTranaform.rotation);
        }
        else
        {
            Instantiate(MissilePefabs, spawnTranaform.position, spawnTranaform.rotation);
        }
    }



    void SetNextSpawnTime()
    {

        nextSpawnTime = Random.Range(minSawnnlnterval, maxSpawnlnterval);

    }



}
