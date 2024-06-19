using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	[HideInInspector] public bool gotAttack = false;
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
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(this.gameObject);
			Debug.Log("GameManager가 중복되서 삭제되었습니다");
		}
		DontDestroyOnLoad(this.gameObject);
	}
	private void Start()
	{
		reStartHeroPosition = GameObject.Find("Player").gameObject.transform.position;
		PlayerPrefs.DeleteAll();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	private void Update()
	{
		Debug.Log(gamestate + " gamestate");
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
	//배틀 시작
	private void StartBattle()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		//적의 수
		enemyAmount = Random.Range(1, regions[0].maxAmountEnemys + 1);
		//enemyAmount = Random.Range(4, 5);	//디거깅 용
		//내보낼 적을 리스트에 추가
		for (int i = 0; i < enemyAmount; i++)
		{
			enemyToBattle.Add(regions[curRegions].possibleEnemys[Random.Range(0, regions[curRegions].possibleEnemys.Count)]);
		}
		//지금 플레이어 position 저장
		lastHeroPosition = GameObject.Find("Player").gameObject.transform.position;
		//지금 있는 씬 저장
		lastScene = SceneManager.GetActiveScene().name;
		//맵에 맞는 배틀 씬 로드
		SceneManager.LoadScene(regions[curRegions].battleScene);
		//영웅 초기화
		gotAttack = false;
	}
	//Enemy찾아서 List에 저장
	public void FindEnemy()
	{
		enemyList.Clear();
		GameObject[] tmpEnemys = GameObject.FindGameObjectsWithTag("Enemy");
		enemyList.AddRange(tmpEnemys);
	}
	//죽은 Enemy 찾아서 비활성화
	public void OffEnemy()
	{
		for (int i = 0; i < enemyList.Count; i++)
		{
			//죽는 Enemy랑 이름이 겹치는지 비교
			if (deadEnemyList.Contains(enemyList[i].name))
			{
				enemyList[i].SetActive(false);
			}
		}
	}
	//죽은 enemy 저장
	public void DeadEnemyListAdd(string _name)
	{
		deadEnemyList.Add(_name);
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene(sceneToLoad);
	}
	//배틀 끝난 뒤 원래 있던 씬으로 이동
	public void LoadSceneAfterBattle()
	{
		SceneManager.LoadScene(lastScene);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
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
	//게임 재시작
	public void GameReStart()
	{
		LoadSceneAfterBattle();
		enemyList.Clear();
		deadEnemyList.Clear();
		battleEnemy.Clear();
		reStart = true;
		gotAttack = false;
		gamestate = GameManager.GameState.World_State;
		enemyToBattle.Clear();
		curRegions = 0;
	}
}
