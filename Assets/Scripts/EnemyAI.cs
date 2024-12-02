using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState {
        Patrolling,
        Searching,
        Chasing
    }
    public EnemyState currentState;
    private NavMeshAgent _AIAgent;
    [SerializeField] Transform[] _patrolPoints;
    void Awake(){
        _AIAgent= GetComponent<NavMeshAgent>();
    }
    void Start(){
        currentState=EnemyState.Patrolling;
        SetRandomPatrolPoint();
    }
    void Update(){
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
            break;
            case EnemyState.Searching:
                Search();
            break;
            case EnemyState.Chasing:
                Chase();
            break;
        }
    }

    void Patrol(){
        if(_AIAgent.remainingDistance < 0.3f) SetRandomPatrolPoint();
    }
    void Search(){

    }
    void Chase(){

    }
    void SetRandomPatrolPoint(){
        _AIAgent.destination=_patrolPoints[Random.Range(0, _patrolPoints.Length)].position;
    }
    void OnDrawGizmos(){
        foreach (Transform point in _patrolPoints){
            Gizmos.color=Color.blue;
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}
