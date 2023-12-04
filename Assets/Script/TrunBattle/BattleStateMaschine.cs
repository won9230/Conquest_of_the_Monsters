using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMaschine : MonoBehaviour
{
	public enum PerformAction
	{
		Wait,
		TakeAction,
		PerformAction
	}
	public PerformAction battleState;

	public List<HandleTrun> preformList = new List<HandleTrun>();
	public List<GameObject> heroInBattle = new List<GameObject>();
	public List<GameObject> enemyInBattle = new List<GameObject>();



	private void Start()
	{
		battleState = PerformAction.Wait;
		enemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
		heroInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
	}

	private void Update()
	{
		switch (battleState)
		{
			case PerformAction.Wait:
				break;
			case PerformAction.TakeAction:
				break;
			case PerformAction.PerformAction:
				break;
			default:
				break;
		}
	}
	
	public void CollectActions(HandleTrun input)
	{
		preformList.Add(input);
	}
}
