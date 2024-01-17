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

	private bool gotAttack = false;

	public string sceneToLoad;
	public string lastScene;

	private Vector3 lastHeroPosition;

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
			Destroy(gameObject);
			Debug.Log("GameManager�� �ߺ��Ǽ� �����Ǿ����ϴ�");
		}
		DontDestroyOnLoad(gameObject);
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
		//� ���� ��������
		for (int i = 0; i < enemyAmount; i++)
		{
			enemyToBattle.Add(regions[curRegions].possibleEnemys[Random.Range(0, regions[curRegions].possibleEnemys.Count)]);
		}
		lastHeroPosition = GameObject.Find("Player").gameObject.transform.position;
		lastScene = SceneManager.GetActiveScene().name;
		//�ε� ����
		SceneManager.LoadScene(regions[curRegions].battleScene);
		//���� �ʱ�ȭ
		gotAttack = false;
	}
	
	public void LoadNextScene()
	{
		SceneManager.LoadScene(sceneToLoad);
	}

	public void LoadSceneAfterBattle()
	{
		SceneManager.LoadScene(lastScene);
	}
}
