using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;

public class TileMover : MonoBehaviour {

	public int tileType;
	float speed = 4;
	int score = 10;
	bool isMoving;
	int row;
	int col;
	int stepsMoved;
	float goalYPosition;
	int id;
	bool longestMover;


	void Update () {
		if(isMoving){
			transform.Translate(Vector3.down * speed * Time.deltaTime);
			if(transform.position.y < goalYPosition){
				isMoving = false;
				stepsMoved = 0; 
				transform.position = new Vector3(transform.position.x, goalYPosition, 0);
				if(longestMover){
					longestMover = false;
					StartCoroutine(SendMovementDoneEvent());
				}
			}
		}
	}


	void OnMouseDown(){
		if(GameManager.instance.IsNewTurn){
			GameManager.instance.IsNewTurn = false;
			GameManager.instance.AddScore(score);
			EventManager.TriggerEvent("StartTurn", row, col);
		}
	}

	IEnumerator SendMovementDoneEvent(){
		GameManager.instance.playCollisionAudio();
		yield return new WaitForSeconds(0.2f);
		EventManager.TriggerEvent("MovementDone");
	}

	void InitMove(){
		if(stepsMoved != 0 && !isMoving){
			goalYPosition = transform.position.y-stepsMoved;
			isMoving = true;
		}
	}

	void OnEnable(){
		EventManager.StartListening("InitMove", InitMove);
	}

	void OnDisable(){
		EventManager.StopListening("InitMove", InitMove);
	}


	public int Row{
		get { return this.row; }
		set { this.row = value; }
	}

	public int Col{
		get { return this.col; }
		set { this.col = value; }
	}

	public int Id{
		get { return this.id; }
		set { this.id = value; }
	}

	public int StepsMoved{
		get { return this.stepsMoved; }
		set { this.stepsMoved = value; }
	}

	public bool LongestMover{
		get{ return this.longestMover; }
		set{ this.longestMover = value; } 
	}

}