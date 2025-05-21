using System.Collections;
using UnityEngine;

public class FruitGame : MonoBehaviour
{
    [Header("프리팹 & 크기 설정")]
    public GameObject[] fruitPrefabs;
    public float[] fruitSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };

    [Header("움직임 X 한계")]
    public float leftLimit = -6.0f;
    public float rightLimit = 6.4f;
    public float leftMargin = 1.0f;
    public float rightMargin = 1.0f;

    [Header("스폰 & 물리")]
    public float fruitStartHeight = 6.0f;
    public float gravityScale = 1f;
    public float respawnDelay = 1.4f;

    [Header("바닥(게임오버 콜리전)")]
    public Transform floorLine;   // Collider2D 있는 오브젝트 할당

    [Header("점수")]
    public int score = 0;

    Camera _cam;
    GameObject _currentFruit;
    int _currentType;
    bool _isDropped;
    bool _isGameOver;

    void Start()
    {
        _cam = Camera.main;
        SpawnNewFruit();
    }

    void Update()
    {
        if (_isGameOver) return;

        // 드래그 중(아직 Drop 전)일 때만 X축 이동
        if (_currentFruit != null && !_isDropped)
        {
            Vector3 sp = Input.mousePosition;
            sp.z = Mathf.Abs(_cam.transform.position.z);
            Vector3 wp = _cam.ScreenToWorldPoint(sp);

            float half = fruitSizes[_currentType] * 0.5f;
            float minX = leftLimit + half + leftMargin;
            float maxX = rightLimit - half - rightMargin;
            float clampedX = Mathf.Clamp(wp.x, minX, maxX);

            _currentFruit.transform.position = new Vector3(clampedX, fruitStartHeight, 0f);

            if (Input.GetMouseButtonDown(0))
                DropFruit();
        }
    }

    void GameOver()
    {
        _isGameOver = true;
        Debug.Log($"Game Over! Final Score: {score}");
        // TODO: UI 표시나 게임 멈춤 로직 추가
    }

    public void SpawnNewFruit()
    {
        if (_isGameOver) return;

        int type = Random.Range(0, fruitPrefabs.Length);

        Vector3 sp = Input.mousePosition;
        sp.z = Mathf.Abs(_cam.transform.position.z);
        Vector3 wp = _cam.ScreenToWorldPoint(sp);

        float half = fruitSizes[type] * 0.5f;
        float minX = leftLimit + half + leftMargin;
        float maxX = rightLimit - half - rightMargin;
        float x = Mathf.Clamp(wp.x, minX, maxX);

        Vector3 spawnPos = new Vector3(x, fruitStartHeight, 0f);
        CreateFruit(type, spawnPos, true);
    }

    public void SpawnMergedFruit(int newType, Vector3 pos)
    {
        if (_isGameOver) return;
        if (newType < fruitPrefabs.Length)
        {
            score += newType * 10;
            CreateFruit(newType, pos, false);
        }
    }

    void CreateFruit(int type, Vector3 pos, bool draggable)
    {
        var go = Instantiate(fruitPrefabs[type], pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * fruitSizes[type];

        var rb = go.GetComponent<Rigidbody2D>() ?? go.AddComponent<Rigidbody2D>();
        if (draggable)
        {
            rb.gravityScale = 0f;
            _currentFruit = go;
            _currentType = type;
            _isDropped = false;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        // 바닥 충돌 감지기 추가
        var notifier = go.AddComponent<FloorCollisionNotifier>();
        notifier.gameManager = this;
        notifier.floorLine = floorLine;
    }

    void DropFruit()
    {
        if (_currentFruit == null || _isDropped || _isGameOver) return;

        var rb = _currentFruit.GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        _isDropped = true;

        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnNewFruit();
    }

    // 과일에 붙여주는 콜리전 감지기
    class FloorCollisionNotifier : MonoBehaviour
    {
        public FruitGame gameManager;
        public Transform floorLine;

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform == floorLine && gameManager._isDropped)
                gameManager.GameOver();
        }
    }
}
