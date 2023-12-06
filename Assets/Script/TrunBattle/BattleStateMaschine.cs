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

	public List<HandleTrun> performList = new List<HandleTrun>();
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
				if(performList.Count > 0)
				{
					battleState = PerformAction.TakeAction;
				}
				break;
			case PerformAction.TakeAction:
				GameObject performer = GameObject.Find(performList[0].attacker);
				if(performList[0].Type == "Enemy")
				{
					EnemyStateMaschine ESM = performer.GetComponent<EnemyStateMaschine>();
					ESM.heroToAttack = performList[0].attackersTarget;
					ESM.currentState = EnemyStateMaschine.TurnState.Action;
				}
				if(performList[0].Type == "Hero")
				{

				}
				battleState = PerformAction.PerformAction;
				break;
			case PerformAction.PerformAction:
				break;
			default:
				break;
		}
	}
	
	public void CollectActions(HandleTrun input)
	{
		performList.Add(input);
	}
}
