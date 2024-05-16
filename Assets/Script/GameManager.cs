using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[System.Serializable]
	public class RegionData
	{
		public string regionName;
		public int maxAmountEnemys = 4;
		public string battleScene;
		public List<GameObject> possibleEnemys = new List<GameObject>();
	}

	public int curRegions;

	public List<RegionData> regions = new List<RegionData>();

	private bool gotAttack = false;
	[HideInInspector] public bool reStart = false;

	public string sceneToLoad;
	public string lastScene;
	//적 리스트
	public List<GameObject> enemyList = new List<GameObject>();
	//죽은 적 리스트
	public List<string> deadEnemyList = new List<string>();
	//플래이어 좌표
	public Vector3 lastHeroPosition;    
	//플래이어 좌표
	public Vector3 reStartHeroPosition;
	//싸우고 있는 적
	public List<string> battleEnemy = new List<string>();
	//파티 인원 최대 4명
	public GameObject[] heroParty = new GameObject[4];
	public enum GameState
	{
		World_State,
		Town_State,
		Battle_State,
		Idle
	}
	public GameState gamestate;

	public List<GameObject> enemyToBattle = new List<GameObject>();
	public int enemyAmount = 0;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(this.gameObject);
			Debug.Log("GameManager가 중복되서 삭제되었습니다");
		}
		DontDestroyOnLoad(this.gameObject);
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
	}
	private void Start()
	{
		reStartHeroPosition = GameObject.Find("Player").gameObject.transform.position;
		PlayerPrefs.DeleteAll();
	}
	private void Update()
	{
		switch (gamestate)
		{
			case GameState.World_State:
				if (gotAttack)
				{
					gamestate = GameState.Battle_State;
				}
				break;
			case GameState.Town_State: 
				break;
			case GameState.Battle_State:
				//배틀 씬 로드
				StartBattle();
				gamestate = GameState.Idle;
				break;
			case GameState.Idle: 
				break;
			default:
			break;
		}
	}

	private void StartBattle()
	{
		//적의 수
		enemyAmount = Random.Range(1, regions[0].maxAmountEnemys + 1);
		//enemyAmount = Random.Range(4, 5);	//디거깅 용
		//어떤 적을 내보낼지
		for (int i = 0; i < enemyAmount; i++)
		{
			enemyToBattle.Add(regions[curRegions].possibleEnemys[Random.Range(0, regions[curRegions].possibleEnemys.Count)]);
		}
		lastHeroPosition = GameObject.Find("Player").gameObject.transform.position;
		//nextHreoPosition = lastHeroPosition;
		lastScene = SceneManager.GetActiveScene().name;
		//로드 레벨
		SceneManager.LoadScene(regions[curRegions].battleScene);
		//영웅 초기화
		gotAttack = false;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	//Enemy찾아서 List에 저장(PlayerManager에서 사용)
	public void FindEnemy()
	{
		enemyList.Clear();
		GameObject[] tmpEnemys = GameObject.FindGameObjectsWithTag("Enemy");
		enemyList.AddRange(tmpEnemys);
	}
	//죽은 Enemy 찾아서 비활성화(PlayerManager에서 사용)
	public void OffEnemy()
	{
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (deadEnemyList.Contains(enemyList[i].name))	//죽는 Enemy랑 이름이 겹치는지 비교
			{
				enemyList[i].SetActive(false);
			}
		}
	}
	public void DeadEnemyListAdd(string _name)
	{
		deadEnemyList.Add(_name);
	}
	public void LoadNextScene()
	{
		SceneManager.LoadScene(sceneToLoad);
	}

	public void LoadSceneAfterBattle()
	{
		SceneManager.LoadScene(lastScene);
	}
	//Player파티 Hp저장
	public void PlayerHpSave(string _name, float _curHp, float _curMp)
	{
		PlayerPrefs.SetFloat($"{_name}_hp", _curHp);
		PlayerPrefs.SetFloat($"{_name}_mp", _curMp);
	}
	//Player파티 Hp로드
	public void PlayerHpLoad(string _name, out float _curHp, out float _curMp)
	{
		_curHp = PlayerPrefs.GetFloat($"{_name}_hp");
		_curMp = PlayerPrefs.GetFloat($"{_name}_mp");
		Debug.Log("로드");
	}
}
