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
	public GameObject select;

	private float cur_cooldown = 0f;
	private float max_cooldown = 5f;

	public GameObject enemyToAttack;
	private bool actionStarted = false;
	private Vector3 startPosition;
	private float animSpeed = 10f;
	//dead
	private bool alive = true;
	//heroPanel
	private HeroPanelStats stats;
	public GameObject heroPanel;
	private Transform heroPanelSpacer;

	private void Awake()
	{
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		BSM.heroToManger.Add(this.gameObject);
	}
	private void Start()
	{
		//spaerã��
		heroPanelSpacer = GameObject.Find("Canvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
		//�г� �����
		CreateHeroPanel();


		cur_cooldown = Random.Range(1, 2.5f);
		select.gameObject.SetActive(false);

		currentState = TurnState.Processing;
		startPosition = this.transform.position;
	}

	private void Update()
	{
		//Debug.Log(this.name + " " + currentState);
		switch (currentState)
		{
			case TurnState.Processing:
				UpgradeProgressBar();
				break;
			case TurnState.Addtolist:

				currentState = TurnState.Waiting;
				break;
			case TurnState.Waiting:
				break;
			case TurnState.Selecting:
				break;
			case TurnState.Action:
				StartCoroutine(TimeForAction());
				break;
			case TurnState.Dead:
				if (!alive)
				{
					return;
				}
				else
				{
					//�±� �ٲٱ�
					this.gameObject.tag = "DeadHero";
					//������ Ÿ�� �ȴ��ϱ�
					BSM.heroInBattle.Remove(this.gameObject);
					//���� �ȵ�
					BSM.heroToManger.Remove(this.gameObject);
					//������ ��Ȱ��ȭ
					select.gameObject.SetActive(false);
					//ui ����
					BSM.attackPanel.SetActive(false);
					BSM.enemySelectPanel.SetActive(false);
					//performlist���� ����
					if(BSM.heroInBattle.Count > 0)
					{
						for (int i = 0; i < BSM.performList.Count; i++)
						{
							if(BSM.performList[i].attackersGamgeObject == this.gameObject)
							{
								BSM.performList.Remove(BSM.performList[i]);
							}
							if(BSM.performList[i].attackersTarget == this.gameObject)
							{
								BSM.performList[i].attackersTarget = BSM.heroInBattle[Random.Range(0, BSM.heroInBattle.Count)];
							}
						}
					}
					//BSM.heroToManger.RemoveAt(0);

					//�� ����(�ƾ����� ��ü)
					Debug.Log(this.gameObject.name + " Dead");
					//����� �Է� ����
					BSM.battleState = BattleStateMaschine.PerformAction.checkAlive;
					alive = false;
				}
				break;
			default:
				break;
		}

		
	}
	//�ൿ ������ ��Ÿ��
	private void UpgradeProgressBar()
	{
		if (BSM.battleOrders[0].attackerName == this.name)
		{
			//Debug.Log("����� ���� ���� " + this.name);
			currentState = TurnState.Addtolist;
		}
	}

	//�� ����
	private IEnumerator TimeForAction()
	{
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;
		//���� ��ó���� ���� �ִϸ��̼�
		Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z - 1.5f);

		while (MoveTowardsEnemy(enemyPosition))
		{
			yield return null;
		}

		//���
		yield return new WaitForSeconds(0.5f);
		//������
		DoDamage();
		//������ġ�� ����
		Vector3 firstPosition = startPosition;
		while (MoveTowardsStart(firstPosition))
		{
			yield return null;
		}
		//BSM���� performer����
		BSM.performList.RemoveAt(0);
		//BSM�� Wait���� ����
		if (BSM.battleState != BattleStateMaschine.PerformAction.Win && BSM.battleState != BattleStateMaschine.PerformAction.Lose)
		{
			BSM.battleState = BattleStateMaschine.PerformAction.Wait;

			//�� ���� �ʱ�ȭ
			cur_cooldown = 0f;
			currentState = TurnState.Processing;
		}
		else
		{
			currentState = TurnState.Waiting;
		}


		//�ڷ�ƾ ����
		actionStarted = false;
	}

	//�÷��̾ ������ �̵�
	private bool MoveTowardsEnemy(Vector3 target)
	{
		//������ true
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}
	//�÷��̾ �ڱ� �ڸ��� �̵�
	private bool MoveTowardsStart(Vector3 target)
	{
		//������ true
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}

	//���� ������
	public void TakeDamage(float getDamageAmount)
	{
		hero.curHp -= getDamageAmount;
		if (hero.curHp <= 0)
		{
			hero.curHp = 0;
			currentState = TurnState.Dead;
		}
		UpdateHeroPanel();
	}

	//������
	private void DoDamage()
	{
		float calc_damage = hero.curATK + BSM.performList[0].choosenAttack.attackDamage;
		enemyToAttack.GetComponent<EnemyStateMaschine>().TakeDamage(calc_damage);
	}

	//���� �г� �����
	private void CreateHeroPanel()
	{
		heroPanel = Instantiate(heroPanel);
		stats = heroPanel.GetComponent<HeroPanelStats>();
		stats.heroName.text = hero.theName;
		stats.heroHp.text = "HP: " + hero.curHp;
		stats.heroMp.text = "MP: " + hero.curMp;
		hpBarSlider = stats.progressBar;

		heroPanel.transform.SetParent(heroPanelSpacer, false);
		
	}
	//���� ������Ʈ
	private void UpdateHeroPanel()
	{
		stats.heroHp.text = "HP: " + hero.curHp;
		stats.heroMp.text = "MP: " + hero.curMp;

	}
}
