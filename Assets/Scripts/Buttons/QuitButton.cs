using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class QuitButton : MonoBehaviour {

	 public void Quit(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
