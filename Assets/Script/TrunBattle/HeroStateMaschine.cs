using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMaschine : MonoBehaviour
{
	private BattleStateMaschine BSM;
	public BaseHero hero;

	public enum TurnState
	{
		Processing,
		Addtolist,
		Waiting,
		Selecting,
		Action,
		Dead
	}

	public TurnState currentState;
	public Slider hpBarSlider;

	private float cur_cooldown = 0f;
	private float max_cooldown = 5f;

	private void Start()
	{
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		currentState = TurnState.Processing;
	}

	private void Update()
	{
		Debug.Log(currentState);
		switch (currentState)
		{
			case TurnState.Processing:
				UpgradeProgressBar();
				break;
			case TurnState.Addtolist:
				BSM.heroToManger.Add(this.gameObject);
				currentState = TurnState.Waiting;
				break;
			case TurnState.Waiting:
				break;
			case TurnState.Selecting:
				break;
			case TurnState.Action:
				break;
			case TurnState.Dead:
				break;
			default:
				break;
		}

		
	}
	//행동 게이지 쿨타임
	private void UpgradeProgressBar()
	{
		cur_cooldown = cur_cooldown + Time.deltaTime;
		if(cur_cooldown >= max_cooldown)
		{
			currentState = TurnState.Addtolist;
		}
	}
}
