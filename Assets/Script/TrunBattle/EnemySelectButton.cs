using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
	public GameObject enemyPrefab;

	public void SelectEnemy()
	{
		GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
	}
}
