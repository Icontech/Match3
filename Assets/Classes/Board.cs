using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board<BoardTile> {
	MatchTile [,] board;
	int height;
	int width;
	enum Directions {Left, Right, Up, Down};

	public Board(int rows, int columns){
		board = new MatchTile[rows, columns];
		height = rows;
		width = columns;
	}

	public void Add(MatchTile tile, int row, int col ){
		board[row,col] = tile;
		tile.Row = row;
		tile.Col = col;
	}

	public void SetLeftAndUpNeighbors(int row, int col){
		//CORNERS
		if(row == 0 && col == 0){
			SetAndUpdate(row, col, 0,0,0,0);
			return;
		} if(row == 0 && col == width-1){
			SetAndUpdate(row,col, 0,0,1,0);
			return;
		} if(row == height-1 && col == 0){
			SetAndUpdate(row,col, 1,0,0,0);
			return;
		} if(row == height-1 && col == width-1){
			SetAndUpdate(row,col, 1,0,1,0);
			return;
		} 

		//FIRST OR LAST ROW
		if(row == 0 && col !=0 && col != width-1){
			SetAndUpdate(row,col,0,0,1,0);
			return;
		}
		if(row == height-1 && col !=0 && col != width-1){
			SetAndUpdate(row,col,1,0,1,0);
			return;
		}
		//FIRST OR LAST COL
		if(col == 0 && row != 0 && row != height-1 ){
			SetAndUpdate(row,col,1,0,0,0);
			return;
		}
		if(col == width-1 && row != 0 && row != height-1 ){
			SetAndUpdate(row,col,1,0,1,0);
			return;
		}
		SetAndUpdate(row,col,1,0,1,0);
	}

	public void SetAndUpdate(int row, int col, int up, int down, int left, int right){
		MatchTile tile = board[row,col];
		if(up == 0)
			tile.Up = null;
		else {
			tile.Up = board[tile.Row-1,tile.Col];
			board[tile.Row-1,tile.Col].Down = tile;
		}
			
		if(down == 0)
			tile.Down = null;
		else {
			tile.Down = board[tile.Row+1,tile.Col];
			board[tile.Row+1,tile.Col].Up = tile;
		}
		if(left == 0)
			tile.Left = null;
		else {
			tile.Left = board[tile.Row,tile.Col-1];
			board[tile.Row,tile.Col-1].Right = tile;
		}
		if(right == 0)
			tile.Right = null;
		else {
			tile.Right = board[tile.Row,tile.Col+1];
			board[tile.Row,tile.Col+1].Left = tile;
		}
	}

	public void SwapTilePositions(int row1, int col1, int row2, int col2){
		board[row1,col1].Row = row2;
		board[row1,col1].Col= col2;
		board[row2,col2].Row = row1;
		board[row2,col2].Col= col1;
		MatchTile tmpTile = board[row1,col1];
		board[row1,col1] = board[row2,col2];
		board[row2,col2] = tmpTile;
	}

	public MatchTile Get(int row, int col){
		return board[row,col];
	}
		
	public int Height {
		get { return this.height; }
		set { this.height = value; }
	}

	public int Width {
		get { return this.width; }
		set { this.width = value; }
	}

	//Not in use. For future implementations
	public void SetAllNeighbors(int row, int col){
		//CORNERS
		if(row == 0 && col == 0){
			SetAndUpdate(row, col, 0,1,0,1);
			return;
		} if(row == 0 && col == width-1){
			SetAndUpdate(row,col, 0,1,1,0);
			return;
		} if(row == height-1 && col == 0){
			SetAndUpdate(row,col, 1,0,0,1);
			return;
		} if(row == height-1 && col == width-1){
			SetAndUpdate(row,col, 1,0,1,0);
			return;
		} 

		//FIRST OR LAST ROW
		if(row == 0 && col !=0 && col != width-1){
			SetAndUpdate(row,col,0,1,1,1);
			return;
		}
		if(row == height-1 && col !=0 && col != width-1){
			SetAndUpdate(row,col,1,0,1,1);
			return;
		}
		//FIRST OR LAST COL
		if(col == 0 && row != 0 && row != height-1 ){
			SetAndUpdate(row,col,1,1,0,1);
			return;
		}
		if(col == width-1 && row != 0 && row != height-1 ){
			SetAndUpdate(row,col,1,1,1,0);
			return;
		}
		SetAndUpdate(row,col,1,1,1,1);
	}
}
