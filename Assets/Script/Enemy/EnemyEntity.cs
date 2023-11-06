
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
		IDLE,		//���
		MOVE,		//������
		CHASE,		//�߰�
		ATTACK		//����
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
					//TODO : �÷��̾ �־����� �ڽ��� �������� ���Ƽ� �� ���·� ����
					moveTime++;
					if (moveTime >= 4)
					{
						state = State.MOVE;
					}
					break;
				case State.MOVE:
					anim.SetBool("Move", true);
					//TODO : �׳� �̵��� �������� ������
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
					//TODO : �÷��̾ �����ȿ� ������ �߰���
					if (player != null)
					{
						mesh.SetDestination(player.gameObject.transform.position);
						Debug.Log(player.gameObject.transform.position);
					}
					anim.SetBool("Move", true);
					break;
				case State.ATTACK:
					anim.SetTrigger("Attack");
					//TODO : �÷��̾�� ������ ���� �� ��Ʋ �������� �Ѿ
					break;
				default:
					break;
			}
			yield return new WaitForSeconds(0.5f);
		}
		
	}

}
