using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

	public class DoubleIntEvent : UnityEvent<int, int>{}
	Dictionary <string, UnityEvent> zeroArgDict;
	Dictionary <string, DoubleIntEvent> doubleIntDict;
	static EventManager eventManager;

	public static EventManager instance {
		get {
			if (!eventManager) {
				eventManager = (EventManager) FindObjectOfType(typeof (EventManager));
				if (!eventManager) {
					Debug.LogError ("We need an eventManager for the game to work. :-)");
				} else {
					eventManager.Init(); 
				}
			}
			return eventManager;
		}
	}

	void Init(){
		if (zeroArgDict == null){
			zeroArgDict = new Dictionary<string, UnityEvent>();
		} if (doubleIntDict == null){
			doubleIntDict = new Dictionary<string, DoubleIntEvent>();
		}
	}

	public static void StartListening(string eventName, UnityAction listener){
		UnityEvent thisEvent = null;
		if (instance.zeroArgDict.TryGetValue(eventName, out thisEvent)){
			thisEvent.AddListener(listener);
		} else {
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			instance.zeroArgDict.Add(eventName, thisEvent);
		}
	}

	public static void StartListening(string eventName, UnityAction<int, int> listener){
		DoubleIntEvent thisEvent = null;
		if (instance.doubleIntDict.TryGetValue(eventName, out thisEvent)){
			thisEvent.AddListener(listener);
		} else {
			thisEvent = new DoubleIntEvent();
			thisEvent.AddListener(listener);
			instance.doubleIntDict.Add(eventName, thisEvent);
		}
	}

	public static void StopListening(string eventName, UnityAction listener){
		if (eventManager == null){
			return;
		}
		UnityEvent thisEvent = null;
		if(instance.zeroArgDict.TryGetValue(eventName, out thisEvent)){
			thisEvent.RemoveListener(listener);
		}
	}

	public static void StopListening(string eventName, UnityAction<int, int> listener){
		if (eventManager == null){
			return;
		}
		DoubleIntEvent thisEvent = null;
		if(instance.doubleIntDict.TryGetValue(eventName, out thisEvent)){
			thisEvent.RemoveListener(listener);
		}
	}

	public static void TriggerEvent(string eventName){
		UnityEvent thisEvent = null;
		if(instance.zeroArgDict.TryGetValue(eventName, out thisEvent)){
			thisEvent.Invoke();
		}
	}

	public static void TriggerEvent(string eventName, int arg0, int arg1){
		DoubleIntEvent thisEvent = null;
		if(instance.doubleIntDict.TryGetValue(eventName, out thisEvent)){
			thisEvent.Invoke(arg0,arg1);
		}
	}
		
}