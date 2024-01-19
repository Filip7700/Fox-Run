using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fox: MonoBehaviour {
	// Atributes
	public Animator foxAnimator;
	public Rigidbody foxRigidbody;
	public TextMesh scoreText;

	public bool IsTraining;

	public float jumpForce;

	public Transform CollidingObject;

	private bool isOverObstacle;

	private FoxBrain Brain;

	void Start() {
		foxRigidbody = transform.GetComponent<Rigidbody>();
		foxAnimator = transform.GetComponentInChildren<Animator>();

		if(IsTraining)
			this.Brain = new FoxBrain();
		else
			this.Brain = new FoxBrain("FoxBrainSnapshot.XML");
	}

	void Update() {
		RaycastHit hit;
		RaycastHit FoxEyes;

		scoreText.text = "Score: " + Brain.GetSuccessPoints().ToString();

		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out FoxEyes)) {
			float[] ObstacleInput = new float[3];
			ObstacleInput[0] = -1.0f * Vector3.Distance(transform.position, FoxEyes.point);

			// Debug.Log(ObstacleInput[0].ToString());

			// Check how long is obstacle
			ObstacleInput[1] = FoxEyes.transform.gameObject.GetComponent<BoxCollider>().size.y;

			// Level speed also affects how successfull the fox would be
			ObstacleInput[2] = transform.gameObject.GetComponentInParent<SimpleGenerator>().obstacleSpeed;

			// Debug.Log(ObstacleInput.ToString());

			float[] FoxChoice = Brain.FeedForward(ObstacleInput);

			// Fox has decided to jump...
			if(FoxChoice[0] > 0 && transform.localPosition.y <= 1.1)
				foxRigidbody.AddForce(transform.up * jumpForce * FoxChoice[0], ForceMode.Impulse);
				//foxAnimator.SetTrigger("Jump");
		}

		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit)) {
			CollidingObject = hit.transform;
			if(hit.transform.gameObject.tag == "Obstacle" && isOverObstacle == false) {
				this.Brain.Reward();
				isOverObstacle = true;
			}
			else if(hit.transform.gameObject.tag != "Obstacle" && isOverObstacle == true) {
				isOverObstacle = false;
			}
		}
	}

	public FoxBrain GetBrain() {
		return this.Brain;
	}

	public void SetNewBrain(FoxBrain NewBrain) {
		this.Brain = new FoxBrain(NewBrain);
	}
}
