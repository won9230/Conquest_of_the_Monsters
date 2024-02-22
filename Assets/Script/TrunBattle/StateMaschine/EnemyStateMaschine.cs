using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BattleStateMaschine;

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

	private float cur_cooldown = 0f;
	private float max_cooldown = 3f;	//10f

	private Vector3 startPosition;

	private bool actionStarted = false;
	public GameObject heroToAttack;
	private float animSpeed = 10f;
	public GameObject select;

	//alive
	private bool alive = true;

	private void Start()
	{
		currentState = TurnState.Processing;
		select.SetActive(false);
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		startPosition = transform.position;
	}

	private void Update()
	{
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
					//enemy로 태그 변경
					this.gameObject.tag = "DeadEnemy";
					//영웅을 공격하지 않음
					BSM.enemyInBattle.Remove(this.gameObject);
					//selector 비활성화
					select.SetActive(false);
					//미리 들어가 있는 공격을 삭제
					if(BSM.enemyInBattle.Count > 0)
					{
						for (int i = 0; i < BSM.performList.Count; i++)
						{
							if (BSM.performList[i].attackersGamgeObject == this.gameObject)
							{
								BSM.performList.Remove(BSM.performList[i]);
								i--;
							}
							if (BSM.performList[i].attackersTarget == this.gameObject)
							{
								BSM.performList[i].attackersTarget = BSM.enemyInBattle[Random.Range(0, BSM.enemyInBattle.Count)];
							}
						}
					}
					//애니메이션 재생
					Debug.Log(this.gameObject.name + " Dead!");

					alive = false;
					//적 선택 버튼 초기화
					BSM.EnemyButtons();
					//생존 체크
					BSM.battleState = BattleStateMaschine.PerformAction.checkAlive;
				}
				break;
			default:
				break;
		}


	}

	//행동 가능 체크
	private void UpgradeProgressBar()
	{
		if (BSM.battleOrders[0].attackerName == this.name)
		{
			//Debug.Log("적공격 실행 " + this.name);
			currentState = TurnState.ChooseAction;
			BattleOrder tmpbattleOrder = BSM.battleOrders[0];
            BSM.battleOrders.RemoveAt(0);
			BSM.battleOrders.Add(tmpbattleOrder);

		}
	}

	//공격할 오브젝트를 정한다.
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

		BSM.performList.Add(myAttack);
        foreach (HandleTrun item in BSM.performList)
        {
			Debug.Log("핸들턴 " + item.attackersGamgeObject.name);
        }
    }
	
	//공격 동작
	private IEnumerator TimeForAction()
	{
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;
		//영웅 근처에서 공격 애니메이션
		Vector3 heroPosition = new Vector3(heroToAttack.transform.position.x, heroToAttack.transform.position.y,heroToAttack.transform.position.z + 1.5f);

		while (MoveTowardsEnemy(heroPosition))
		{
			yield return null;
		}

		//대기
		yield return new WaitForSeconds(0.5f);
		//대미지
		DoDamage();
		//원래위치로 복귀
		Vector3 firstPosition = startPosition;
		while (MoveTowardsStart(firstPosition))
		{
			yield return null;
		}
		//BSM에서 performer제거
		BSM.performList.RemoveAt(0);
		//BSM를 Wait으로 변경
		BSM.battleState = BattleStateMaschine.PerformAction.Wait;

		actionStarted = false;
		//적 상태 초기화
		cur_cooldown = 0f;
		currentState = TurnState.Processing;
	}

	//플레이어가 적에게 이동
	private bool MoveTowardsEnemy(Vector3 target)
	{
		//성공시 true
		return target != (transform.position = Vector3.MoveTowards(transform.position,target,animSpeed * Time.deltaTime));
	}
	//플레이어가 자기 자리로 이동
	private bool MoveTowardsStart(Vector3 target)
	{
		//성공시 true
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}

	//데미지 입음
	private void DoDamage()
	{
		float calc_damage = enemy.curATK + BSM.performList[0].choosenAttack.attackDamage;
		heroToAttack.GetComponent<HeroStateMaschine>().TakeDamage(calc_damage);
	}

	//데미지 입힘
	public void TakeDamage(float getDamageAmount)
	{
		enemy.curHp -= getDamageAmount;
		if(enemy.curHp <= 0)
		{
			enemy.curHp = 0;
			currentState = TurnState.Dead;
		}
	}
}
