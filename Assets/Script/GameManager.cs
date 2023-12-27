using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
			Debug.Log("GameManager가 중복되서 삭제되었습니다");
		}
		DontDestroyOnLoad(gameObject);
	}
}
