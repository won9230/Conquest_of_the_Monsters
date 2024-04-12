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

	//public List<HandleTrun> performList = new List<HandleTrun>();
	public HandleTrun perform;
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

	public Transform spacer;			

	public GameObject attackPanel;		//attack ��ư
	public GameObject enemySelectPanel;	//�� ���� ��ư
	public GameObject magicPanel;		//magic��ư
	public GameObject UIPanel;          //UI �г� ��ä(Canve)
	public GameObject hpBarPrefab;      //�� �Ӹ� �� hp
	public Vector3 hpBarOffset = new Vector3(-0.5f, 2.4f, 0);	//�� hp offset

	public GameObject enemyButton;
	public Transform actionSpacer;		//action �θ�
	public Transform magicSpacer;		//magic��ư �θ�
	public GameObject actionButton;
	public GameObject magicButton;
	private List<GameObject> atkBtns = new List<GameObject>();

	private List<GameObject> enemyBtns = new List<GameObject>();

	public List<Transform> enemySpawnPoints = new List<Transform>();
	public List<Transform> heroSpawnPoints = new List<Transform>();

	[HideInInspector] public bool isEnemyAttack = false;	//�� ���� ���� ����
	[HideInInspector] public bool isHeroAttack = false;	//����� ���� ���� ����
	private void Awake()
	{
		for (int i = 0; i < GameManager.instance.enemyAmount; i++)
		{
			GameObject newEnemy = Instantiate(GameManager.instance.enemyToBattle[i], enemySpawnPoints[i].position,Quaternion.Euler(new Vector3(0,180,0)));
			GameObject newEnemyHpbar = Instantiate(hpBarPrefab, newEnemy.transform.position, newEnemy.transform.rotation);
			newEnemyHpbar.transform.parent = newEnemy.transform;

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

		perform = new HandleTrun();

		//���� ���ϱ�
		HeroOrderAdd();
		EnemyOrderAdd();
		BattleOrderSort();

		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(false);
		magicPanel.SetActive(false);
		UIPanel.SetActive(false) ;

		EnemyButtons();
	}

	private void Update()
	{
		//Debug.Log("GameState " + battleState);
		switch (battleState)
		{
			case PerformAction.Wait:
				//���
				if(perform.attacker != null)
				{
					battleState = PerformAction.TakeAction;
				}
				break;
			case PerformAction.TakeAction:
				//���� ����
				GameObject performer = GameObject.Find(perform.attacker);
				if (perform.Type == "Enemy")
				{
					EnemyStateMaschine ESM = performer.GetComponent<EnemyStateMaschine>();
					for (int i = 0; i < heroInBattle.Count; i++)
					{
						if(perform.attackersTarget == heroInBattle[i])
						{
							ESM.heroToAttack = perform.attackersTarget;
							ESM.currentState = EnemyStateMaschine.TurnState.Action;
							break;
						}
						else
						{
							perform.attackersTarget = heroInBattle[Random.Range(0,heroInBattle.Count)];
							ESM.heroToAttack = perform.attackersTarget;
							ESM.currentState = EnemyStateMaschine.TurnState.Action;
						}
					}
				}
				if(perform.Type == "Hero")
				{
					HeroStateMaschine HSM = performer.GetComponent<HeroStateMaschine>();
					HSM.enemyToAttack = perform.attackersTarget;
					HSM.currentState = HeroStateMaschine.TurnState.Action;
					heroInput = HeroGUI.Activate;
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
					Debug.Log("��Ʋ �й�");
					for (int i = 0; i < heroInBattle.Count; i++)
					{
						heroInBattle[i].GetComponent<HeroStateMaschine>().currentState = HeroStateMaschine.TurnState.Waiting;
					}
				}
				else if(enemyInBattle.Count < 1)
				{
					battleState = PerformAction.Win;
					Debug.Log("��Ʋ �¸�");
				}
				break;
			case PerformAction.Win:
				Debug.Log("�¸�");
				for (int i = 0; i < heroInBattle.Count; i++)
				{
					heroInBattle[i].GetComponent<HeroStateMaschine>().currentState = HeroStateMaschine.TurnState.Waiting;
				}


				GameManager.instance.LoadSceneAfterBattle();
				GameManager.instance.gamestate = GameManager.GameState.World_State;
				GameManager.instance.enemyToBattle.Clear();
				break;
			case PerformAction.Lose:
				Debug.Log("�й�");
				break;
			default:
				break;
		}
		//Debug.Log("heroInput : " + heroInput);
		switch (heroInput)
		{
			case HeroGUI.Activate:
				if(heroToManger.Count > 0)
				{
					heroToManger[0].transform.Find("Selector").gameObject.SetActive(true);
					heroChoise = new HandleTrun();

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

	//����� battleOrder�� �߰�
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

	//�� battleOrder�� �߰�
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

	//battleOrder����
	private void BattleOrderSort()
	{
		battleOrders.Sort(delegate(BattleOrder a, BattleOrder b)
		{
			return a.ahility > b.ahility ? -1 : 1;
		});
	}

	public void BattleNext()
	{
		BattleOrder tmpbattleOrder = battleOrders[0];
		battleOrders.RemoveAt(0);
		battleOrders.Add(tmpbattleOrder);
	}

	//UI�� Enemy��ư�� Text ����
	public void EnemyButtons()
	{
		//��ü ����
		foreach(GameObject enemyBtn in enemyBtns)
		{
			Destroy(enemyBtn);
		}
		enemyBtns.Clear();

		//��ư ����
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



	private void HeroInputDone()
	{
		heroInput = HeroGUI.Waiting;
		perform = heroChoise;
		ClearAttackPanel();

		heroToManger[0].transform.Find("Selector").gameObject.SetActive(false);
		GameObject tmpHero = heroToManger[0];
		heroToManger.RemoveAt(0);
		heroToManger.Add(tmpHero);
	}
	
	//���ݹ�ư ����
	private void ClearAttackPanel()
	{
		enemySelectPanel.SetActive(false);
		attackPanel.SetActive(false);
		magicPanel.SetActive(false);

		foreach (GameObject atkBtn in atkBtns)
		{
			Destroy(atkBtn);
		}
		atkBtns.Clear();

		UIPanel.SetActive(false);
	}

	//���� ��ư �����
	private void CreateAttackButtons()
	{
		UIPanel.SetActive(true);
		GameObject attackButton = Instantiate(actionButton);
		Text attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		attackButtonText.text = "Attack";
		attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());	//��ư�� ������ Input1 ����
		attackButton.transform.SetParent(actionSpacer, false);
		atkBtns.Add(attackButton);

		GameObject magicAttackButton = Instantiate(actionButton);
		Text magicAttackButtonText = magicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		magicAttackButtonText.text = "Magic";
		magicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());   //��ư�� ������ Input3 ����
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

	public void Input1()    //attack ��ư
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";
		//heroChoise.ahility = heroToManger[0].GetComponent<HeroStateMaschine>().hero.ahility;
		heroChoise.choosenAttack = heroToManger[0].GetComponent<HeroStateMaschine>().hero.attacks[0];
		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(true);
	}

	public void Input2(GameObject choosenEnemy) //�� ����
	{
		heroChoise.attackersTarget = choosenEnemy;
		heroInput = HeroGUI.Done;
	}

	public void Input3()//������������ ����
	{
		attackPanel.SetActive(false);
		magicPanel.SetActive(true);
	}

	public void Input4(BaseAttack chooosenMagic)//���� ����
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";

		heroChoise.choosenAttack = chooosenMagic;
		magicPanel.SetActive(false);
		enemySelectPanel.SetActive(true);
	}


}
