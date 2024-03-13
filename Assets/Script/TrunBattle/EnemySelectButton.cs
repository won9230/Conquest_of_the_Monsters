using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
	public GameObject enemyPrefab;

	public void SelectEnemy()
	{
		BattleStateMaschine BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		BSM.Input2(enemyPrefab);
		BattleOrder tmpbattleOrder = BSM.battleOrders[0];
		BSM.battleOrders.RemoveAt(0);
		BSM.battleOrders.Add(tmpbattleOrder);
	}

	public void HideSelector()
	{
			enemyPrefab.transform.Find("Selector").gameObject.SetActive(false);
	}	
	public void ShowSelector()
	{
			enemyPrefab.transform.Find("Selector").gameObject.SetActive(true);
	}
}
