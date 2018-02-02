using UnityEngine;
using UnityEngine.SceneManagement;

public class MuteButtonBehaviour : ButtonBehaviour {

	protected override void Start () {
		base.Start ();
		if (PlayerPrefs.GetInt ("muteSound") > 0) {
			Deactivate ();
		} else {
			Activate ();
		}
	}
		
	protected override void Activate() {
		PlayerPrefs.SetInt ("muteSound", 0);
		AudioListener.pause = true;
		base.Activate();
	}

	protected override void Deactivate() {
		PlayerPrefs.SetInt ("muteSound", 1);
		AudioListener.pause = false;
		base.Deactivate();
	}
}
