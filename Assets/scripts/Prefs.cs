using UnityEngine;

public class Prefs {
	
	public int muteSound;

	public void Load() { 
		muteSound = PlayerPrefs.GetInt("muteSound", 0);
	}

	public void Save() { 
		PlayerPrefs.SetInt("muteSound", muteSound);
	}
}