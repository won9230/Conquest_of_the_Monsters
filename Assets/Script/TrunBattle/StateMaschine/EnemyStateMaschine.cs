using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BattleStateMaschine;

public class EnemyStateMaschine : MonoBehaviour
{
	public BaseEnemy enemy;
	[HideInInspector] BattleStateMaschine BSM;
	[HideInInspector] public AnimatorManager anim;
	//hp��
	public Slider enemyHpBarSlider;

	public enum TurnState
	{
		Processing,
		ChooseAction,
		Waiting,
		Action,
		Dead
	}

	public TurnState currentState;
	
	private Vector3 startPosition;

	private bool actionStarted = false;
	private float animSpeed = 10f;
	public GameObject heroToAttack;
	public GameObject select;
	public GameObject hpBar;

	//��������
	private bool alive = true;

	private void Awake()
	{
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		anim = GetComponent<AnimatorManager>();
		GameObject newhpBar = Instantiate(hpBar,transform.position,transform.rotation);
		Transform enemyHpBar = GameObject.Find("Canvas").transform.Find("EnemyHpBars");
		enemyHpBar.parent = newhpBar.transform;
		enemyHpBarSlider = newhpBar.GetComponent<Slider>();
	}

	private void Start()
	{
		currentState = TurnState.Processing;
		select.SetActive(false);

		startPosition = this.transform.position;
		if (BSM == null)
			Debug.LogError("BSM�� �����ϴ�.");
		if (anim == null)
			Debug.LogError("AnimatorManager�� �����ϴ�");
		if(enemyHpBarSlider == null)
			Debug.LogError("enemyHpBarSlider �����ϴ�");

	}

	private void Update()
	{
		//Debug.Log(this.name + " " + currentState);
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
				StartCoroutine(TimeForAction());
				break;
			case TurnState.Dead:
				if (!alive)
				{
					return;
				}
				else
				{
					//enemy�� �±� ����
					this.gameObject.tag = "DeadEnemy";
					//������ �������� ����
					Debug.Log("1");
					BSM.enemyInBattle.Remove(this.gameObject);
					Debug.Log("2");

					//selector ��Ȱ��ȭ
					select.SetActive(false);

					RemoveAttackersTarget();
					RemoveBattleOrder();

					//�ִϸ��̼� ���
					Debug.Log(this.gameObject.name + " Dead!");

					alive = false;
					//�� ���� ��ư �ʱ�ȭ
					BSM.EnemyButtons();


					//���� üũ
					BSM.battleState = PerformAction.checkAlive;
				}
				break;
			default:
				break;
		}


	}

	//�ൿ ���� üũ
	private void UpgradeProgressBar()
	{
		if (BSM.battleOrders[0].attackerName == this.name && !BSM.isEnemyAttack)
		{
			//Debug.Log("������ ���� " + this.name);
			//        foreach (var item in BSM.battleOrders)
			//        {
			//Debug.Log("������ ���� " + item.attackerName);
			//        }
			BSM.isEnemyAttack = true;
            currentState = TurnState.ChooseAction;
			StartCoroutine(WaitTime(0.1f));
		}
	}

	//������ ������Ʈ�� ���Ѵ�.
	private void ChooseAction()
	{
		HandleTrun myAttack = new HandleTrun();
		myAttack.attacker = this.name;
		myAttack.Type = "Enemy";
		myAttack.attackersGamgeObject = this.gameObject;
		myAttack.attackersTarget = BSM.heroInBattle[Random.Range(0, BSM.heroInBattle.Count)];

		int num = Random.Range(0, enemy.attacks.Count);
		myAttack.choosenAttack = enemy.attacks[num];
		//Debug.Log(this.gameObject.name + " has choosen " + myAttack.choosenAttack.attackName + " and do " + myAttack.choosenAttack.attackDamage + " damage!");

		BSM.perform = myAttack;
    }
	
	//���� ����
	private IEnumerator TimeForAction()
	{
		BSM.UIPanel.SetActive(false);
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;
		//���� ��ó���� ���� �ִϸ��̼�
		Vector3 heroPosition = new Vector3(heroToAttack.transform.position.x, heroToAttack.transform.position.y,heroToAttack.transform.position.z + 1.5f);
		//�̵� �ִϸ��̼�
		anim.RunAnim(true);
		while (MoveTowardsEnemy(heroPosition))
		{
			yield return null;
		}
		//���� �ִϸ��̼�
		anim.AttackAnim(true);
		//���
		//yield return new WaitForSeconds(0.005f);
		yield return new WaitForSeconds(anim.GetAnimTime());

		DoDamage();
		//������ġ�� ����
		//����� 
		anim.AttackAnim(false);
		while (MoveTowardsStart(startPosition))
		{
			yield return null;
		}
		anim.RunAnim(false);
		//BSM���� performer����
		BSM.perform = new HandleTrun();
		//BSM�� Wait���� ����
		BSM.battleState = PerformAction.Wait;
		actionStarted = false;
		//�� ���� �ʱ�ȭ
		//cur_cooldown = 0f;
		BSM.BattleNext();
		BSM.isEnemyAttack = false;

		currentState = TurnState.Processing;
	}

	//�÷��̾ ������ �̵�
	private bool MoveTowardsEnemy(Vector3 target)
	{
		//������ true
		return target != (transform.position = Vector3.MoveTowards(transform.position,target,animSpeed * Time.deltaTime));
	}
	//�÷��̾ �ڱ� �ڸ��� �̵�
	private bool MoveTowardsStart(Vector3 target)
	{
		//������ true
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}

	//������ ����
	private void DoDamage()
	{
		float calc_damage = enemy.curATK + BSM.perform.choosenAttack.attackDamage;
		heroToAttack.GetComponent<HeroStateMaschine>().TakeDamage(calc_damage);
		
	}

	//������ ����
	public void TakeDamage(float getDamageAmount)
	{
		enemy.curHp -= getDamageAmount;
		if(enemy.curHp <= 0)
		{
			enemyHpBarSlider.value = enemy.curHp;
			anim.DieAnim(true);
			enemy.curHp = 0;
			currentState = TurnState.Dead;
		}
		else
		{
			enemyHpBarSlider.value = enemy.curHp;
			//Debug.Log("�� ������");
			anim.TakeDamageAnim();
		}

	}

	//�̸� �� �ִ� ������ ����
	private void RemoveAttackersTarget()
	{
		if (BSM.enemyInBattle.Count > 0)
		{
			if (BSM.perform.attackersGamgeObject == this.gameObject)
			{
				BSM.perform = null;
			}
			if (BSM.perform.attackersTarget == this.gameObject)
			{
				BSM.perform.attackersTarget = BSM.enemyInBattle[Random.Range(0, BSM.enemyInBattle.Count)];

			}
		}

	}

	private void RemoveBattleOrder()
	{
		if (BSM.battleOrders.Count > 0)
		{
			for (int i = 0; i < BSM.battleOrders.Count; i++)
			{
				if (BSM.battleOrders[i].attackerName == this.gameObject.name)
				{
					BSM.battleOrders.Remove(BSM.battleOrders[i]);
					i--;
				}
			}
		}
	}

	private IEnumerator WaitTime(float _time)
	{
		yield return new WaitForSeconds(_time);
	}
}
