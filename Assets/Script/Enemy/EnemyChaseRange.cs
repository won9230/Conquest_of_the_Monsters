using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseRange : MonoBehaviour
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
			enemyEntity.state = EnemyEntity.State.CHASE;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(player))
		{
			enemyEntity.state = EnemyEntity.State.IDLE;
		}
	}
}
