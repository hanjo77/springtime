using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleBehaviour : MonoBehaviour {

	void OnClick () {
		SceneManager.LoadScene("game", LoadSceneMode.Single);
	}
}
