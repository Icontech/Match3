using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static CameraController instance;
	float speed = 5;
	public GameObject background;
	float angleToBackgroundEdgeX = 0.675f;
	float angleToBackgroundEdgeY = 0.46f;
	float adjustCameraPosByXSizeThreshold = 1.6f;
	int minDistToBackground = 10;

	void Awake(){
		instance = this;
	}

	void Start () {
		ResetCameraPosition();
	}

	void Update () {
		if(Input.GetKey(KeyCode.W)){
			if(Input.GetKey(KeyCode.LeftShift)){
				transform.position += new Vector3(0,0,speed*Time.deltaTime);
			} else {
				transform.position += new Vector3(0,speed*Time.deltaTime,0);	
			}
		}
		if(Input.GetKey(KeyCode.S)){
			if(Input.GetKey(KeyCode.LeftShift)){
				transform.position += new Vector3(0,0,-speed*Time.deltaTime);
			} else {
				transform.position += new Vector3(0,-speed*Time.deltaTime,0);	
			}
		    
		}
		if(Input.GetKey(KeyCode.A)){
			transform.position += new Vector3(-speed*Time.deltaTime,0,0);
		}
		if(Input.GetKey(KeyCode.D)){
			transform.position += new Vector3(speed*Time.deltaTime,0,0);
		}
	}

	public void ResetCameraPosition(){
		float distToBackground;

		if((background.transform.localScale.x/background.transform.localScale.y) > adjustCameraPosByXSizeThreshold){
			distToBackground = (background.transform.localScale.x*0.5f)/Mathf.Tan(angleToBackgroundEdgeX);
		} else {
			distToBackground = (background.transform.localScale.y*0.5f)/Mathf.Tan(angleToBackgroundEdgeY);
		}
		if (distToBackground > minDistToBackground){
			transform.position = new Vector3(0,0,-distToBackground);
		}
	}
}
