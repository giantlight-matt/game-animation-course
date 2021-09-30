using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	public Text scoreText;
	public CanvasGroup pauseUI;

	private int score = 0;
	private bool isPauseVisible;

	public IEnumerator Start()
	{
		UpdateScoreUI();
		UpdatePauseVisibility();

		FXManager.Instance.JumpToBlack();
		yield return null;
		FXManager.Instance.FadeIn(1f);
	}

	public void Update()
	{
		if (Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			TogglePause();
		}

		if (Keyboard.current.rKey.wasPressedThisFrame)
		{
			RestartCurrentSceneImmediately();
		}
	}

	public void TransitionToScene(string newSceneName, float duration)
	{
		StartCoroutine(TransitionToSceneCoroutine(newSceneName, duration));
	}

	public IEnumerator TransitionToSceneCoroutine(string newSceneName, float duration)
	{

		FXManager.Instance.FadeOut(duration);
		yield return new WaitForSeconds(duration);

		yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
	}

	public void HandleWin(string nextSceneName)
	{
		StartCoroutine(HandleWinCoroutine(nextSceneName));
	}

	public IEnumerator HandleWinCoroutine(string nextSceneName)
	{
		yield return new WaitForSeconds(2f);
		// reload to current scene
		if (string.IsNullOrEmpty(nextSceneName))
		{
			TransitionToScene(SceneManager.GetActiveScene().name, 1f);
		}
		else
		{
			TransitionToScene(nextSceneName, 1f);
		}
	}

	public void HandleLose()
	{
		StartCoroutine(HandleLoseCoroutine());
	}

	public IEnumerator HandleLoseCoroutine()
	{
		yield return new WaitForSeconds(5f);
		// reload to current scene
		RestartCurrentScene(1f);
	}

	public void RestartCurrentSceneImmediately()
	{
		RestartCurrentScene(0);
	}

	public void RestartCurrentScene(float duration)
	{
		TransitionToScene(SceneManager.GetActiveScene().name, duration);
	}

	public void GetPoints(int amount)
	{
		score += amount;
		UpdateScoreUI();
	}

	public void UpdateScoreUI()
	{
		scoreText.text = score.ToString();
	}

	public void TogglePause()
	{
		isPauseVisible = !isPauseVisible;
		UpdatePauseVisibility();
	}

	public void UpdatePauseVisibility()
	{
		if (isPauseVisible)
		{
			pauseUI.alpha = 1;
			pauseUI.gameObject.SetActive(true);
			pauseUI.blocksRaycasts = true;
			pauseUI.interactable = true;
		}
		else
		{
			pauseUI.alpha = 0;
			pauseUI.gameObject.SetActive(false);
			pauseUI.blocksRaycasts = false;
			pauseUI.interactable = false;
		}
	}

	public void QuitGame()
	{
		#if UNITY_EDITOR
		Debug.LogError("Application quitting doesn't work in the editor, but should work in desktop builds");
		#endif

		Application.Quit();
	}

}
