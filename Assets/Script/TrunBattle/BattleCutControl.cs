using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BattleCutControl : MonoBehaviour
{
	public static BattleCutControl instance;
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
			Debug.Log("BattleCutControl�� 2�� �ֽ��ϴ�. �̱������� ���� ������ �����߽��ϴ�.");
		}
	}

	private PlayableDirector pd;
	public TimelineAsset[] ta;

	private void Start()
	{
		pd = GetComponent<PlayableDirector>();
	}


	//�� ���� ������ ������ �ƽ� �ߵ�
	public void BattleCutPlay()
	{

		pd.Play(ta[0]);
	}

}
