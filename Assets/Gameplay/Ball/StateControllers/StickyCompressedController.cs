﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyCompressedController : BallController {

	protected float timeCompressed;
	protected float maxTimeCompressed = 0.5f;
	protected float maxLaunchAngle = 50f;
	protected Vector2 releaseVector;
	float maxAngularVelocity = 50f;
	Vector2 lastDirection;

	public override void Enter(Ball ball) {
		timeCompressed = 0f;
		ball.aimBar.Show();
		ball.animator.SetBool("Squished", true);
		Rigidbody2D rb2d = ball.GetComponent<Rigidbody2D>();
		rb2d.gravityScale = 0;
		lastDirection = -ball.collisionManager.GetReboundVector();
	}

	public override void Exit(Ball ball) {
		ball.aimBar.Hide();
		ball.animator.SetBool("Squished", false);
	}

	public override BallController CheckTransitions(Ball ball) {
		if (CheckAirbornTransition(ball))
			return new AirbornController();
		if (CheckLaunchTransition(ball) || CheckTimeoutTransition(ball)) {
			LaunchBall(ball, releaseVector);
			return new AirbornController();
		}
		return null;
	}

	public override void Update(Ball ball) {
		timeCompressed += Time.deltaTime;
		Vector2 referenceVector = ball.collisionManager.GetReboundVector();
		Vector2 inputDirection = ball.playerInfo.inputScheme.GetInputDirection();
		Vector2 clampedDirection = ClampDirection(inputDirection, -referenceVector.normalized, maxLaunchAngle);
		Vector2 smoothedDirection = ClampDirection(clampedDirection, lastDirection, maxAngularVelocity * Time.deltaTime);
		lastDirection = smoothedDirection;
		float magnitude = FindMagnitude(smoothedDirection, referenceVector);
		releaseVector = -smoothedDirection * magnitude;
		ball.aimBar.UpdatePosition(-releaseVector);
	}

	protected Vector2 ClampDirection(Vector2 direction, Vector2 referenceDirection, float maxAngle) {
		float angle = Vector2.SignedAngle(referenceDirection, direction);
		if (Mathf.Abs(angle) <= maxAngle)
			return direction;
		float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);
		Quaternion rotation = Quaternion.AngleAxis(clampedAngle, Vector3.forward);
		return rotation * referenceDirection;
	}

	protected float FindMagnitude(Vector2 direction, Vector2 referenceVector) {
		return referenceVector.magnitude;
	}

	protected void LaunchBall(Ball ball, Vector2 launchDirection) {
		float forceScaling = 450f;
		Vector2 releaseForce = launchDirection * forceScaling;
		ball.collisionManager.gameObject.GetComponent<Rigidbody2D>().AddForce(releaseForce);
	}

	protected bool CheckAirbornTransition(Ball ball) {
		return !ball.collisionManager.ballIsGrounded ? true : false;
	}

	protected bool CheckLaunchTransition(Ball ball) {
		Vector2 inputDirection = ball.playerInfo.inputScheme.GetInputDirection();
		return inputDirection == Vector2.zero ? true : false;
	}

	protected bool CheckTimeoutTransition(Ball ball) {
		return timeCompressed >= maxTimeCompressed ? true : false;
	}
}