using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //SetDestination = �񵿱���
        //Update = ������
        // == Update������ ù �������� SetDestination�� �ݿ����� ���� -> remainingDistance = 0 -> stoppingDistance���� ���� -> Attak ȣ��
        agent.SetDestination(target.position);

        //pathPending: ��� ��� ������ �Ǵ���.(Update�� ù �����ӿ��� ȣ�� �ϴ°� ��������).
        //PathComplete: ���� ��ġ���� �� ��ġ���� ��ȿ�� ��ΰ� ������
        //(���� ��ġ���� ���������� ���� �Ÿ�) - (stop ��ġ) < 0.1
        if (!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete
                && agent.remainingDistance - agent.stoppingDistance < .1f)
        {
            StartCoroutine(Attack());
        }
    }

    public IEnumerator Attack()
    {
        Debug.Log("����. �ٺ�.");

        yield return null;
    }
}
