using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[Tooltip("�÷��̾� �ӵ�")] public int anySpeed;
	public bool isAttack = false;	//�÷��̾ ���� ������ Ȯ��

	private new Rigidbody rigidbody;
	private PlayerAnimation anim;
	public void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		anim = GetComponent<PlayerAnimation>();
	}
	private void FixedUpdate()
	{
		if (!isAttack)
		{
			DoMove();
		}

		if(Input.GetMouseButtonDown(0) && !isAttack)
		{
			StartCoroutine(DoAttack());
		}
	}

	//�÷��̾� �̵�
	private void DoMove()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		Vector3 moveH = transform.right * h;
		Vector3 moveV = transform.forward * v;
		Vector3 velocity = (moveH + moveV).normalized * anySpeed;
		rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
		anim.DoMoveAnim(h,v);
	}

	//�÷��̾� ����
	IEnumerator DoAttack()
	{
		anim.DoPlayerAttack();
		yield return new WaitForSeconds(0.02f);
		isAttack = true;
		float animTime = anim.GetAnimPlayTime();
		//Debug.Log(animTime + " " + isAttack);
		yield return new WaitForSeconds(animTime);
		isAttack = false;
	}

	IEnumerator DoDefas()
	{
		yield return new WaitForSeconds(0.05f);
	}
}
