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
		//spaer찾기
		heroPanelSpacer = GameObject.Find("Canvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
		//패널 만들기
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
					//태그 바꾸기
					this.gameObject.tag = "DeadHero";
					//적에게 타겟 안당하기
					BSM.heroInBattle.Remove(this.gameObject);
					//선택 안됨
					BSM.heroToManger.Remove(this.gameObject);
					//셀렉터 비활성화
					select.gameObject.SetActive(false);
					//ui 리셋
					BSM.attackPanel.SetActive(false);
					BSM.enemySelectPanel.SetActive(false);
					//performlist에서 삭제
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

					//색 변경(컷씬으로 대체)
					Debug.Log(this.gameObject.name + " Dead");
					//히어로 입력 리셋
					BSM.battleState = BattleStateMaschine.PerformAction.checkAlive;
					alive = false;
				}
				break;
			default:
				break;
		}

		
	}
	//행동 게이지 쿨타임
	private void UpgradeProgressBar()
	{
		if (BSM.battleOrders[0].attackerName == this.name)
		{
			//Debug.Log("히어로 공격 실행 " + this.name);
			currentState = TurnState.Addtolist;
		}
	}

	//적 공격
	private IEnumerator TimeForAction()
	{
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;
		//영웅 근처에서 공격 애니메이션
		Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z - 1.5f);

		while (MoveTowardsEnemy(enemyPosition))
		{
			yield return null;
		}

		//대기
		yield return new WaitForSeconds(0.5f);
		//데미지
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
		if (BSM.battleState != BattleStateMaschine.PerformAction.Win && BSM.battleState != BattleStateMaschine.PerformAction.Lose)
		{
			BSM.battleState = BattleStateMaschine.PerformAction.Wait;

			//적 상태 초기화
			cur_cooldown = 0f;
			currentState = TurnState.Processing;
		}
		else
		{
			currentState = TurnState.Waiting;
		}


		//코루틴 종료
		actionStarted = false;
	}

	//플레이어가 적에게 이동
	private bool MoveTowardsEnemy(Vector3 target)
	{
		//성공시 true
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}
	//플레이어가 자기 자리로 이동
	private bool MoveTowardsStart(Vector3 target)
	{
		//성공시 true
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}

	//영웅 데미지
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

	//데미지
	private void DoDamage()
	{
		float calc_damage = hero.curATK + BSM.performList[0].choosenAttack.attackDamage;
		enemyToAttack.GetComponent<EnemyStateMaschine>().TakeDamage(calc_damage);
	}

	//영웅 패널 만들기
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
	//스텟 업데이트
	private void UpdateHeroPanel()
	{
		stats.heroHp.text = "HP: " + hero.curHp;
		stats.heroMp.text = "MP: " + hero.curMp;

	}
}
