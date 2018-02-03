using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleBehaviour : MonoBehaviour {

	public GameObject playButton;

	private Image _muteButtonImage;

	// Use this for initialization
	void Start () {
		EventTrigger trigger = playButton.GetComponent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener(PlayButtonClick);
		trigger.triggers[0] = entry;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene("game", LoadSceneMode.Single);
		}
	}

	void PlayButtonClick(BaseEventData data) {
		SceneManager.LoadScene("game", LoadSceneMode.Single);
	}
}
