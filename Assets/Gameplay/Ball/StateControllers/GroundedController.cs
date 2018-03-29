﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedController : BallController {

	public override BallController CheckTransitions(Ball ball) {
		if (CheckAirbornTransition(ball))
			return new AirbornController();
		if (CheckCompressedTransition(ball))
			return CompressedControllerFactory.GetCompressedController(ball.playerInfo.inputScheme);
		return null;
	}

	public override void Enter(Ball ball) {
		Rigidbody2D rb2d = ball.collisionManager.gameObject.GetComponent<Rigidbody2D>();
		rb2d.gravityScale = 1f;
	}

	bool CheckAirbornTransition(Ball ball) {
		return !ball.collisionManager.ballIsGrounded ? true : false;
	}

	bool CheckCompressedTransition(Ball ball) {
		Vector2 inputDirection = ball.playerInfo.inputScheme.GetInputDirection();
		Vector2 surfaceNormal = ball.collisionManager.GetSumContactNormal();
		return Vector2.Dot(inputDirection, surfaceNormal) < 0 ? true : false;
	}
}

