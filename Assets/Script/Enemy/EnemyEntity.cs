
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEntity : MonoBehaviour
{
	public NavMeshAgent mesh;

	public Animator anim;
	public float playerDist;
	public enum State
	{
		IDLE,       //���
		MOVE,       //������
		CHASE,      //�߰�
		ATTACK      //����
	}

	[HideInInspector] public State state = State.IDLE;

	public new string name;
	public int hp;
	public int mp;
	public int range;
	[SerializeField] private float enemyAttackRange;
	[SerializeField] private GameObject attackRange;
	[HideInInspector] public GameObject player;
}
