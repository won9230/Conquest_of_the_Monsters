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
	//�� ����Ʈ
	public List<GameObject> enemyList = new List<GameObject>();
	//���� �� ����Ʈ
	public List<string> deadEnemyList = new List<string>();
	//�÷��̾� ��ǥ
	public Vector3 lastHeroPosition;    
	//�÷��̾� ��ǥ
	public Vector3 reStartHeroPosition;
	//�ο�� �ִ� ��
	public List<string> battleEnemy = new List<string>();
	//��Ƽ �ο� �ִ� 4��
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
			Debug.Log("GameManager�� �ߺ��Ǽ� �����Ǿ����ϴ�");
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
				//��Ʋ �� �ε�
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
		//���� ��
		enemyAmount = Random.Range(1, regions[0].maxAmountEnemys + 1);
		//enemyAmount = Random.Range(4, 5);	//��ű� ��
		//� ���� ��������
		for (int i = 0; i < enemyAmount; i++)
		{
			enemyToBattle.Add(regions[curRegions].possibleEnemys[Random.Range(0, regions[curRegions].possibleEnemys.Count)]);
		}
		lastHeroPosition = GameObject.Find("Player").gameObject.transform.position;
		//nextHreoPosition = lastHeroPosition;
		lastScene = SceneManager.GetActiveScene().name;
		//�ε� ����
		SceneManager.LoadScene(regions[curRegions].battleScene);
		//���� �ʱ�ȭ
		gotAttack = false;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	//Enemyã�Ƽ� List�� ����(PlayerManager���� ���)
	public void FindEnemy()
	{
		enemyList.Clear();
		GameObject[] tmpEnemys = GameObject.FindGameObjectsWithTag("Enemy");
		enemyList.AddRange(tmpEnemys);
	}
	//���� Enemy ã�Ƽ� ��Ȱ��ȭ(PlayerManager���� ���)
	public void OffEnemy()
	{
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (deadEnemyList.Contains(enemyList[i].name))	//�״� Enemy�� �̸��� ��ġ���� ��
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
	//Player��Ƽ Hp����
	public void PlayerHpSave(string _name, float _curHp, float _curMp)
	{
		PlayerPrefs.SetFloat($"{_name}_hp", _curHp);
		PlayerPrefs.SetFloat($"{_name}_mp", _curMp);
	}
	//Player��Ƽ Hp�ε�
	public void PlayerHpLoad(string _name, out float _curHp, out float _curMp)
	{
		_curHp = PlayerPrefs.GetFloat($"{_name}_hp");
		_curMp = PlayerPrefs.GetFloat($"{_name}_mp");
		Debug.Log("�ε�");
	}
}
