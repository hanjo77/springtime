using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonBehaviour : ButtonBehaviour {

	public GameBehaviour gameManager;

	protected override void Activate() {
		gameManager.Pause ();
		Time.timeScale = 0;
		base.Activate();
	}

	protected override void Deactivate() {
		gameManager.Play ();
		Time.timeScale = 1;
		base.Deactivate();
	}
}
