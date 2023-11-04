
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

	private void Start()
	{
		mesh = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	public void CheckState()
	{
		switch (state)
		{
			case State.IDLE:
				anim.SetBool("Move", false);
				break;
			case State.MOVE:
				anim.SetBool("Move", true);
				break;
			case State.CHASE:
				anim.SetBool("Move", true);
				break;
			case State.ATTACK:
				anim.SetTrigger("Attack");
				break;
			default:
				break;
		}
	}

}
