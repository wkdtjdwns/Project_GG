using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    /* 해야할 거
     * 1. Player 스크립트 (상속 받아서 쓸 수 있도록 하기 - Move, Move, Interact) - Move: 카메라랑 연결해서 마크처럼 카메라 돌린 방향을 기준으로 wasd 가능하도록
     *    1-1. Interact: 채굴, 때찌하기, 스탯[체력, 이속, 점프력, 특수 스탯(둔화 효과 감소, 특별한 광석 채굴 등)
     *
     * 2. 상속 받아서 캐릭터 만들기 (기본, 상시 발동, 액티브 능력자) - 각각 1개씩
     */

    [Header("Player Info")]
    public float moveSpeed = 5f;        // 이동 속도
    public float hp = 100f;             // 체력
    public float turnSpeed = 65f;       // 회전 속도
    public float jumpPower = 5f;        // 점프력

    [Range(0.01f, 1f)] public float minGroundDistance = 0.7f; // 지면으로 간주할 Y축 최소값 (벽과 지면 구분)
    private LayerMask groundLayer;
    private int groundCnt = 0;

    [Header("Special Player Info")]
    public float minePower = 10f;       // 채광 속도(힘)
    public int mineLevel = 1;           // 채광 레벨
    public float dmg = 5f;              // 데미지

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] private GameObject firstPersonCamera; // 플레이어의 자식 오브젝트여야만 함!

    [Header("Other")]
    private Transform mainCameraTransform;       // Main Camera
    public float mouseSensitivity = 3.5f;       // 마우스 감도
    public float cameraVerticalLimit = 80f;     // 카메라 상하 시야 제한

    private bool isFirstPerson = false;

    private Rigidbody rigid;
    private Vector3 movementDir;
    private bool isGrounded;

    private float cameraRotationX = 0f; // 1인칭 카메라 Y축 회전 값 (상하 시점 조절)

    public enum AbilityType             // 스킬 유형 (패시브, 액티브)
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

            // 이동 방향이 항상 수평이 되도록 Y축을 0으로 설정
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

        // 플레이어 회전
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * turnSpeed);

        if (isFirstPerson) // 1인칭일 때 카메라 상하 회전만 추가로 처리
        {
            // 1인칭 카메라 Y축 회전 조절
            cameraRotationX -= mouseY;
            cameraRotationX = Mathf.Clamp(cameraRotationX, -cameraVerticalLimit, cameraVerticalLimit); // 상하 시야각 제한

            if (firstPersonCamera != null)
            {
                // 1인칭 카메라 오브젝트 로컬 회전을 변경 -> 상하 시점을 구현
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
        // 브젝트와의 접촉이 끊어졌는지 확인
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            groundCnt--; // 접촉 중인 지면 수 감소
            if (groundCnt <= 0) { isGrounded = false; }
        }
    }

    private void CheckGrounded(Collision collision)
    {
        // 지면 레이어 오브젝트와 충돌했는지 확인
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // 아래쪽 충돌 (벽에 부딪힐 때는 실행 X)
            foreach (ContactPoint contact in collision.contacts)
            {
                // 플레이어가 지면과 닿았는지 확인 (minGroundDistance보다 크면 지면으로 간주)
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
        // 상황에 맞게 Attack, Mining 등으로 사용되게 ㄱㄱ
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
            // 3인칭일 때는 Cinemachine Brain이 제어하는 실제 렌더링 카메라를 사용
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