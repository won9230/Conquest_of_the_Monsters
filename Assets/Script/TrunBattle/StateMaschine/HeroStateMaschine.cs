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
	public Slider hpBarSlider;
	public GameObject heroPanel;
	private Transform heroPanelSpacer;

	private void Awake()
	{
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
		BSM.heroToManger.Add(this.gameObject);
		heroPanelSpacer = GameObject.Find("Canvas").transform.Find("UIPanel").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
		anim = GetComponent<AnimatorManager>();
		if (BSM == null)
			Debug.LogError("BattleManager가 없습니다");
		if (BSM == null)
			Debug.LogError("heroPanelSpacer가 없습니다");
		if (anim == null)
			Debug.LogError("AnimatorManager가 없습니다");
	}
	private void Start()
	{
		//저장 불러오기
		if (PlayerPrefs.HasKey($"{hero.theName}_hp"))
			GameManager.instance.PlayerHpLoad(hero.theName, out hero.curHp, out hero.curMp);
		//패널 만들기
		CreateHeroPanel();
		
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
					BSM.deadToHero.Add(this.gameObject);
					//셀렉터 비활성화
					select.gameObject.SetActive(false);
					//ui 리셋
					//BSM.attackPanel.SetActive(false);
					//BSM.enemySelectPanel.SetActive(false);
					RemoveBattleOrder();

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
		if (BSM.battleOrders[0].attackerName == this.name && !BSM.isHeroAttack)
		{
			BSM.UIPanel.SetActive(true);
			//Debug.Log("히어로 공격 실행 " + this.name);
			BSM.isHeroAttack = true;
			currentState = TurnState.Addtolist;
		}
	}

	//적 공격
	private IEnumerator TimeForAction()
	{
		BSM.UIPanel.SetActive(false);
		enemyToAttack.GetComponent<EnemyStateMaschine>().enemyHpBar.SetActive(true);
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;
		//영웅 근처에서 공격 애니메이션
		Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z - 1.5f);
		//달리기 애니메이션 시작
		anim.RunAnim(true);
		//목표 지점 도착
		while (MoveTowardsEnemy(enemyPosition))
		{
			yield return null;
		}
		//공격 애니메이션
		anim.AttackAnim(true);
		yield return new WaitForSeconds(0.01f);
		yield return new WaitForSeconds(anim.GetAnimTime() + 0.1f);


		//데미지
		DoDamage();
		//원래위치로 복귀
		anim.AttackAnim(false);
		while (MoveTowardsStart(startPosition))
		{
			yield return null;
		}
		anim.RunAnim(false);
		//BSM에서 performer제거
		BSM.perform = new HandleTrun();
		//전투 종료 체크
		if (BSM.battleState != BattleStateMaschine.PerformAction.Win && BSM.battleState != BattleStateMaschine.PerformAction.Lose)
		{
			BSM.battleState = BattleStateMaschine.PerformAction.Wait;
			//상태 초기화
			currentState = TurnState.Processing;
			BSM.BattleNext();   //battleorder다음으로
		}
		else
		{
			//전투 종료
			currentState = TurnState.Waiting;
		}
		enemyToAttack.GetComponent<EnemyStateMaschine>().enemyHpBar.SetActive(false);
		//공격 끝
		BSM.isHeroAttack = false;
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

	//영웅 데미지 입음
	public void TakeDamage(float getDamageAmount)
	{
		hero.curHp -= getDamageAmount;
		
		if (hero.curHp <= 0)
		{
			//죽음 애니메이션
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

	//데미지
	private void DoDamage()
	{
		float calc_damage = hero.curATK + BSM.perform.choosenAttack.attackDamage;
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
		hpBarSlider.interactable = false;
		hpBarSlider.maxValue = hero.baseHp;
		hpBarSlider.minValue = 0;
		hpBarSlider.value = hero.curHp;

		heroPanel.transform.SetParent(heroPanelSpacer, false);
		
	}
	//스텟 업데이트
	private void UpdateHeroPanel()
	{
		stats.heroHp.text = "HP: " + hero.curHp;
		stats.heroMp.text = "MP: " + hero.curMp;
		hpBarSlider.value = hero.curHp;
	}
	//배틀순서삭제
	private void RemoveBattleOrder()
	{
		if (BSM.battleOrders.Count > 0)
		{
			for (int i = 0; i < BSM.battleOrders.Count; i++)
			{
				if (BSM.battleOrders[i].attackerName == this.gameObject.name)
				{
					BSM.battleOrders.Remove(BSM.battleOrders[i]);
					return;
				}
			}
		}
	}
}
