using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGenerator: MonoBehaviour {
	private const float MaxObstacleSpeed = 80.0f;
	private const float MinObstacleSpeed = 20.0f;

	public float obstacleSpeed;

	public Transform tree;
	public Transform rock;

	public GameObject Foxy;
	public GameObject Ground;

	public float MinObstacleCreationTick;
	public float MaxObstacleCreationTick;

	private Transform usedObstacle;
	private float obstacleTick;
	private byte randomObstacle;

	private float obstacleCreationTick;

	private GameObject foxyCopy;

	private int ObstacleAccelerationDirection = 1;

	void Start() {
		// Create foxy...
		foxyCopy = Instantiate(Foxy, this.transform, false);
		GameObject groundCopy = Instantiate(Ground, this.transform, false);

		obstacleTick = 0;
	}

	void Update() {
		randomObstacle = (byte)Random.Range(0, 2);
		obstacleTick += 1 * Time.deltaTime;

		if(randomObstacle == 0) {
			usedObstacle = tree;
		}
		else if(randomObstacle == 1) {
			usedObstacle = rock;
		}

		if(obstacleTick >= obstacleCreationTick) {
			obstacleCreationTick = (float)UnityEngine.Random.Range(MinObstacleCreationTick, MaxObstacleCreationTick);
			obstacleTick = 0;
			LevelCreation();
		}

		if(obstacleSpeed > MaxObstacleSpeed)
			ObstacleAccelerationDirection = -1;
		else if(obstacleSpeed < MinObstacleSpeed)
			ObstacleAccelerationDirection = 1;

		obstacleSpeed += 0.002f * (float)ObstacleAccelerationDirection;
	}

	void LevelCreation() {
		// Debug.Log("'And another one...' -DJ Khalid");
		Rigidbody usedObstacleRigidbody = usedObstacle.GetComponent<Rigidbody>();

		Rigidbody obstacleClone = Instantiate(usedObstacleRigidbody, this.transform, false);
		obstacleClone.transform.localPosition = new Vector3(50, 0, 0);

		obstacleClone.transform.SetParent(this.transform);

		obstacleClone.velocity = transform.TransformDirection(Vector3.left * obstacleSpeed);

		Destroy(obstacleClone.gameObject, 100 / obstacleSpeed);
	}

	public GameObject GetFox() {
		return this.foxyCopy;
	}
}
