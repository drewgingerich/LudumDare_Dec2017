﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	// public event System.Action<Ball> OnDie;

	// public AimBarBehavior aimBar;
	// public BallSpriteBehavior ballSprite;
	// public Animator animator;

	public ControlScheme controlScheme;
	public BallCollision collisionManager;
	public Animator animator;
	public BallState state;

	BallController controller;

	// public void LoadControlScheme (ControlScheme controlScheme) {
	// 	this.controlScheme = controlScheme;
	// }

	void Awake() {
		controlScheme = new ControlScheme("Horizontal", "Vertical"); 
		controller = new AirbornController(this);
	}

	// public void Die () {
	// 	if (OnDie != null)
	// 		OnDie (this);
	// 	Destroy (gameObject);
	// }

	void Update () {
		controlScheme.UpdateInput();
		bool checkForTransition = true;
		while (checkForTransition) {
			checkForTransition = CheckForNewState();
		}
		controller.Update ();
	}

	bool CheckForNewState() {
		BallController newController = controller.CheckTransitions();
		if (newController != null) {
			controller.Exit();
			controller = newController;
			controller.Enter();
			return true;
		} else {
			return false;
		}
	}
}