using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret settings")]
    [SerializeField] Transform m_rigidBody;     //����
    [SerializeField] float m_range = 0f;        //�����Ÿ�
    [SerializeField] LayerMask m_layerMask = 0; //���̾� ����ũ
    [SerializeField] float m_spinSpeed = 0f;    //ȸ�� �ӵ�
    [SerializeField] float m_fireRate = 0f;     //�߻� �ӵ�

    [Header("Bullet")]
    [SerializeField] Transform shotPosition;    //�Ѿ� �߻� ��ġ
    [SerializeField] GameObject bullet;         //�Ѿ� ������

    float m_currentFireRate; //���� �߻� �ӵ�

    Transform m_target = null;                    //���� Ÿ��

    /// <summary>
    /// �����ص� ���� �ȿ��� ����� �� Ž��(LayerMask�� �ش��ϴ� ��)
    /// </summary>
    void SerarchEnemy()
    {
        Collider[] t_cols = Physics.OverlapSphere(transform.position, m_range, m_layerMask); //���� �ȿ� �ִ� ���̸���ũ�γ� ã�ƿ�
        Transform shortTarget = null;   //���� ����� ��

        if(t_cols.Length > 0)
        {
            float t_shortDistance = float.MaxValue; //�ּ� �Ÿ� �ʱ�ȭ
            foreach (Collider collider in t_cols)
            {
                //float distance = Vector3.Distance(transform.position, collider.transform.position); //�Ÿ� ���
                float t_distance = Vector3.SqrMagnitude(transform.position - collider.transform.position); //���� �Ÿ� ��� (���� ����ȭ)
                if (t_distance < t_shortDistance) //���� ����� �� ã��
                {
                    t_shortDistance = t_distance;
                    shortTarget = collider.transform;
                }
            }

            m_target = shortTarget; //���� ����� ���� Ÿ������ ����
        }
    }

    private void Start()
    {
        m_currentFireRate = m_fireRate; //���� �߻� �ӵ� �ʱ�ȭ
        //�Լ� �̸�, ȣ����� ��� �ð�, �ݺ� �ֱ�
        InvokeRepeating("SerarchEnemy", 0f, 0.7f); //0.7�ʸ��� �� Ž��
    }

    private void Update()
    {
        //Ÿ���� ���ٸ�
        if (m_target == null)
        {
            m_rigidBody.Rotate(new Vector3(0, 45, 0) * Time.deltaTime); //���� ȸ��
        }
        else
        {
            Quaternion t_lookRotation = Quaternion.LookRotation(m_target.position); //Ÿ�� ��ġ
            Vector3 t_euler = Quaternion.RotateTowards(m_rigidBody.rotation, t_lookRotation, m_spinSpeed * Time.deltaTime).eulerAngles; //�ε巴�� ȸ��(���Ϸ���)
            m_rigidBody.rotation = Quaternion.Euler(0, t_euler.y, 0); //���� ȸ��

            Quaternion t_fireRotation = Quaternion.Euler(0, t_lookRotation.eulerAngles.y, 0);   //���� ����
            if(Quaternion.Angle(m_rigidBody.rotation, t_fireRotation) < 5f)
            {
                m_currentFireRate -= Time.deltaTime; //�߻� �ӵ� ����
                if (m_currentFireRate <= 0f) //�߻� �ӵ��� 0 ���϶��
                {
                    m_currentFireRate = m_fireRate; //�߻� �ӵ� �ʱ�ȭ

                    GameObject t_bullet = Instantiate(bullet, shotPosition.position, t_fireRotation); // �Ѿ� ������ ����
                    t_bullet.GetComponent<TurretBullet>().Initialize(m_target.position);              // Ÿ�� ����
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_range); // m_range �ð�ȭ
    }
}