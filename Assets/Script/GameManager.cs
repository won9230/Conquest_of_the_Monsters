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
			Debug.Log("GameManager�� �ߺ��Ǽ� �����Ǿ����ϴ�");
		}
		DontDestroyOnLoad(gameObject);
	}
}
