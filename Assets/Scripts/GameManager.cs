using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public Text scoreText;
	public Text gameOverText;
	public GameObject pauseMenuPanel;
	public int MAX_ALLOWED_TILES = 3;
	int score;
	bool isPaused;
	bool gameIsRunning;
	bool isNewTurn;
	public GameObject background;
	AudioSource tileCollisionAudio;
	bool verticalMatchingEnabled = true;

	void Awake(){
		instance = this;
		isNewTurn = true;
	}

    void Start () {
		tileCollisionAudio = GetComponent<AudioSource>();
    }

	void Update () {
		if(gameIsRunning){
			if(Input.GetKeyDown(KeyCode.Escape)){
				isPaused = !isPaused;
				if(isPaused){
					pauseMenuPanel.SetActive(true);
				} else {
					pauseMenuPanel.SetActive(false);
				}
			}
		} 
	}

	public void StartGame(int rows, int columns){
		gameIsRunning = true;
		BoardManager.instance.BoardSetup(rows, columns);
		background.transform.localScale = new Vector3(columns, rows, 0.1f);
		background.SetActive(true);
		CameraController.instance.ResetCameraPosition();
	}
		
	public void AddScore(int newScoreValue){
		score += newScoreValue;
		scoreText.text = "SCORE: "+score;
	}

	public void playCollisionAudio(){
		tileCollisionAudio.Play();
	}

	public bool VerticalMatchingEnabled{
		get{ return this.verticalMatchingEnabled;}
		set{ this.verticalMatchingEnabled = value;}
	}

	public void EndGame(){
		gameIsRunning = false;
		background.SetActive(false);
		pauseMenuPanel.SetActive(true);
		gameOverText.gameObject.SetActive(true);
		gameOverText.text += " "+score;
	}

	public bool IsNewTurn{
		get{ return this.isNewTurn;}
		set{ this.isNewTurn = value;}
	}
}