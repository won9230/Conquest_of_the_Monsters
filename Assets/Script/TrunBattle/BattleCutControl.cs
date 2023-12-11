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
			Debug.Log("BattleCutControl가 2개 있습니다. 싱글톤으로 인한 삭제를 실행했습니다.");
		}
	}

	private PlayableDirector pd;
	public TimelineAsset[] ta;

	private void Start()
	{
		pd = GetComponent<PlayableDirector>();
	}


	//적 한테 공격을 받으면 컷신 발동
	public void BattleCutPlay()
	{

		pd.Play(ta[0]);
	}

}
