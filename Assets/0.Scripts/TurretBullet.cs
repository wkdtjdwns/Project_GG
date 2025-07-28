using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;     //�ӵ�
    [SerializeField] private float damage = 10f;    //������
    [SerializeField] private float lifetime = 3f;   //��.

    private Vector3 targetPosition;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                HitTarget();
            }
        }
    }

    private void HitTarget()
    {
        Destroy(gameObject);
    }
}
