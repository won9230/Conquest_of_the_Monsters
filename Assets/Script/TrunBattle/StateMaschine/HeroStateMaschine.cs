using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeroStateMaschine : MonoBehaviour
{
	private BattleStateMaschine BSM;
	public AnimatorManager anim;
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

	//private float cur_cooldown = 0f;
	//private float max_cooldown = 5f;

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
		heroPanelSpacer = GameObject.Find("Canvas").transform.Find("UIPanel").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
		if (BSM == null)
			Debug.LogError("BattleManager�� �����ϴ�");
		if (BSM == null)
			Debug.LogError("heroPanelSpacer�� �����ϴ�");
	}
	private void Start()
	{
		//�г� �����
		CreateHeroPanel();
		
		select.gameObject.SetActive(false);
		anim = GetComponent<AnimatorManager>();
		if (anim == null)
			Debug.LogError("AnimatorManager�� �����ϴ�");

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
		if (BSM.battleOrders[0].attackerName == this.name && !BSM.isHeroAttack)
		{
			BSM.UIPanel.SetActive(true);
			//Debug.Log("����� ���� ���� " + this.name);
			BSM.isHeroAttack = true;
			currentState = TurnState.Addtolist;
		}
	}

	//�� ����
	private IEnumerator TimeForAction()
	{
		BSM.UIPanel.SetActive(false);
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;
		//���� ��ó���� ���� �ִϸ��̼�
		Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z - 1.5f);
		//�޸��� �ִϸ��̼� ����
		anim.RunAnim(true);
		//��ǥ ���� ����
		while (MoveTowardsEnemy(enemyPosition))
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.1f);
		//���� �ִϸ��̼�
		anim.AttackAnim(true);
		yield return new WaitForSeconds(0.005f);
		yield return new WaitForSeconds(anim.GetAnimTime());
		//������
		DoDamage();
		//������ġ�� ����
		anim.AttackAnim(false);
		while (MoveTowardsStart(startPosition))
		{
			yield return null;
		}
		anim.RunAnim(false);
		//BSM���� performer����
		BSM.perform = new HandleTrun();
		//���� ���� üũ
		if (BSM.battleState != BattleStateMaschine.PerformAction.Win && BSM.battleState != BattleStateMaschine.PerformAction.Lose)
		{
			BSM.battleState = BattleStateMaschine.PerformAction.Wait;
			//�� ���� �ʱ�ȭ
			currentState = TurnState.Processing;
			BSM.BattleNext();   //battleorder��������
		}
		else
		{
			//���� ����
			currentState = TurnState.Waiting;
		}

		//���� ��
		BSM.isHeroAttack = false;
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

	//���� ������ ����
	public void TakeDamage(float getDamageAmount)
	{
		hero.curHp -= getDamageAmount;

		if (hero.curHp <= 0)
		{
			//���� �ִϸ��̼�
			anim.DieAnim(true);
			hero.curHp = 0;
			currentState = TurnState.Dead;
		}
		else
		{

			anim.TakeDamageAnim();
		}
		UpdateHeroPanel();
	}

	//������
	private void DoDamage()
	{
		float calc_damage = hero.curATK + BSM.perform.choosenAttack.attackDamage;
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
