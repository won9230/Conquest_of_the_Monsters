using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
	private const string player = "Player";

	private EnemyEntity enemyEntity;
	private void Start()
	{
		enemyEntity = transform.parent.gameObject.GetComponent<EnemyEntity>();
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(player))
		{
			enemyEntity.player = other.gameObject;
			enemyEntity.state = EnemyEntity.State.Attack;
		}

	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(player))
		{
			enemyEntity.player = null;
			enemyEntity.state = EnemyEntity.State.Chase;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 1.5f);
	}
}
