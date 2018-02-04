using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameBehaviour : MonoBehaviour {

	public List<TextAsset> levels;

	public GameObject player;
	public GameObject floor;
	public GameObject corner;
	public GameObject innerCorner;
	public GameObject coin;
	public GameObject goal;

	public GameObject recordingPanel;
	public GameObject levelNamePanel;

	public GameObject levelEndCanvas;
	public GameObject hudCanvas;
	public GameObject configCanvas;

	public AudioSource backgroundAudioSource;
	public AudioClip gameTune;
	public AudioClip goalTune;
	public Text scoreText;
	public Text timeText;
	public Text livesText;
	public float titleFadeTime = 2.0f;
	public float titleDisplayTime = 10.0f;
	public string levelEndMessage = "well done!";
	public string completeMessage = "thank you for playing!";
	public float levelEndFadeTime = 2.0f;
	public float levelEndDisplayTime = 10.0f;
	public float edge = -5;

	private int _level = 0;
	private CanvasGroup _titleCanvasGroup;
	private Text _levelNameText;
	private float _titleFadeElapsedTime;
	private CanvasGroup _levelEndCanvasGroup;
	private Text _levelEndText;
	private float _levelEndFadeElapsedTime;
	private bool _showFrontView = false;
	private DateTime _startTime;
	private Int32 _coins;
	private AudioSource _audioSource;
	private DateTime _pauseStart;
	private bool _levelIsLoading;
	private PlayerBehaviour _playerBehaviour;

	// Use this for initialization
	void Start () {
		_titleCanvasGroup = levelNamePanel.GetComponent<CanvasGroup> ();
		_levelEndCanvasGroup = levelEndCanvas.GetComponent<CanvasGroup> ();
		_audioSource = GetComponent<AudioSource> ();
		_levelEndText = levelEndCanvas.GetComponentInChildren<Text> ();
		_playerBehaviour = player.GetComponent<PlayerBehaviour> ();

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		if (levels.Count > 0) {
			LoadLevel ();
		} else {
			#if UNITY_EDITOR
			recordingPanel.SetActive (true);
			#endif
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (_playerBehaviour.GetLastPosition() == Vector3.zero) {
			_startTime = DateTime.Now;
		}
		if (scoreText && timeText) {
			scoreText.text = _coins.ToString ();
			TimeSpan timeElapsed = DateTime.Now - _startTime;
			timeText.text = string.Format("{0:00}:{1:00}", timeElapsed.Minutes, timeElapsed.Seconds);
		}
	}

	public void LoadLevel() {
		PlayBackgroundMusic (gameTune, true);
		hudCanvas.SetActive (true);
		configCanvas.SetActive (true);
		_showFrontView = false;
		_levelNameText = levelNamePanel.GetComponentInChildren<Text> ();
		_startTime = DateTime.Now;
		TextAsset levelText = levels [_level];
		Level.Load (levelText, player, floor, corner, innerCorner, coin, goal);
		_levelNameText.text = levelText.name;
		StartCoroutine (ShowTitle ());
	}

	public void Pause() {
		_pauseStart = DateTime.Now;
	}

	public void Play() {
		TimeSpan pauseTime = DateTime.Now - _pauseStart;
		_startTime += pauseTime;
	}

	public void PlayAudioClip(AudioClip audioClip) {
		_audioSource.clip = audioClip;
		_audioSource.Play ();
	}

	public void PlayBackgroundMusic(AudioClip audioClip, bool loop) {
		backgroundAudioSource.clip = audioClip;
		backgroundAudioSource.loop = loop;
		backgroundAudioSource.Play ();
	}

	public void SetFrontView(bool showFrontView) {
		_showFrontView = showFrontView;
	}

	public void LoadScene(string sceneName) {
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
	}

	public void EnterGoal() {
		_showFrontView = true;
		StartCoroutine(EndLevel ());
	}

	public void AddCoin() {
		_coins++;
	}

	public bool IsFrontView() {
		return _showFrontView;
	}

	IEnumerator EndLevel() {
		if (!_levelIsLoading) {
			_levelIsLoading = true;
			_levelEndFadeElapsedTime = 0;
			_level++;
			if (levels.Count > _level) {
				_levelEndText.text = levelEndMessage;
			} else {
				_levelEndText.text = completeMessage;
			}
			hudCanvas.SetActive (false);
			configCanvas.SetActive (false);
			PlayBackgroundMusic (goalTune, false);
			while (_levelEndCanvasGroup.alpha < 1) {
				_levelEndFadeElapsedTime += Time.deltaTime;
				_levelEndCanvasGroup.alpha = Mathf.Clamp01 (_levelEndFadeElapsedTime / levelEndFadeTime);
				yield return null;
			}
			yield return new WaitForSeconds (levelEndDisplayTime);
			StartCoroutine (FadeOut (_levelEndCanvasGroup));
			if (levels.Count > _level) {
				_levelEndFadeElapsedTime = 0;
				while (_levelEndCanvasGroup.alpha > 0) {
					_levelEndFadeElapsedTime += Time.deltaTime;
					_levelEndCanvasGroup.alpha = Mathf.Clamp01 (1.0f - (_levelEndFadeElapsedTime / levelEndFadeTime));
					yield return null;
				}
				LoadLevel ();
			} else {
				SceneManager.LoadScene ("title", LoadSceneMode.Single);
			}
			_levelIsLoading = false;
			yield return null;
		}
	}

	IEnumerator ShowTitle()
	{
		_titleFadeElapsedTime = 0;
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
