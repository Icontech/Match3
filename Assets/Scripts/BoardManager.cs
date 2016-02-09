using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class BoardManager : MonoBehaviour {
	
	public static BoardManager instance;
	Transform tileHolder;
	public GameObject[] tileTypes;
	Board<MatchTile> board;
	List<int> tileIDsToCheckForMatches;
	List<MatchTile> newDeletedTiles;
	List<int> checkedHorizontalTileIDs;
	List<int> checkedVerticalTileIDs;
	enum Directions {Up, Down,Left, Right};
	Dictionary<int, GameObject> gameObjectDict;
	List<int> deletedIdsLastTurn;
	AudioSource tileBreakAudio;
	bool isRunning;
	float destroySpeed = 0.17f;

	void Awake(){
		instance = this;
	}

	void Update(){
		if(isRunning){
			if(tileHolder.childCount == 0){
				isRunning = false;
				GameManager.instance.EndGame();
			}
		}
	}

	public void BoardSetup(int rows, int columns) {
		board = new Board<MatchTile>(rows, columns);
		tileIDsToCheckForMatches = new List<int>();
		tileHolder = new GameObject("TileHolder").transform;
		tileHolder.position = new Vector3(-columns*0.5f+0.5f,rows*0.5f-0.5f,0f);
		gameObjectDict = new Dictionary<int, GameObject>();
		checkedHorizontalTileIDs = new List<int>();
		checkedVerticalTileIDs = new List<int>();
		newDeletedTiles = new List<MatchTile>();
		deletedIdsLastTurn = new List<int>();
		tileBreakAudio = GetComponent<AudioSource>();
		tileBreakAudio.pitch = 0.7f;

		int id = 0;
		for(int y = 0; y < rows; y++) {

			for(int x = 0; x < columns; x++) {
				MatchTile tmpTile = new MatchTile(y,x, id);
				board.Add(tmpTile, y,x);
				board.SetLeftAndUpNeighbors(y,x);
				GameObject toInstantiate = GetValidTileToInstantiate(y,x);
				GameObject instance =
					(GameObject)Object.Instantiate(toInstantiate, new Vector3 (tileHolder.position.x+x, tileHolder.position.y-y, 0f), Quaternion.identity);
				instance.GetComponent<TileMover>().Row = y;
				instance.GetComponent<TileMover>().Col = x;
				instance.GetComponent<TileMover>().Id = id;
				instance.transform.SetParent(tileHolder);
				MatchTile tile = new MatchTile(instance.GetComponent<TileMover>().tileType, y,x, id);
				board.Add(tile,y,x); 
				board.SetLeftAndUpNeighbors(y,x);
				gameObjectDict.Add(id, instance);
				id++;
			}
		}
		isRunning = true;
	}

	GameObject GetValidTileToInstantiate(int row, int col){
		GameObject validTile = tileTypes[Random.Range(0,tileTypes.Length)];
		int currentType = validTile.GetComponent<TileMover>().tileType;
		MatchTile currentTile = board.Get(row,col);
		currentTile.TileType = currentType;
		bool isMaxHorizontal = false;
		bool isMaxVertical = false;

		if(CountSetupMatches(board.Get(row,col),0,currentType,Directions.Left) > GameManager.instance.MAX_ALLOWED_TILES)
			isMaxHorizontal = true;
		if(CountSetupMatches(board.Get(row,col),0,currentType,Directions.Up) > GameManager.instance.MAX_ALLOWED_TILES)
			isMaxVertical = true;

		if(isMaxVertical || isMaxHorizontal){
			List<GameObject> validTiles = new List<GameObject>();
			foreach(GameObject tt in tileTypes){
				if(currentTile.Left != null && currentTile.Up != null){
					if(tt.GetComponent<TileMover>().tileType != currentTile.Left.TileType && tt.GetComponent<TileMover>().tileType != currentTile.Up.TileType){
						validTiles.Add(tt);
					}
				} else if ((currentTile.Left == null && currentTile.Up != null) || (currentTile.Left != null && currentTile.Up == null)){
					if(tt.GetComponent<TileMover>().tileType != currentType){
						validTiles.Add(tt);
					}
				}
			}
			validTile = validTiles[Random.Range(0,validTiles.Count)];
		}
		return validTile;
	}

	int CountSetupMatches(MatchTile tile, int count, int tileType, Directions direction){
		if(tile == null || tile.TileType != tileType){
			return count;
		}
		count++;
		MatchTile nextTile = new MatchTile();
		if(direction == Directions.Left){
			nextTile = tile.Left;
		} else if (direction == Directions.Up){
			nextTile = tile.Up;
		}
		return CountSetupMatches(nextTile,count, tileType, direction);
	}
		

	void StartTurn(int row, int col){
		MatchTile tile = board.Get(row,col);
		if(tile.Up == null || tile.Up.IsEmpty){
			tile.IsEmpty = true;
			Destroy(gameObjectDict[tile.Id]);
			GoToNextTurn();
		} else{
			tile.IsEmpty = true;
			newDeletedTiles.Add(tile);
			MoveAllTiles();
		}
	}
		
	void MovementDone(){
		newDeletedTiles.Clear();
		if(tileIDsToCheckForMatches.Count != 0){
			InitMatchSearch();	
		}else {
			GoToNextTurn();
		}
	}

	void InitMatchSearch(){
		foreach(int tileId in tileIDsToCheckForMatches){
			int r = gameObjectDict[tileId].GetComponent<TileMover>().Row;
			int c = gameObjectDict[tileId].GetComponent<TileMover>().Col;
			MatchTile startTile = board.Get(r,c);
			SearchMatches(startTile);
		}
		checkedHorizontalTileIDs.Clear();
		checkedVerticalTileIDs.Clear();
		deletedIdsLastTurn.Clear();
		tileIDsToCheckForMatches.Clear();
		MoveAllTiles();
	}
		
	void SearchMatches(MatchTile tile){
		if(tile == null || (tile.IsEmpty && !newDeletedTiles.Contains(tile))){
			return;
		}
		checkedHorizontalTileIDs.Add(tile.Id);
		int countLeft = 0;
		int countRight= 0;
		List<MatchTile> leftDeletedTiles = new List<MatchTile>();
		List<MatchTile> rightDeletedTiles = new List<MatchTile>();

		if(tile.Left != null){
			countLeft = CountMatches(tile.Left, 0, tile.TileType, Directions.Left, leftDeletedTiles, checkedHorizontalTileIDs);
		} if(tile.Right != null){
			countRight = CountMatches(tile.Right, 0, tile.TileType, Directions.Right, rightDeletedTiles, checkedHorizontalTileIDs);
		} 
		int totalHorizontalCount = countLeft+countRight+1;


		if(totalHorizontalCount > GameManager.instance.MAX_ALLOWED_TILES){
			for(int i = leftDeletedTiles.Count-1; i >= 0; i--){
				leftDeletedTiles[i].IsEmpty = true;
				newDeletedTiles.Add(leftDeletedTiles[i]);
			}
			tile.IsEmpty = true;
			newDeletedTiles.Add(tile);

			foreach(MatchTile t in rightDeletedTiles){
				t.IsEmpty = true;
				newDeletedTiles.Add(t);
			}
		}

		if(GameManager.instance.VerticalMatchingEnabled){
			checkedVerticalTileIDs.Add(tile.Id);
			int countUp = 0;
			int countDown = 0;
			List<MatchTile> upDeletedTiles = new List<MatchTile>();
			List<MatchTile> downDeletedTiles = new List<MatchTile>();

			if(tile.Up != null){
				countUp = CountMatches(tile.Up, 0, tile.TileType, Directions.Up, upDeletedTiles, checkedVerticalTileIDs);
			} if(tile.Down != null){
				countDown = CountMatches(tile.Down, 0, tile.TileType, Directions.Down, downDeletedTiles, checkedVerticalTileIDs);
			}
			int totalVerticalCount = countUp+countDown+1;
			if(totalVerticalCount > GameManager.instance.MAX_ALLOWED_TILES){
				for(int i = upDeletedTiles.Count-1; i >= 0; i--){
					upDeletedTiles[i].IsEmpty = true;
					newDeletedTiles.Add(upDeletedTiles[i]);
				}
				if(totalHorizontalCount <= GameManager.instance.MAX_ALLOWED_TILES){
					tile.IsEmpty = true;
					newDeletedTiles.Add(tile);
				}

				foreach(MatchTile t in downDeletedTiles){
					t.IsEmpty = true;
					newDeletedTiles.Add(t);
				}
			}
		}

		SearchMatches(tile.Up);
	}

	int CountMatches(MatchTile tile, int count, int prevTileType, Directions direction, List<MatchTile> matchedTiles, List<int> checkedList){
		if(tile == null || checkedList.Contains(tile.Id) || tile.TileType != prevTileType || tile.IsEmpty){
			return count;
		}
		count++;
		MatchTile nextTile = new MatchTile();
		checkedList.Add(tile.Id);
		matchedTiles.Add(tile);
		if(direction == Directions.Left){
			nextTile = tile.Left;
		} else if(direction == Directions.Right){
			nextTile = tile.Right;
		} else if (direction == Directions.Up){
			nextTile = tile.Up;
		} else if (direction == Directions.Down){
			nextTile = tile.Down;
		}
		return CountMatches(nextTile, count, tile.TileType, direction, matchedTiles, checkedList);	
	}

	void MoveAllTiles(){
		if(newDeletedTiles.Count != 0){
			foreach(MatchTile deleted in newDeletedTiles){
				if(deleted.Up != null && !deleted.Up.IsEmpty){
					tileIDsToCheckForMatches.Add(deleted.Up.Id);
				}
			}
			MatchTile longestMover = new MatchTile();
			longestMover.Id = -1;
			foreach(MatchTile deleted in newDeletedTiles){
				deletedIdsLastTurn.Add(deleted.Id);
				longestMover = MoveDeletedTileToTop(deleted,0, longestMover);
			}
			if(longestMover.Id != -1){
				SetLongestMover(longestMover.Id);
			}
			StartCoroutine(DestroyGameObjects());

		} else{
			GoToNextTurn();
		}
	}

	IEnumerator DestroyGameObjects(){
		float pitchInc = 0.1f;
		foreach(MatchTile tile in newDeletedTiles.ToArray()){
			if(newDeletedTiles.Count > 1){
				tileBreakAudio.pitch += pitchInc;
				tileBreakAudio.Play();
				GameManager.instance.AddScore(20);
			}
			Destroy(gameObjectDict[tile.Id]);
			yield return new WaitForSeconds(destroySpeed);
		}
		if(tileIDsToCheckForMatches.Count != 0){
			EventManager.TriggerEvent("InitMove");
		}else{
			yield return new WaitForSeconds(0.1f);
			newDeletedTiles.Clear();
			GoToNextTurn();
		}
	}

	void SetLongestMover(int longestStepsMovedId){
		if(gameObjectDict[longestStepsMovedId] != null)
			gameObjectDict[longestStepsMovedId].GetComponent<TileMover>().LongestMover = true;
	}

	MatchTile MoveDeletedTileToTop(MatchTile tile, int count, MatchTile longestMover){
		if(tile.Up == null || ((tile.Up.IsEmpty && !deletedIdsLastTurn.Contains(tile.Up.Id)) && (tile.Up.IsEmpty && !newDeletedTiles.Contains(tile.Up)))){
			return longestMover;
		}
		if(!tile.Up.IsEmpty){
			TileMover tileHolder = gameObjectDict[tile.Up.Id].GetComponent<TileMover>();
			tileHolder.StepsMoved++;
			tileHolder.Row++;
			if((tileHolder.StepsMoved > longestMover.StepsMoved) && !newDeletedTiles.Contains(tile.Up)){
				longestMover.StepsMoved = tileHolder.StepsMoved;
				longestMover.Id = tileHolder.Id;
			}
		}

		MatchTile up = tile.Up;
		board.SwapTilePositions(tile.Row,tile.Col,up.Row, up.Col);
		MatchTile left = tile.Left;
		MatchTile right = tile.Right;
		MatchTile down = tile.Down;
		MatchTile upUp = tile.Up.Up;
		MatchTile upLeft = tile.Up.Left;
		MatchTile upRight = tile.Up.Right;

		if(upUp != null)
			upUp.Down = tile;
		if(upLeft != null)
			upLeft.Right = tile;
		if(upRight != null)
			upRight.Left = tile;
		if(down != null)
			down.Up = up;
		if(left != null)
			left.Right = up;	
		if(right != null)
			right.Left = up;

		tile.Up = upUp;
		tile.Down = up;
		tile.Left = upLeft;
		tile.Right = upRight;
		up.Up = tile;
		up.Down = down;
		up.Left = left;
		up.Right = right;
		count++;
		return MoveDeletedTileToTop(tile, count, longestMover);
	}
		
	void OnEnable(){
		EventManager.StartListening("StartTurn", StartTurn);
		EventManager.StartListening("MovementDone", MovementDone);
	}

	void OnDisable(){
		EventManager.StopListening("StartTurn", StartTurn);
		EventManager.StopListening("MovementDone", MovementDone);
	}

	void GoToNextTurn(){
		tileBreakAudio.pitch = 0.7f;
		GameManager.instance.IsNewTurn = true;
	}
}