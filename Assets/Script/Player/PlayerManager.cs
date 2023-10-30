using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public int anySpeed;
	private new Rigidbody rigidbody;
	private PlayerAnimation anim;
	public void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		anim = GetComponent<PlayerAnimation>();
	}
	private void FixedUpdate()
	{
		DoMove();
	}

	public void DoMove()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		Vector3 moveH = transform.right * h;
		Vector3 moveV = transform.forward * v;
		Vector3 velocity = (moveH + moveV).normalized * anySpeed;
		rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
		anim.DoMoveAnim(h,v);
	}
}
