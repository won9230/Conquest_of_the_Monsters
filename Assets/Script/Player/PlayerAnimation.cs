using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	private Animator anim;
	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	//�÷��̾� �̵� �ִϸ��̼�
	public void DoMoveAnim(float _x, float _y)
	{
		int x = Mathf.Abs((int)_x);
		int y = Mathf.Abs((int)_y);
		//Debug.Log("x" + x + "y" + y);
		if(x == 0 && y >= 1)
		{
			anim.SetFloat("x", y);
		}
		else if(x >= 1 && y == 0)
		{
			anim.SetFloat("x", x);
		}
		else if(x >= 1 && y >= 1)
		{
			anim.SetFloat("x", x);
		}
		else
		{
			anim.SetFloat("x", 0);
		}
	}
	
	//�÷��̾� ���� �ִϸ��̼�
	public void DoPlayerAttack()
	{
		anim.SetTrigger("Attack");
	}

	public float GetAnimPlayTime()
	{
		return anim.GetCurrentAnimatorStateInfo(0).length;
	}
}
