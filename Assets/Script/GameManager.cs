using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);		//���� �ٲ� �ı����� ����
		Cursor.visible = false;		//Ŀ�� �Ⱥ��̰� ����
		Cursor.lockState = CursorLockMode.Locked;	//Ŀ�� ���
	}


}
