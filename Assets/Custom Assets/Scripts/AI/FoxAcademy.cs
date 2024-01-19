using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAcademy: MonoBehaviour {
	public byte LevelsCount;
	public GameObject LevelObj;
	public float NextGenTimer;

	private GameObject[] FoxLevels;
	private uint Generation;
	private float TimeElapsed;

	// Start is called before the first frame update
	void Start() {
		this.FoxLevels = new GameObject[LevelsCount];
		this.Generation = 0;

		InitializeLevels();

		this.TimeElapsed = 0;
	}

	// Update is called once per frame
	void Update() {
		TimeElapsed += Time.deltaTime;
		// Debug.Log(this.TimeElapsed.ToString() + " / " + this.NextGenTimer.ToString());

		// Check if player hit save button...
		if(Input.GetKeyDown(KeyCode.F5)) {
			SaveBestFoxBrain();
			Debug.Log("Fox brain saved...");
		}

		if(NextGenTimer < TimeElapsed) {
			ExecuteArtificialSelection();
			this.Generation += 1;
			Debug.Log(this.Generation.ToString());
			this.TimeElapsed = 0;

			// Save every 50th generation
			if(this.Generation % 50 == 0) {
				SaveAllFoxBrains();
				Debug.Log("Fox brains saved...");
			}
		}
	}

	private void InitializeLevels() {
		byte i;
		for(i = 0; i < LevelsCount; i++)
			FoxLevels[i] = Instantiate(LevelObj, new Vector3(0, -75 * i, 0), Quaternion.identity);
	}

	private void ExecuteArtificialSelection() {
		Fox[] Foxes = GetFoxes();

		// Sort foxes by score (from best score to worst score)...
		byte i;
		for(i = 0; i < LevelsCount - 1; i++) {
			byte j;
			for(j = (byte)(i + 1); j < LevelsCount; j++)
				if(Foxes[i].GetBrain().GetSuccessPoints() < Foxes[j].GetBrain().GetSuccessPoints())
					SwapFoxes(Foxes, i, j);
		}

		// Select 50% of better half to replace 50% of worse half...
		byte SelectionHalf = (byte)(LevelsCount / 2);
		for(i = 0; i < SelectionHalf; i++)
			Foxes[SelectionHalf + i].SetNewBrain(Foxes[i].GetBrain());

		// Randomly mutate foxes...
		for(i = 0; i < LevelsCount; i++)
			Foxes[i].GetBrain().Mutate();

	}

	private void SwapFoxes(Fox[] Foxes, byte FirstFoxSwapIndex, byte SecondFoxSwapIndex) {
		Fox TmpFox = Foxes[FirstFoxSwapIndex];
		Foxes[FirstFoxSwapIndex] = Foxes[SecondFoxSwapIndex];
		Foxes[SecondFoxSwapIndex] = TmpFox;
	}

	private void SaveBestFoxBrain() {
		Fox[] Foxes = GetFoxes();

		// Get fox brain with best score...
		int CurrentBestScore = Foxes[0].GetBrain().GetSuccessPoints();
		uint CurrentBestScoreIndex = 0;

		uint i;
		for(i = 1; i < LevelsCount; i++) {
			int CurrentScore = Foxes[i].GetBrain().GetSuccessPoints();
			if(CurrentBestScore < CurrentScore) {
				CurrentBestScore = CurrentScore;
				CurrentBestScoreIndex = i;
			}
		}

		FoxBrainSaver Saver = new FoxBrainSaver();
		Saver.SaveFoxBrainToXML(Foxes[CurrentBestScoreIndex].GetBrain(), "FoxBrainSnapshot.XML");
	}

	private void SaveAllFoxBrains() {
		Fox[] Foxes = GetFoxes();

		FoxBrainSaver Saver = new FoxBrainSaver();

		byte i;
		for(i = 0; i < LevelsCount; i++) {
			string FileName = "FoxBrainSnapshots/FoxBrain" + i.ToString() + "Gen" + Generation.ToString() + ".XML";
			Saver.SaveFoxBrainToXML(Foxes[i].GetBrain(), FileName);
		}
	}

	private Fox[] GetFoxes() {
		Fox[] Foxes = new Fox[LevelsCount];

		byte i;
		for(i = 0; i < LevelsCount; i++) {
			Fox CurrentFox = this.FoxLevels[i].GetComponent<SimpleGenerator>().GetFox().GetComponent<Fox>();
			Foxes[i] = CurrentFox;
		}

		return Foxes;
	}

}
