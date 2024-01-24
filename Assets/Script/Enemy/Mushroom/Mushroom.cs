using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Mushroom : EnemyEntity
{
	public int actionRadius = 4;
	private void Start()
	{
		mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = GetComponent<Animator>();
		StartCoroutine(CheckState());
	}
	private int moveTime = 0;
	IEnumerator CheckState()
	{
		while (true)
		{
			switch (state)
			{
				case State.IDLE:
					//TODO : 플레이어가 멀어지면 자신의 구역으로 돌아서 이 상태로 변경
					anim.SetBool("Move", false);
					moveTime++;
					if (moveTime >= 30)
					{
						state = State.MOVE;
					}
					break;
				case State.MOVE:
					//TODO : 그냥 이따금 랜덤으로 움직임
					anim.SetBool("Move", true);
					moveTime = 0;
					Vector3 mtmp = transform.position - Random.insideUnitSphere * actionRadius;
					Vector3 ptmp = transform.position + Random.insideUnitSphere * actionRadius;
					int random = Random.Range(0, 1);
					if (random == 1)
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
					mesh.isStopped = false;
					mesh.ResetPath();
					mesh.updatePosition = true;
					mesh.updateRotation = true;
					if (player != null)
					{
						mesh.SetDestination(player.gameObject.transform.position);
						playerDist = Vector3.Distance(player.gameObject.transform.position, this.gameObject.transform.position);
						
						if (playerDist <= enemyAttackRange)
						{
							state = State.ATTACK;
						}
					}
					anim.SetBool("Move", true);
					break;
				case State.ATTACK:
					//TODO : 플레이어랑 가까이 왔을 때 공격함
					anim.SetBool("Move", false);
					mesh.isStopped = true;
					mesh.velocity = Vector3.zero;
					mesh.updatePosition = false;
					//mesh.updateRotation = false;
					anim.SetTrigger("Attack");
					playerDist = Vector3.Distance(player.gameObject.transform.position, this.gameObject.transform.position);
					
					if (player != null && playerDist >= enemyAttackRange)
					{
						state = State.CHASE;
					}
					yield return new WaitForSeconds(1f);
					break;
				default:
					break;
			}
			yield return new WaitForSeconds(0.1f);
		}

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 1.5f);
	}
}
