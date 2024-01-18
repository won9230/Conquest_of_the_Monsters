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
	public GameObject magicPanel;

	public Transform actionSpacer;
	public Transform magicSpacer;
	public GameObject actionButton;
	public GameObject magicButton;
	private List<GameObject> atkBtns = new List<GameObject>();

	private List<GameObject> enemyBtns = new List<GameObject>();

	public List<Transform> spawnPoints = new List<Transform>();
	private void Awake()
	{
		for (int i = 0; i < GameManager.instance.enemyAmount; i++)
		{
			GameObject newEnemy = Instantiate(GameManager.instance.enemyToBattle[i], spawnPoints[i].position,Quaternion.Euler(new Vector3(0,180,0)));
			newEnemy.name = newEnemy.GetComponent<EnemyStateMaschine>().enemy.theName + "_" + (i+1);
			newEnemy.GetComponent<EnemyStateMaschine>().enemy.theName = newEnemy.name;
			enemyInBattle.Add(newEnemy);
		}
	}

	private void Start()
	{
		battleState = PerformAction.Wait;
		//enemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
		heroInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
		heroInput = HeroGUI.Activate;

		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(false);
		magicPanel.SetActive(false);

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
				//idle
				break;
			case PerformAction.checkAlive:
				if(heroInBattle.Count < 1)
				{
					battleState = PerformAction.Lose;
					//��Ʋ ��
					Debug.Log("��Ʋ �й�");
					for (int i = 0; i < heroInBattle.Count; i++)
					{
						heroInBattle[i].GetComponent<HeroStateMaschine>().currentState = HeroStateMaschine.TurnState.Waiting;
					}
				}
				else if(enemyInBattle.Count < 1)
				{
					battleState = PerformAction.Win;
					//��Ʋ �̱�
					Debug.Log("��Ʋ �¸�");
				}
				else
				{
					ClearAttackPanel();
					heroInput = HeroGUI.Activate;
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
	

	public void CollectActions(HandleTrun input)
	{
		performList.Add(input);
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

	public void Input1()	//attack ��ư
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";
		heroChoise.choosenAttack = heroToManger[0].GetComponent<HeroStateMaschine>().hero.attacks[0];
		attackPanel.SetActive(false);
		enemySelectPanel.SetActive(true);
	}

	public void Input2(GameObject choosenEnemy)	//�� ����
	{
		heroChoise.attackersTarget = choosenEnemy;
		heroInput = HeroGUI.Done;
	}

	private void HeroInputDone()
	{
		performList.Add(heroChoise);
		ClearAttackPanel();

		heroToManger[0].transform.Find("Selector").gameObject.SetActive(false);
		heroToManger.RemoveAt(0);
		heroInput = HeroGUI.Activate;
	}

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

	private void CreateAttackButtons()
	{
		GameObject attackButton = Instantiate(actionButton);
		Text attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		attackButtonText.text = "Attack";
		attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
		attackButton.transform.SetParent(actionSpacer, false);
		atkBtns.Add(attackButton);

		GameObject magicAttackButton = Instantiate(actionButton);
		Text magicAttackButtonText = magicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		magicAttackButtonText.text = "Magic";
		magicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
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
	public void Input4(BaseAttack chooosenMagic)//���� ����
	{
		heroChoise.attacker = heroToManger[0].name;
		heroChoise.attackersGamgeObject = heroToManger[0];
		heroChoise.Type = "Hero";

		heroChoise.choosenAttack = chooosenMagic;
		magicPanel.SetActive(false);
		enemySelectPanel.SetActive(true);
	}

	public void Input3()//������������ ����
	{
		attackPanel.SetActive(false);
		magicPanel.SetActive(true);
	}
}
