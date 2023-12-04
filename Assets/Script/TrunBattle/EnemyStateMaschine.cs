using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMaschine : MonoBehaviour
{
	public BaseEnemy enemy;

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
	private void UpgradeProgressBar()
	{
		cur_cooldown = cur_cooldown + Time.deltaTime;
		hpBarSlider.value = cur_cooldown / max_cooldown;
		if (cur_cooldown >= max_cooldown)
		{
			currentState = TurnState.Addtolist;
		}
	}
}
