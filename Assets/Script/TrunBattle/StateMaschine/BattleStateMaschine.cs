using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public enum HeroGUI
	{
		Activate,
		Waiting,
		Input1,
		Input2,
		Done,
	}

	public HeroGUI heroInput;

	public List<GameObject> heroToManger = new List<GameObject>();
	private HandleTrun heroChoise;

	public GameObject enemyButton;
	public Transform spacer;

	public GameObject attackPanel;
	public GameObject enemySelectPanel;


	private void Start()
	{
		battleState = PerformAction.Wait;
		enemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
		heroInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
		heroInput = HeroGUI.Activate;

		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(false);

		EnemyButtons();
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
					for (int i = 0; i < heroInBattle.Count; i++)
					{
						if(performList[0].attackersTarget == heroInBattle[i])
						{
							ESM.heroToAttack = performList[0].attackersTarget;
							ESM.currentState = EnemyStateMaschine.TurnState.Action;
							break;
						}
						else
						{
							performList[0].attackersTarget = heroInBattle[Random.Range(0,heroInBattle.Count)];
							ESM.heroToAttack = performList[0].attackersTarget;
							ESM.currentState = EnemyStateMaschine.TurnState.Action;
						}
					}
				}
				if(performList[0].Type == "Hero")
				{
					HeroStateMaschine HSM = performer.GetComponent<HeroStateMaschine>();
					HSM.enemyToAttack = performList[0].attackersTarget;
					HSM.currentState = HeroStateMaschine.TurnState.Action;
				}
				battleState = PerformAction.PerformAction;
				break;
			case PerformAction.PerformAction:
				break;
			default:
				break;
		}

		switch (heroInput)
		{
			case HeroGUI.Activate:
				if(heroToManger.Count > 0)
				{
					heroToManger[0].transform.Find("Selector").gameObject.SetActive(true);
					heroChoise = new HandleTrun();
						
					attackPanel.SetActive(true);
					heroInput = HeroGUI.Waiting;
				}
				break;
			case HeroGUI.Waiting:
				//TODO : idle
				break;
			case HeroGUI.Input1:
				break;
			case HeroGUI.Input2:
				break;
			case HeroGUI.Done:
				HeroInputDone();
				break;
		}
	}
	

	public void CollectActions(HandleTrun input)
	{
		performList.Add(input);
	}

	//UI에 Enemy버튼과 Text 띄우기
	private void EnemyButtons()
	{
		foreach (GameObject enemy in enemyInBattle)
		{
			GameObject newButton = Instantiate(enemyButton);
			EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

			EnemyStateMaschine cur_enemy = enemy.GetComponent<EnemyStateMaschine>();

			Text buttonText = newButton.transform.Find("Text").GetComponent<Text>();
			buttonText.text = cur_enemy.name;

			button.enemyPrefab = enemy;

			newButton.transform.SetParent(spacer,false);
		}
	}

	public void Input1()	//attack 버튼
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";

		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(true);
	}

	public void Input2(GameObject choosenEnemy)	//적 선택
	{
		heroChoise.attackersTarget = choosenEnemy;
		heroInput = HeroGUI.Done;
	}

	private void HeroInputDone()
	{
		performList.Add(heroChoise);
		enemySelectPanel.SetActive(false);
		heroToManger[0].transform.Find("Selector").gameObject.SetActive(false);
		heroToManger.RemoveAt(0);
		heroInput = HeroGUI.Activate;
	}
}
