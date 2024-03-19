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
		PerformAction,
		checkAlive,
		Win,
		Lose
	}
	public PerformAction battleState;

	public List<HandleTrun> performList = new List<HandleTrun>();
	public List<BattleOrder> battleOrders = new List<BattleOrder>();
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

	public GameObject attackPanel;		//attack 버튼
	public GameObject enemySelectPanel;	//적 선택 버튼
	public GameObject magicPanel;		//magic버튼
	public GameObject UIPanel;			//UI 패널 전채(Canve)

	public Transform actionSpacer;		//action 부모
	public Transform magicSpacer;		//magic버튼 부모
	public GameObject actionButton;
	public GameObject magicButton;
	private List<GameObject> atkBtns = new List<GameObject>();

	private List<GameObject> enemyBtns = new List<GameObject>();

	public List<Transform> enemySpawnPoints = new List<Transform>();
	public List<Transform> heroSpawnPoints = new List<Transform>();
	
	private void Awake()
	{
		for (int i = 0; i < GameManager.instance.enemyAmount; i++)
		{
			GameObject newEnemy = Instantiate(GameManager.instance.enemyToBattle[i], enemySpawnPoints[i].position,Quaternion.Euler(new Vector3(0,180,0)));
			newEnemy.name = newEnemy.GetComponent<EnemyStateMaschine>().enemy.theName + "_" + (i+1);
			newEnemy.GetComponent<EnemyStateMaschine>().enemy.theName = newEnemy.name;
			enemyInBattle.Add(newEnemy);
		}
		for (int i = 0;i < GameManager.instance.heroParty.Length; i++)
		{
			GameObject newHero = Instantiate(GameManager.instance.heroParty[i], heroSpawnPoints[i].position, Quaternion.Euler(new Vector3(0, 0, 0)));
			newHero.name = newHero.GetComponent<HeroStateMaschine>().hero.theName;
			heroInBattle.Add(newHero);
		}
	}

	private void Start()
	{
		battleState = PerformAction.Wait;
		//enemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
		//heroInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
		heroInput = HeroGUI.Activate;

		//순서 정하기
		HeroOrderAdd();
		EnemyOrderAdd();
		BattleOrderSort();

		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(false);
		magicPanel.SetActive(false);

		EnemyButtons();
	}

	private void Update()
	{
		//Debug.Log("GameState " + battleState);
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
				//idle
				break;
			case PerformAction.checkAlive:
				if(heroInBattle.Count < 1)
				{
					battleState = PerformAction.Lose;
					//배틀 짐
					Debug.Log("배틀 패배");
					for (int i = 0; i < heroInBattle.Count; i++)
					{
						heroInBattle[i].GetComponent<HeroStateMaschine>().currentState = HeroStateMaschine.TurnState.Waiting;
					}
				}
				else if(enemyInBattle.Count < 1)
				{
					battleState = PerformAction.Win;
					//배틀 이김
					Debug.Log("배틀 승리");
				}
				else
				{
					ClearAttackPanel();
					heroInput = HeroGUI.Activate;
				}
				break;
			case PerformAction.Win:
				Debug.Log("승리");
				for (int i = 0; i < heroInBattle.Count; i++)
				{
					heroInBattle[i].GetComponent<HeroStateMaschine>().currentState = HeroStateMaschine.TurnState.Waiting;
				}

				GameManager.instance.LoadSceneAfterBattle();
				GameManager.instance.gamestate = GameManager.GameState.World_State;
				GameManager.instance.enemyToBattle.Clear();
				break;
			case PerformAction.Lose:
				Debug.Log("패배");
				break;
			default:
				break;
		}

		switch (heroInput)
		{
			case HeroGUI.Activate:
				if(heroToManger.Count > 0 && battleOrders[0].heroToEnemy == HeroToEnemy.Hero)
				{
					heroToManger[0].transform.Find("Selector").gameObject.SetActive(true);
					heroChoise = new HandleTrun();

					UIPanel.SetActive(true);
					attackPanel.SetActive(true);
					CreateAttackButtons();

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

	//히어로 battleOrder에 추가
	private void HeroOrderAdd()
	{
		for (int i = 0; i < heroInBattle.Count; i++)
		{
			BattleOrder newBattleOrder = new BattleOrder();
			HeroStateMaschine tmpHero = heroInBattle[i].GetComponent<HeroStateMaschine>();
			newBattleOrder.attackerName = tmpHero.name;
			newBattleOrder.ahility = tmpHero.hero.ahility;
			newBattleOrder.heroToEnemy = HeroToEnemy.Hero;
			battleOrders.Add(newBattleOrder);
		}
	}

	//적 battleOrder에 추가
	private void EnemyOrderAdd()
	{
		for (int i = 0; i < enemyInBattle.Count; i++)
		{
			BattleOrder newBattleOrder = new BattleOrder();
			EnemyStateMaschine tmpEnemy = enemyInBattle[i].GetComponent<EnemyStateMaschine>();
			newBattleOrder.attackerName = tmpEnemy.name;
			newBattleOrder.ahility = tmpEnemy.enemy.ahility;
			newBattleOrder.heroToEnemy = HeroToEnemy.Enemy;
			battleOrders.Add(newBattleOrder);
		}
	}

	//battleOrder정렬
	private void BattleOrderSort()
	{
		battleOrders.Sort(delegate(BattleOrder a, BattleOrder b)
		{
			return a.ahility > b.ahility ? -1 : 1;
		});
	}

	//public void CollectActions(HandleTrun input)
	//{
	//	performList.Add(input);
	//}

	//UI에 Enemy버튼과 Text 띄우기
	public void EnemyButtons()
	{
		//전체 삭제
		foreach(GameObject enemyBtn in enemyBtns)
		{
			Destroy(enemyBtn);
		}
		enemyBtns.Clear();

		//버튼 생성
		foreach (GameObject enemy in enemyInBattle)
		{
			GameObject newButton = Instantiate(enemyButton);
			EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
			EnemyStateMaschine cur_enemy = enemy.GetComponent<EnemyStateMaschine>();

			Text buttonText = newButton.transform.Find("Text").GetComponent<Text>();
			buttonText.text = cur_enemy.name;

			button.enemyPrefab = enemy;

			newButton.transform.SetParent(spacer,false);
			enemyBtns.Add(newButton);
		}
	}

	public void Input1()	//attack 버튼
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";
		//heroChoise.ahility = heroToManger[0].GetComponent<HeroStateMaschine>().hero.ahility;
		heroChoise.choosenAttack = heroToManger[0].GetComponent<HeroStateMaschine>().hero.attacks[0];
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
		//Debug.Log("HeroInputDone 실행");
		performList.Add(heroChoise);
		ClearAttackPanel();

		heroToManger[0].transform.Find("Selector").gameObject.SetActive(false);
		GameObject tmpHero = heroToManger[0];
		heroToManger.RemoveAt(0);
		heroToManger.Add(tmpHero);
		heroInput = HeroGUI.Activate;
	}
	
	//공격버튼 삭제
	private void ClearAttackPanel()
	{
		enemySelectPanel.SetActive(false);
		attackPanel.SetActive(false);
		magicPanel.SetActive(false);

		foreach (GameObject atkBtn in atkBtns)
		{
			Destroy(atkBtn);        //???
		}
		atkBtns.Clear();
	}

	//어택 버튼 만들기
	private void CreateAttackButtons()
	{
		GameObject attackButton = Instantiate(actionButton);
		Text attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		attackButtonText.text = "Attack";
		attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());	//버튼을 누르면 Input1 실행
		attackButton.transform.SetParent(actionSpacer, false);
		atkBtns.Add(attackButton);

		GameObject magicAttackButton = Instantiate(actionButton);
		Text magicAttackButtonText = magicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		magicAttackButtonText.text = "Magic";
		magicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());   //버튼을 누르면 Input1 실행
		magicAttackButton.transform.SetParent(actionSpacer, false);
		atkBtns.Add(magicAttackButton);

		if(heroToManger[0].GetComponent<HeroStateMaschine>().hero.magicAttacks.Count > 0)
		{
			foreach(BaseAttack magicAtk in heroToManger[0].GetComponent<HeroStateMaschine>().hero.magicAttacks)
			{
				GameObject _magicButton = Instantiate(magicButton);
				Text _magicButtonText = _magicButton.transform.Find("Text").gameObject.GetComponent<Text>();
				_magicButtonText.text = magicAtk.attackName;

				AttackButton ATB = magicButton.GetComponent<AttackButton>();
				ATB.magicAttackToPerform = magicAtk;
				_magicButton.transform.SetParent(magicSpacer, false);
				atkBtns.Add(_magicButton);
			}
		}
		else
		{
			magicAttackButton.GetComponent<Button>().interactable = false;
		}
	}
	public void Input4(BaseAttack chooosenMagic)//마법 공격
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";

		heroChoise.choosenAttack = chooosenMagic;
		magicPanel.SetActive(false);
		enemySelectPanel.SetActive(true);
	}

	public void Input3()//마법공격으로 변경
	{
		attackPanel.SetActive(false);
		magicPanel.SetActive(true);
	}
}
