using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;     //속도
    [SerializeField] private float damage = 10f;    //데미지
    [SerializeField] private float lifetime = 3f;   //삶.

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
