using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VerticalMatchingButton : MonoBehaviour {
	Color defaultTextColor;
	Text btnText;
	Button btn;

	void Start(){
		btn = gameObject.GetComponent<Button>();
		btnText = btn.transform.FindChild("Text").GetComponent<Text>();
		defaultTextColor = btnText.color;
	}

	public void ToggleVerticalMatching(){
		GameManager.instance.VerticalMatchingEnabled = !GameManager.instance.VerticalMatchingEnabled;
		if(!GameManager.instance.VerticalMatchingEnabled){
			btnText.color = Color.gray;
		} else {
			btnText.color = defaultTextColor;
		}
		
	}
}
