using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	public enum GameState
	{
		Field,
		Battle
	}
	public GameState gameState = GameState.Field;
	private void Awake()
	{
		//DontDestroyOnLoad(gameObject);		//씬이 바뀌어도 파괴되지 않음
		//Cursor.visible = false;		//커서 안보이게 설정
		//Cursor.lockState = CursorLockMode.Locked;	//커서 잠금
	}

	private void Update()
	{
		switch (gameState)
		{
			case (GameState.Field):

				break;
			case (GameState.Battle):

				break;
		}
	}

}
