using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour {

	public Sprite activeSprite;
	public Sprite inactiveSprite;

	private Image _buttonImage;
	private bool _isActive;

	// Use this for initialization
	protected virtual void Start () {
		_buttonImage = GetComponent<Image> ();
		EventTrigger trigger = GetComponent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener(ButtonClick);
		trigger.triggers[0] = entry;
	}
		
	protected virtual void ButtonClick(BaseEventData data) {
		Debug.Log ("click");
		if (_isActive) {
			Deactivate ();
		} else {
			Activate ();
		}
	}

	protected virtual void Activate() {
		_buttonImage.sprite = activeSprite;
		_isActive = true;
	}

	protected virtual void Deactivate() {
		_buttonImage.sprite = inactiveSprite;
		_isActive = false;
	}
}
