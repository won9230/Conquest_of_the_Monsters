using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMaschine : MonoBehaviour
{
	public BaseEnemy enemy;
	public BattleStateMaschine BSM;
	public enum TurnState
	{
		Processing,
		ChooseAction,
		Waiting,
		Action,
		Dead
	}

	public TurnState currentState;
	public Slider hpBarSlider;

	private float cur_cooldown = 0f;
	private float max_cooldown = 5f;

	private Vector3 startPosition;

	private void Start()
	{
		currentState = TurnState.Processing;
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		startPosition = transform.position;
	}

	private void Update()
	{
		Debug.Log(currentState);
		switch (currentState)
		{
			case TurnState.Processing:
				UpgradeProgressBar();
				break;
			case TurnState.ChooseAction:
				ChooseAction();
				currentState = TurnState.Waiting;
				break;
			case TurnState.Waiting:
				//idle state
				break;
			case TurnState.Action:
				break;
			case TurnState.Dead:
				break;
			default:
				break;
		}


	}
	private void UpgradeProgressBar()
	{
		cur_cooldown = cur_cooldown + Time.deltaTime;
		hpBarSlider.value = cur_cooldown / max_cooldown;

		if (cur_cooldown >= max_cooldown)
		{
			currentState = TurnState.ChooseAction;
		}
	}

	private void ChooseAction()
	{
		HandleTrun myAttack = new HandleTrun();
		myAttack.attacker = enemy.name;
		myAttack.attackersGamgeObject = this.gameObject;
		myAttack.attackersTarget = BSM.heroInBattle[Random.Range(0, BSM.heroInBattle.Count)];
		BSM.CollectActions(myAttack);
	}
}
