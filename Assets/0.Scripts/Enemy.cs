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
        //SetDestination = 비동기적
        //Update = 동기적
        //Update에서의 첫 프레임은 SetDestination가 반영되지 않음 -> remainingDistance = 0 -> stoppingDistance보다 작음 -> Attak 호출
        agent.SetDestination(target.position);

        //pathPending: 경로 계산 중인지 판단함.(Update의 첫 프레임에서 호출 하는걸 방지해줌).
        //PathComplete: 현재 위치에서 그 위치까지 유효한 경로가 존재함
        //(현재 위치에서 목적지까지 남은 거리) - (stop 위치) < 0.1
        if (!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete
                && agent.remainingDistance - agent.stoppingDistance < .1f)
        {
            StartCoroutine(Attack());
        }
    }

    public IEnumerator Attack()
    {
        //Debug.Log("Attack");

        yield return null;
    }
}
