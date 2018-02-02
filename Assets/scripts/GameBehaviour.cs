using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameBehaviour : MonoBehaviour {

	public List<TextAsset> levels;

	public GameObject player;
	public GameObject floor;
	public GameObject coin;
	public GameObject goal;

	public GameObject recordingPanel;
	public GameObject levelNamePanel;

	public float titleFadeTime = 2.0f;
	public float titleDisplayTime = 10.0f;

	private int _level = 0;
	private CanvasGroup _titleCanvasGroup;
	private Text _levelNameText;
	private float _titleFadeElapsedTime;

	// Use this for initialization
	void Start () {
		_titleCanvasGroup = levelNamePanel.GetComponent<CanvasGroup> ();
		_levelNameText = levelNamePanel.GetComponentInChildren<Text> ();

		if (levels.Count > 0) {
			TextAsset levelText = levels [_level];
			Level.Load (levelText, player, floor, coin, goal);
			_levelNameText.text = levelText.name;
			StartCoroutine (ShowTitle ());
		} else {
			#if UNITY_EDITOR
			recordingPanel.SetActive (true);
			#endif
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator ShowTitle()
	{
		while(_titleCanvasGroup.alpha < 1)
		{
			_titleFadeElapsedTime += Time.deltaTime;
			_titleCanvasGroup.alpha = Mathf.Clamp01((_titleFadeElapsedTime / titleFadeTime));
			yield return null;
		}
		yield return new WaitForSeconds(titleDisplayTime);
		StartCoroutine(FadeOut (_titleCanvasGroup));

		yield return null;
	}

	IEnumerator FadeIn(CanvasGroup canvasGroup)
	{
		_titleFadeElapsedTime = 0;
		while(canvasGroup.alpha < 1)
		{
			_titleFadeElapsedTime += Time.deltaTime;
			canvasGroup.alpha = Mathf.Clamp01(_titleFadeElapsedTime / titleFadeTime);
			yield return null;
		}

		yield return null;
	}

	IEnumerator FadeOut(CanvasGroup canvasGroup)
	{
		_titleFadeElapsedTime = 0;
		while(canvasGroup.alpha > 0)
		{
			_titleFadeElapsedTime += Time.deltaTime;
			canvasGroup.alpha = Mathf.Clamp01(1.0f - (_titleFadeElapsedTime / titleFadeTime));
			yield return null;
		}

		yield return null;
	}
}
