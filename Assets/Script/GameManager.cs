using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[System.Serializable]
	public class RagionData
	{
		public string regionName;
		public int maxAmountEnemys = 4;
		public List<GameObject> possibleEnemys = new List<GameObject>();
	}

	public List<RagionData> ragions = new List<RagionData>();

	private bool gotAttack = false;

	public enum GameState
	{
		World_State,
		Town_State,
		Battle_State,
		Idle
	}
	public GameState gamestate;

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

				break;
			case GameState.Idle: 
				break;
			default:
			break;
		}
	}
}
