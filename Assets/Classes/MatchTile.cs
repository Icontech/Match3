using UnityEngine;
using System.Collections;
using System;

public class MatchTile {
	MatchTile left;
	MatchTile right;
	MatchTile up;
	MatchTile down;
	int tileType;
	bool isEmpty;
	int id;
	int row = -1;
	int col = -1;
	int stepsMoved;

	public MatchTile(){}

	public MatchTile(int tileType, int row, int col, int id){
		this.tileType = tileType;
		this.row = row;
		this.col =col;
		this.id = id;
	}

	public MatchTile(int tileType, int row, int col){
		this.tileType = tileType;
		this.row = row;
		this.col =col;
	}

	public MatchTile Left {
		get { return this.left; }
		set { this.left = value; }
	}

	public MatchTile Right {
		get { return this.right; }
		set { this.right = value; }
	}

	public MatchTile Up {
		get { return this.up; }
		set { this.up = value; }
	}

	public MatchTile Down {
		get { return this.down; }
		set { this.down = value; }
	}

	public bool IsEmpty{
		get { return this.isEmpty; }
		set { this.isEmpty = value; }
	}

	public int TileType {
		get { return this.tileType; }
		set { this.tileType = value; }
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
}
