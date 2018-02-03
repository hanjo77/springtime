using UnityEngine;
using UnityEngine.SceneManagement;

public class MuteButtonBehaviour : ButtonBehaviour {

	protected override void Start () {
		base.Start ();
		Debug.Log (PlayerPrefs.GetInt ("muteSound"));
		if (PlayerPrefs.GetInt ("muteSound") > 0) {
			Activate ();
		} else {
			Deactivate ();
		}
	}
		
	protected override void Activate() {
		PlayerPrefs.SetInt ("muteSound", 1);
		AudioListener.pause = true;
		base.Activate();
		Debug.Log ("test");
	}

	protected override void Deactivate() {
		PlayerPrefs.SetInt ("muteSound", 0);
		AudioListener.pause = false;
		base.Deactivate();
	}
}
