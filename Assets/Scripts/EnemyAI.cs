using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState {
        Patrolling,
        Searching,
        Chasing
    }
    public EnemyState currentState;
    private NavMeshAgent _AIAgent;
    private Transform playerTransform;
    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] Vector2 _patrolAreaSize = new Vector2(17,15);
    [SerializeField] Transform _patrolAreaCenter;

    [SerializeField] float _visionRange=12f;
    [SerializeField] float _visionAngle=120f;
    private Vector3 _playerLastPosition;
    float searchTimer;
    float searchWaitTime = 15f;
    [SerializeField] float _searchRadiuus = 10f;
    void Awake(){
        _AIAgent= GetComponent<NavMeshAgent>();
        playerTransform= GameObject.FindWithTag("Player").transform;
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
        if(OnRange()){
            currentState=EnemyState.Chasing;
        }
    }
    void Search(){
        if(OnRange()) currentState=EnemyState.Chasing;
        searchTimer +=Time.deltaTime;
        if(searchTimer<searchWaitTime){
            if(_AIAgent.remainingDistance<0.5f){
                Vector3 randomPoint;
                if(RandomSearchPoint(_playerLastPosition, _searchRadiuus, out randomPoint)) _AIAgent.destination = randomPoint;
                else {
                    currentState=EnemyState.Patrolling;
                    searchTimer=0f;
                }
            }
        }
    }
    bool RandomSearchPoint(Vector3 center, float radius, out Vector3 point){
        Vector3 randomPoint = center + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint,out hit,4, NavMesh.AllAreas)){
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
        return false;
    }
    bool OnRange(){
        Vector3 directionToPlayer= playerTransform.position - transform.position;
        float angleToplayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position,playerTransform.position);
        if(playerTransform.position == _playerLastPosition) return true;
        if(distanceToPlayer > _visionRange) return false;
        if(angleToplayer > _visionAngle*0.5f) return false;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer)){
            if(hit.collider.CompareTag("Player")){
                _playerLastPosition= playerTransform.position;
                return true;
            }else return false;
        }
        return true;

        if(distanceToPlayer < _visionRange) {
            if(angleToplayer<_visionAngle*0.5f) return true;
            else return  false;
        }else return false;
    }
    void Chase(){
        if(!OnRange()) currentState=EnemyState.Searching; 
        _AIAgent.destination = playerTransform.position;
    }
    void SetRandomPatrolPoint(){
        // _AIAgent.destination=_patrolPoints[Random.Range(0, _patrolPoints.Length)].position;
        float RandomX = Random.Range(-_patrolAreaSize.x*0.5f,_patrolAreaSize.x*0.5f);
        float RandomZ = Random.Range(-_patrolAreaSize.y*0.5f,_patrolAreaSize.y*0.5f);
        Vector3 randomPoint = new Vector3(RandomX, 0,RandomZ) + _patrolAreaCenter.position;
        _AIAgent.destination = randomPoint;
    }
    void OnDrawGizmos(){
        // foreach (Transform point in _patrolPoints){
        //     Gizmos.color=Color.blue;
        //     Gizmos.DrawWireSphere(point.position, 0.5f);
        // }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _visionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolAreaCenter.position, new Vector3(_patrolAreaSize.x,1,_patrolAreaSize.y));

        Gizmos.color = Color.magenta;
        Vector3 fovLine1= Quaternion.AngleAxis(_visionAngle*0.5f, transform.up) * transform.forward*_visionRange;
        Vector3 fovLine2= Quaternion.AngleAxis(-_visionAngle*0.5f, transform.up) * transform.forward*_visionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
