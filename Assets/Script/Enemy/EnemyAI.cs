using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : EnemyEntity
{
	public int actionRadius = 4;
	public int monsterID = 0;

	private void Start()
	{
		mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = GetComponent<Animator>();
		StartCoroutine(CheckState());
	}
	private int curMoveTime = 0;

	IEnumerator CheckState()
	{

		while (true)
		{
			//Debug.Log(state);
			switch (state)
			{
				case State.Idle:
					//TODO : �÷��̾ �־����� �ڽ��� �������� ���Ƽ� �� ���·� ����
					int moveTime = Random.Range(25, 35);
					if (player != null)
						state = State.Chase;

					anim.SetBool("Move", false);
					mesh.ResetPath();
					curMoveTime++;
					if (curMoveTime >= moveTime)
					{
						moveTime = Random.Range(25, 35);
						state = State.Move;
					}

					break;
				case State.Move:
					//TODO : �̵��� �������� ������
					if (player != null)
						state = State.Chase;

					anim.SetBool("Move", true);

					curMoveTime = 0;
					mesh.isStopped = false;
					mesh.ResetPath();
					mesh.updatePosition = true;
					mesh.updateRotation = true;

					Vector3 mtmp = transform.position - Random.insideUnitSphere * actionRadius;
					Vector3 ptmp = transform.position + Random.insideUnitSphere * actionRadius;

					int random = Random.Range(0, 1);
					if (random == 1)
						mesh.SetDestination(mtmp);
					else
						mesh.SetDestination(ptmp);
					while (mesh.pathPending || mesh.remainingDistance > mesh.stoppingDistance)
					{
						if (player != null)
						{
							break;
						}
						yield return null;
					}
					state = State.Idle;
					break;
				case State.Chase:
					//TODO : �÷��̾ �����ȿ� ������ �߰���
					mesh.isStopped = false;
					mesh.ResetPath();
					mesh.updatePosition = true;
					mesh.updateRotation = true;

					if (player != null)
					{
						mesh.SetDestination(player.gameObject.transform.position);
						playerDist = Vector3.Distance(player.gameObject.transform.position, this.gameObject.transform.position);
						anim.SetBool("Move", true);

						if (playerDist <= enemyAttackRange)
						{
							state = State.Attack;
						}
					}
					else
					{
						state = State.Idle;
					}
					break;
				case State.Attack:
					//TODO : �÷��̾�� ������ ���� �� ������
					mesh.isStopped = true;
					mesh.velocity = Vector3.zero;
					mesh.updatePosition = false;
					mesh.updateRotation = false;

					anim.SetBool("Move", false);
					anim.SetTrigger("Attack");

					yield return new WaitForSeconds(1f);

					if (player != null)
					{
						playerDist = Vector3.Distance(player.gameObject.transform.position, this.gameObject.transform.position);
						if (playerDist >= enemyAttackRange)
						{
							state = State.Chase;
						}
					}
					else
					{
						mesh.ResetPath();
						state = State.Idle;
					}
					break;
				default:
					break;
			}
			yield return new WaitForSeconds(0.1f);
		}

	}
}
