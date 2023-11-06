
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyEntity : MonoBehaviour
{
	private NavMeshAgent mesh;
	[SerializeField] private EnemyAttackRange enemyAttackRange;
	private Animator anim;
	public enum State
	{
		IDLE,		//평소
		MOVE,		//움직임
		CHASE,		//추격
		ATTACK		//공격
	}

	[HideInInspector] public State state = State.IDLE;

	public new string name;
	public int hp;
	public int mp;
	public int range;

	[SerializeField] private GameObject attackRange;
	[HideInInspector] public GameObject player;

	private int moveTime = 0;
	private void Start()
	{
		mesh = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		StartCoroutine(CheckState());
	}

	IEnumerator CheckState()
	{
		while (true)
		{
			Debug.Log(state);
			switch (state)
			{
				case State.IDLE:
					anim.SetBool("Move", false);
					//TODO : 플레이어가 멀어지면 자신의 구역으로 돌아서 이 상태로 변경
					moveTime++;
					if (moveTime >= 4)
					{
						state = State.MOVE;
					}
					break;
				case State.MOVE:
					anim.SetBool("Move", true);
					//TODO : 그냥 이따금 랜덤으로 움직임
					moveTime = 0;
					Vector3 mtmp = transform.position - UnityEngine.Random.insideUnitSphere * 2;
					Vector3 ptmp = transform.position + UnityEngine.Random.insideUnitSphere * 2;
					int random = UnityEngine.Random.Range(0, 1);
					if(random == 1)
					{
						mesh.SetDestination(mtmp);
					}
					else
					{
						mesh.SetDestination(ptmp);
					}
					state = State.IDLE;
					break;
				case State.CHASE:
					//TODO : 플레이어가 범위안에 들어오면 추격함
					if (player != null)
					{
						mesh.SetDestination(player.gameObject.transform.position);
						Debug.Log(player.gameObject.transform.position);
					}
					anim.SetBool("Move", true);
					break;
				case State.ATTACK:
					anim.SetTrigger("Attack");
					//TODO : 플레이어랑 가까이 왔을 때 배틀 페이지로 넘어감
					break;
				default:
					break;
			}
			yield return new WaitForSeconds(0.5f);
		}
		
	}

}
