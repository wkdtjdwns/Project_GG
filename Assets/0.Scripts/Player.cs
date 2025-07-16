using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    /* �ؾ��� ��
     * 1. Player ��ũ��Ʈ (��� �޾Ƽ� �� �� �ֵ��� �ϱ� - Move, Move, Interact) - Move: ī�޶�� �����ؼ� ��ũó�� ī�޶� ���� ������ �������� wasd �����ϵ���
     *    1-1. Interact: ä��, �����ϱ�, ����[ü��, �̼�, ������, Ư�� ����(��ȭ ȿ�� ����, Ư���� ���� ä�� ��)
     *
     * 2. ��� �޾Ƽ� ĳ���� ����� (�⺻, ��� �ߵ�, ��Ƽ�� �ɷ���) - ���� 1����
     */

    [Header("Player Info")]
    public float moveSpeed = 5f;        // �̵� �ӵ�
    public float hp = 100f;             // ü��
    public float turnSpeed = 65f;       // ȸ�� �ӵ�
    public float jumpPower = 5f;        // ������

    [Range(0.01f, 1f)] public float minGroundDistance = 0.7f; // �������� ������ Y�� �ּҰ� (���� ���� ����)
    private LayerMask groundLayer;
    private int groundCnt = 0;

    [Header("Special Player Info")]
    public float minePower = 10f;       // ä�� �ӵ�(��)
    public int mineLevel = 1;           // ä�� ����
    public float dmg = 5f;              // ������

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] private GameObject firstPersonCamera; // �÷��̾��� �ڽ� ������Ʈ���߸� ��!

    [Header("Other")]
    private Transform mainCameraTransform;       // Main Camera
    public float mouseSensitivity = 3.5f;       // ���콺 ����
    public float cameraVerticalLimit = 80f;     // ī�޶� ���� �þ� ����

    private bool isFirstPerson = false;

    private Rigidbody rigid;
    private Vector3 movementDir;
    private bool isGrounded;

    private float cameraRotationX = 0f; // 1��Ī ī�޶� Y�� ȸ�� �� (���� ���� ����)

    public enum AbilityType             // ��ų ���� (�нú�, ��Ƽ��)
    {
        Passive,
        Active
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        if (rigid == null)
        {
            Debug.LogError("Rigidbody Component is null!");
            enabled = false;
            return;
        }

        groundLayer = LayerMask.GetMask("GridCell");

        if (thirdPersonCamera != null) thirdPersonCamera.gameObject.SetActive(true);
        if (firstPersonCamera != null) firstPersonCamera.SetActive(false);

        UpdateMainCameraTransform();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public virtual void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        movementDir = new Vector3(h, 0f, v).normalized;

        if (mainCameraTransform != null)
        {
            Vector3 cameraForward = mainCameraTransform.forward;
            Vector3 cameraRight = mainCameraTransform.right;

            // �̵� ������ �׻� ������ �ǵ��� Y���� 0���� ����
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 desiredMoveDirection = (cameraForward * movementDir.z + cameraRight * movementDir.x).normalized;

            rigid.MovePosition(rigid.position + desiredMoveDirection * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Debug.LogWarning("Main Camera Transform is null!");
        }
    }

    public virtual void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // �÷��̾� ȸ��
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * turnSpeed);

        if (isFirstPerson) // 1��Ī�� �� ī�޶� ���� ȸ���� �߰��� ó��
        {
            // 1��Ī ī�޶� Y�� ȸ�� ����
            cameraRotationX -= mouseY;
            cameraRotationX = Mathf.Clamp(cameraRotationX, -cameraVerticalLimit, cameraVerticalLimit); // ���� �þ߰� ����

            if (firstPersonCamera != null)
            {
                // 1��Ī ī�޶� ������Ʈ ���� ȸ���� ���� -> ���� ������ ����
                firstPersonCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
            }
            else
            {
                Debug.LogWarning("First Person Camera is null!");
            }
        }
    }

    public virtual void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckGrounded(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckGrounded(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        // ����Ʈ���� ������ ���������� Ȯ��
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            groundCnt--; // ���� ���� ���� �� ����
            if (groundCnt <= 0) { isGrounded = false; }
        }
    }

    private void CheckGrounded(Collision collision)
    {
        // ���� ���̾� ������Ʈ�� �浹�ߴ��� Ȯ��
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // �Ʒ��� �浹 (���� �ε��� ���� ���� X)
            foreach (ContactPoint contact in collision.contacts)
            {
                // �÷��̾ ����� ��Ҵ��� Ȯ�� (minGroundDistance���� ũ�� �������� ����)
                if (Vector3.Dot(contact.normal, Vector3.up) > minGroundDistance)
                {
                    if (groundCnt == 0) { groundCnt++; }
                    isGrounded = true;

                    return;
                }
            }
        }
    }

    public virtual void UseSkill()
    {

    }

    public virtual void Interaction()
    {
        // ��Ȳ�� �°� Attack, Mining ������ ���ǰ� ����
    }

    public virtual void Attack()
    {

    }

    public virtual void Mining()
    {

    }

    private void UpdateMainCameraTransform()
    {
        if (isFirstPerson)
        {
            if (firstPersonCamera != null)
            {
                Camera fpCam = firstPersonCamera.GetComponent<Camera>();

                if (fpCam != null) { mainCameraTransform = fpCam.transform; }
                else { Debug.LogWarning("First Person Camera is null!"); }
            }

            else { Debug.LogWarning("First Person Camera is null!"); }
        }

        else
        {
            // 3��Ī�� ���� Cinemachine Brain�� �����ϴ� ���� ������ ī�޶� ���
            if (Camera.main != null) { mainCameraTransform = Camera.main.transform; }
            else { Debug.LogWarning("Main Camera is null!"); }
        }
    }

    public void ToggleCamera()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

            if (thirdPersonCamera != null) { thirdPersonCamera.gameObject.SetActive(!isFirstPerson); }
            if (firstPersonCamera != null) { firstPersonCamera.SetActive(isFirstPerson); }

            UpdateMainCameraTransform();

            if (isFirstPerson) { cameraRotationX = 0f; }
        }
    }

    private void Update()
    {
        ToggleCamera();
        Rotate();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }
}