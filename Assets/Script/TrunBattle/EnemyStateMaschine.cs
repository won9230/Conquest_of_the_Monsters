using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	public Slider hpBarSlider;

	private float cur_cooldown = 0f;
	private float max_cooldown = 5f;

	private Vector3 startPosition;

	private bool actionStarted = false;
	public GameObject heroToAttack;
	private float animSpeed = 5f;

	private void Start()
	{
		currentState = TurnState.Processing;
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		startPosition = transform.position;
	}

	private void Update()
	{
		Debug.Log(currentState);
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
				break;
			default:
				break;
		}


	}
	//행동 가능 체크
	private void UpgradeProgressBar()
	{
		cur_cooldown = cur_cooldown + Time.deltaTime;
		hpBarSlider.value = cur_cooldown / max_cooldown;

		if (cur_cooldown >= max_cooldown)
		{
			currentState = TurnState.ChooseAction;
		}
	}

	
	private void ChooseAction()
	{
		HandleTrun myAttack = new HandleTrun();
		myAttack.attacker = enemy.name;
		myAttack.Type = "Enemy";
		myAttack.attackersGamgeObject = this.gameObject;
		myAttack.attackersTarget = BSM.heroInBattle[Random.Range(0, BSM.heroInBattle.Count)];
		BSM.CollectActions(myAttack);
	}

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

	private bool MoveTowardsEnemy(Vector3 target)
	{
		return target != (transform.position = Vector3.MoveTowards(transform.position,target,animSpeed * Time.deltaTime));
	}
	private bool MoveTowardsStart(Vector3 target)
	{
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}
}
