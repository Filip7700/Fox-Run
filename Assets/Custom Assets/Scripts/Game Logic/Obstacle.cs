using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle: MonoBehaviour {
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Fox") {
			FoxBrain Brain = col.GetComponent<Fox>().GetBrain();
			Brain.Punish(3);
			// Debug.Log("Fox is punished...");
			// Debug.Log("Fox is kil");
		}
	}
}
