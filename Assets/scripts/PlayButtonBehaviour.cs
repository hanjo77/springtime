using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonBehaviour : ButtonBehaviour {

	protected override void Activate() {
		SceneManager.LoadScene("game", LoadSceneMode.Single);
		base.Activate();
	}

	protected override void Deactivate() {
		SceneManager.LoadScene("game", LoadSceneMode.Single);
		base.Deactivate();
	}
}
