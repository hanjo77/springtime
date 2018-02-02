using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonBehaviour : ButtonBehaviour {

	public PlayerBehaviour player;

	protected override void Activate() {
		player.Pause ();
		Time.timeScale = 0;
		base.Activate();
	}

	protected override void Deactivate() {
		player.Play ();
		Time.timeScale = 1;
		base.Deactivate();
	}
}
