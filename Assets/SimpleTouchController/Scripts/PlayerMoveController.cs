﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMoveController : MonoBehaviour {

	// PUBLIC
	public SimpleTouchController leftController;
	public SimpleTouchController rightController;
	public ButtonStatus boost;
	public float speedMovements = 5f;
	public float speedContinuousLook = 100f;
	public float speedProgressiveLook = 3000f;
	public bool continuousRightController = true;

	// PRIVATE
	private Rigidbody _rigidbody;
	private Vector3 localEulRot;
	private Vector2 prevRightTouchPos;

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		rightController.TouchEvent += RightController_TouchEvent;
		rightController.TouchStateEvent += RightController_TouchStateEvent;
	}

	public bool ContinuousRightController
	{
		set{continuousRightController = value;}
	}

	void RightController_TouchStateEvent (bool touchPresent)
	{
		if(!continuousRightController)
		{
			prevRightTouchPos = Vector2.zero;
		}
	}

	void RightController_TouchEvent (Vector2 value)
	{
		if(!continuousRightController)
		{
			Vector2 deltaValues = value - prevRightTouchPos;
			prevRightTouchPos = value;

			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x - deltaValues.y * Time.deltaTime * speedProgressiveLook,
				transform.localEulerAngles.y + deltaValues.x * Time.deltaTime * speedProgressiveLook,
				0f);
		}
	}

	void Update()
	{
		// move
		var speedThisFrame = boost.isHeld ? speedMovements * 5 : speedMovements;
		_rigidbody.AddForce((transform.forward * leftController.GetTouchPosition.y * Time.deltaTime * speedThisFrame) +
			(transform.right * leftController.GetTouchPosition.x * Time.deltaTime * speedMovements) );

		if(continuousRightController)
		{
			var lookVector = new Vector3(transform.localEulerAngles.x - rightController.GetTouchPosition.y * Time.deltaTime * speedContinuousLook,
				transform.localEulerAngles.y + rightController.GetTouchPosition.x * Time.deltaTime * speedContinuousLook,
				0f);
			float x = lookVector.x;
			x = (x > 180) ? x - 360 : x;
			if (x > 80)
			{
				lookVector.x = 80;
			}
			else if (x < -80)
			{
				lookVector.x = -80;
			}
			transform.localEulerAngles = lookVector;
		}
	}

	void OnDestroy()
	{
		rightController.TouchEvent -= RightController_TouchEvent;
		rightController.TouchStateEvent -= RightController_TouchStateEvent;
	}

}
