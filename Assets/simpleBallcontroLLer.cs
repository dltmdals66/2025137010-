using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleBallController : MonoBehaviour
{
    [Header("�⺻ ����")]
    public float power = 10f;             // �⺻ �� (�ּ� �߻��)
    public Sprite arrowSprite;            // ȭ��ǥ �̹���

    [Header("ī�޶� ����")]
    public Transform cameraTransform;     // ����ٴ� ī�޶� Ʈ������
    public float cameraSmoothSpeed = 5f;  // ī�޶� �ε巯�� �̵� �ӵ�

    private Rigidbody rb;                 // ���� ����
    private GameObject arrow;             // ȭ��ǥ ������Ʈ
    private bool isDragging = false;      // �巡�� ������ Ȯ��
    private Vector3 startPos;             // �巡�� ���� ������ ��ũ�� ��ǥ
    private Vector3 camOffset;            // ī�޶�� �� ���� �ʱ� ������

    // �Ŀ��� ����� ��� (���� �� ���� �߻�¿��� ����)
    private float nextPowerMultiplier = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.drag = 1f;
        // Rigidbody.useGravity�� �⺻�� ���(ApplyGravity ��� ����)

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        if (cameraTransform != null)
            camOffset = cameraTransform.position - transform.position;
    }

    void Update()
    {
        HandleInput();
        if (isDragging && arrow != null)
            UpdateArrow();
    }

    void LateUpdate()
    {
        FollowCamera();
    }

    void FollowCamera()
    {
        if (cameraTransform == null) return;
        Vector3 targetPos = transform.position + camOffset;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * cameraSmoothSpeed);
        cameraTransform.LookAt(transform);
    }

    void HandleInput()
    {
        if (IsMoving()) return;
        if (Input.GetMouseButtonDown(0)) StartDrag();
        if (Input.GetMouseButtonUp(0) && isDragging) Shoot();
    }

    void StartDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            isDragging = true;
            startPos = Input.mousePosition;
            CreateArrow();
        }
    }

    void CreateArrow()
    {
        if (arrow != null) Destroy(arrow);
        arrow = new GameObject("Arrow");
        var sr = arrow.AddComponent<SpriteRenderer>();
        sr.sprite = arrowSprite;
        sr.color = Color.green;
        sr.sortingOrder = 10;
        arrow.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
        arrow.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        arrow.transform.localScale = new Vector3(0.5f, 1f, 1f);
    }

    void UpdateArrow()
    {
        if (!isDragging || arrow == null) return;
        Vector3 mouseDelta = Input.mousePosition - startPos;
        float absDist = new Vector2(mouseDelta.x, mouseDelta.y).magnitude;
        float size = Mathf.Clamp(absDist * 0.01f, 0.5f, 2.0f);
        arrow.transform.localScale = new Vector3(size, 1f, 1f);
        var sr = arrow.GetComponent<SpriteRenderer>();
        float colorRatio = Mathf.Clamp01(absDist * 0.005f);
        sr.color = Color.Lerp(Color.green, Color.red, colorRatio);
        Vector3 dir3D = new Vector3(mouseDelta.x, 0, mouseDelta.y);
        if (dir3D.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir3D.x, dir3D.z) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(90f, angle + 180f, 0f);
        }
    }

    void Shoot()
    {
        Vector3 dir;
        float force;
        if (arrow != null)
        {
            dir = new Vector3(arrow.transform.up.x, 0f, arrow.transform.up.z).normalized;
            float arrowSize = arrow.transform.localScale.x;
            force = Mathf.Max(arrowSize * power, power);
        }
        else
        {
            Vector3 mouseDelta = Input.mousePosition - startPos;
            dir = new Vector3(mouseDelta.x, 0, mouseDelta.y).normalized;
            float baseForce = new Vector2(mouseDelta.x, mouseDelta.y).magnitude * 0.01f * power;
            force = Mathf.Max(baseForce, power);
        }
        force *= nextPowerMultiplier;
        nextPowerMultiplier = 1f;

        rb.AddForce(dir * force, ForceMode.Impulse);
        isDragging = false;
        if (arrow != null) Destroy(arrow);
        arrow = null;
    }

    /// <summary>
    /// �Ŀ������� ȣ��: ���� �� ���� �߻�¿��� ������ ��� ����
    /// </summary>
    public void ApplyPowerMultiplier(float mult)
    {
        nextPowerMultiplier = mult;
    }

    public bool IsMoving()
    {
        return rb.velocity.magnitude > 0.2f;
    }
}
