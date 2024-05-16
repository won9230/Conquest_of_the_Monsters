
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
		Idle,       //���
		Move,       //������
		Chase,      //�߰�
		Attack		//����
	}

	[HideInInspector] public State state = State.Idle;

	public new string name;
	//public int range;
	public float enemyAttackRange = 1.5f;
	//[SerializeField] private GameObject chaseRange;
	[HideInInspector] public GameObject player;
}
