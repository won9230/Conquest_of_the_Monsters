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
		//DontDestroyOnLoad(gameObject);		//���� �ٲ� �ı����� ����
		//Cursor.visible = false;		//Ŀ�� �Ⱥ��̰� ����
		//Cursor.lockState = CursorLockMode.Locked;	//Ŀ�� ���
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
