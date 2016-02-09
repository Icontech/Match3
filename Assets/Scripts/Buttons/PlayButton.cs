using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour {
	public InputField rowsInputField;
	public InputField columnsInputField;
	public GameObject mainMenuPanel;

	public void StartGameAndDestroyMainMenu(){
		int rows; 
		int columns;
		if(int.TryParse(rowsInputField.text, out rows) && int.TryParse(columnsInputField.text, out columns)){
			if(rows > 0 && columns > 0){
				GameManager.instance.StartGame(rows, columns);
				mainMenuPanel.SetActive(false);
			}
		} else  {
			GameManager.instance.StartGame(10,10);
			mainMenuPanel.SetActive(false);
		}
	}
}
