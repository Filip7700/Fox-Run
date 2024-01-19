using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour {
	public Button TrainModeButton;
	public Button TestModeButton;
	public Button PlayerModeButton;
	public Button PlayerVSAiButton;
	public Button QuitGameButton;

	void Start() {
		TrainModeButton.onClick.AddListener(TrainMode);
		TestModeButton.onClick.AddListener(TestMode);
		PlayerModeButton.onClick.AddListener(PlayerMode);
		PlayerVSAiButton.onClick.AddListener(PlayerVSAI);
		QuitGameButton.onClick.AddListener(QuitGame);
	}


	void TrainMode () {
		SceneManager.LoadScene("TrainModeScene", LoadSceneMode.Single);
	}

	void TestMode () {
		SceneManager.LoadScene("TestModeScene", LoadSceneMode.Single);
	}

	void PlayerMode () {
		SceneManager.LoadScene("PlayerModeScene", LoadSceneMode.Single);
	}

	void PlayerVSAI () {
		SceneManager.LoadScene("PlayerVSAIScene", LoadSceneMode.Single);
	}

	void QuitGame () {
		Application.Quit();
	}
}
