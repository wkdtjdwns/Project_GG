using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret settings")]
    [SerializeField] Transform m_rigidBody;     //몸통
    [SerializeField] float m_range = 0f;        //사정거리
    [SerializeField] LayerMask m_layerMask = 0; //레이어 마스크
    [SerializeField] float m_spinSpeed = 0f;    //회전 속도
    [SerializeField] float m_fireRate = 0f;     //발사 속도

    [Header("Bullet")]
    [SerializeField] Transform shotPosition;    //총알 발사 위치
    [SerializeField] GameObject bullet;         //총알 프리팹

    float m_currentFireRate; //현재 발사 속도

    Transform m_target = null;                    //현재 타겟

    /// <summary>
    /// 설정해둔 범위 안에서 가까운 적 탐색(LayerMask에 해당하는 적)
    /// </summary>
    void SerarchEnemy()
    {
        Collider[] t_cols = Physics.OverlapSphere(transform.position, m_range, m_layerMask); //범위 안에 있는 레이마스크인놈 찾아옴
        Transform shortTarget = null;   //가장 가까운 적

        if(t_cols.Length > 0)
        {
            float t_shortDistance = float.MaxValue; //최소 거리 초기화
            foreach (Collider collider in t_cols)
            {
                //float distance = Vector3.Distance(transform.position, collider.transform.position); //거리 계산
                float t_distance = Vector3.SqrMagnitude(transform.position - collider.transform.position); //제곱 거리 계산 (성능 최적화)
                if (t_distance < t_shortDistance) //가장 가까운 적 찾기
                {
                    t_shortDistance = t_distance;
                    shortTarget = collider.transform;
                }
            }

            m_target = shortTarget; //가장 가까운 적을 타겟으로 설정
        }
    }

    private void Start()
    {
        m_currentFireRate = m_fireRate; //현재 발사 속도 초기화
        //함수 이름, 호출까지 대기 시간, 반복 주기
        InvokeRepeating("SerarchEnemy", 0f, 0.7f); //0.7초마다 적 탐색
    }

    private void Update()
    {
        //타겟이 없다면
        if (m_target == null)
        {
            m_rigidBody.Rotate(new Vector3(0, 45, 0) * Time.deltaTime); //몸통 회전
        }
        else
        {
            Quaternion t_lookRotation = Quaternion.LookRotation(m_target.position); //타겟 위치
            Vector3 t_euler = Quaternion.RotateTowards(m_rigidBody.rotation, t_lookRotation, m_spinSpeed * Time.deltaTime).eulerAngles; //부드럽게 회전(오일러값)
            m_rigidBody.rotation = Quaternion.Euler(0, t_euler.y, 0); //실제 회전

            Quaternion t_fireRotation = Quaternion.Euler(0, t_lookRotation.eulerAngles.y, 0);   //조준 방향
            if(Quaternion.Angle(m_rigidBody.rotation, t_fireRotation) < 5f)
            {
                m_currentFireRate -= Time.deltaTime; //발사 속도 감소
                if (m_currentFireRate <= 0f) //발사 속도가 0 이하라면
                {
                    m_currentFireRate = m_fireRate; //발사 속도 초기화

                    GameObject t_bullet = Instantiate(bullet, shotPosition.position, t_fireRotation); // 총알 프리팹 생성
                    t_bullet.GetComponent<TurretBullet>().Initialize(m_target.position);              // 타겟 설정
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_range); // m_range 시각화
    }
}