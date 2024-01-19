using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;

public class FoxBrain {
	// Attributes
	private const byte NumberOfLayers = 4;

	public byte[] Layers;
	public float[][] Neurons;
	public float[][][] Weights;

	public int SuccessPoints;

	// Constructors

	// Default Constructor...
	public FoxBrain() {
		this.Layers = new byte[NumberOfLayers];

		this.SuccessPoints = 0;

		// Manually construct layers in neural network...

		// Two input layers will handle distance from detected obstacle and what that obstacle is...
		// Obstacele can be either rock or tree. Rock is shorter than tree and lower jump is enough, and tree requires longer jump...
		this.Layers[0] = 3;

		this.Layers[1] = 10;
		this.Layers[2] = 10;

		// Output layer is deciding whether the fox should jump or not...
		this.Layers[3] = 1;

		InitializeNeurons();
		InitializeWeights();
	}

	// Parameter Constructor...
	// Constructs deep copy from original fox brain passed as parameter...
	public FoxBrain(FoxBrain Original) {
		this.SuccessPoints = Original.SuccessPoints;
		this.Layers = new byte[Original.Layers.Length];

		uint i;
		for(i = 0; i < Original.Layers.Length; i++)
			this.Layers[i] = Original.Layers[i];

		InitializeNeurons();
		InitializeWeights();
		DeepCopyWeights(Original.Weights);
	}

	// Parameter Constructor
	// Constructs fox brain from XML file
	public FoxBrain(string XMLFileName) {
		Debug.Log("Loading XML file...");
		XmlDocument FoxBrainSnapshot = new XmlDocument();
		FoxBrainSnapshot.Load(XMLFileName);

		// Initialize layers
		this.Layers = new byte[NumberOfLayers];

		this.Layers[0] = 3;

		this.Layers[1] = 10;
		this.Layers[2] = 10;

		this.Layers[3] = 1;

		InitializeNeurons();
		InitializeWeights();

		// Load neurons
		XmlNode NeuronsNodes = FoxBrainSnapshot.DocumentElement.SelectSingleNode("/FoxBrain/Neurons");

		byte i = 0;

		// Level 1 loop - loop through every <ArrayOfFloat> inside <Neurons>
		foreach(XmlNode NeuronNodeDimension1 in NeuronsNodes) {
			byte j = 0;

			// Level 2 loop - loop through every <float> inside <ArrayOfFloat>
			foreach(XmlNode NeuronNodeDimension2 in NeuronNodeDimension1) {
				this.Neurons[i][j] = float.Parse(NeuronNodeDimension2.InnerText, CultureInfo.InvariantCulture.NumberFormat);
				Debug.Log(float.Parse(NeuronNodeDimension2.InnerText, CultureInfo.InvariantCulture.NumberFormat));
				j += 1;
			}

			i += 1;
		}

		// Load weights
		XmlNode WeightsNodes = FoxBrainSnapshot.DocumentElement.SelectSingleNode("/FoxBrain/Weights");

		i = 0;

		// Level 1 loop - loop through every <ArrayOfArrayOfFloat> inside <Weights>
		foreach(XmlNode WeightNodeDimension1 in WeightsNodes) {
			byte j = 0;

			// Level 2 loop - loop through every <ArrayOfFloat> inside <ArrayOfArrayOfFloat>
			foreach(XmlNode WeightNodeDimension2 in WeightNodeDimension1) {
				byte k = 0;

				// Level 3 loop - loop through every <float> inside <ArrayOfFloat>
				foreach(XmlNode WeightNodeDimension3 in WeightNodeDimension2) {
					this.Weights[i][j][k] = float.Parse(WeightNodeDimension3.InnerText, CultureInfo.InvariantCulture.NumberFormat);
					Debug.Log(float.Parse(WeightNodeDimension3.InnerText, CultureInfo.InvariantCulture.NumberFormat));

					k += 1;
				}

				j += 1;
			}

			i += 1;
		}

		// Finally read success points
		XmlNode SuccessPointsNode = FoxBrainSnapshot.DocumentElement.SelectSingleNode("/FoxBrain/SuccessPoints");
		this.SuccessPoints = int.Parse(SuccessPointsNode.InnerText);
	}

	// Methods
	private void InitializeNeurons() {
		List<float[]> NeuronsList = new List<float[]>();

		// Run through all layers...
		uint i;
		for(i = 0; i < NumberOfLayers; i++)
			NeuronsList.Add(new float[Layers[i]]);

		// Convert list to array...
		Neurons = NeuronsList.ToArray();
	}

	private void InitializeWeights() {
		List<float[][]> WeightsList = new List<float[][]>();

		uint i;
		for(i = 1; i < NumberOfLayers; i++) {
			List<float[]> LayerWeightsList = new List<float[]>();

			byte NeuronsInPreviousLayer = Layers[i - 1];

			// Loop over all neurons in current layer...
			uint j;
			for(j = 0; j < Neurons[i].Length; j++) {
				float[] NeuronWeights = new float[NeuronsInPreviousLayer];

				// Loop over every single neuron in the previous layer...
				// Set the weights randomly between -0.5 and 0.5...
				uint k;
				for(k = 0; k < Neurons[i - 1].Length; k++)
					NeuronWeights[k] = (float)UnityEngine.Random.Range(0.5f, 0.5f);

				LayerWeightsList.Add(NeuronWeights);
			}

			WeightsList.Add(LayerWeightsList.ToArray());
		}

		this.Weights = WeightsList.ToArray();
	}

	private void DeepCopyWeights(float[][][] OriginalWeights) {
		uint i;
		for(i = 0; i < this.Weights.Length; i++) {
			uint j;
			for(j = 0; j < Weights[i].Length; j++) {
				uint k;
				for(k = 0; k < Weights[i][j].Length; k++)
					this.Weights[i][j][k] = OriginalWeights[i][j][k];
			}
		}
	}

	public float[] FeedForward(float[] Inputs) {
		const float BiasValue = 0.25f;

		uint i;
		for(i = 0; i < Inputs.Length; i++)
			Neurons[0][i] = Inputs[i];

		// Loop over every single layer starting from first hidden layer (input layer is excluded)...
		for(i = 1; i < Layers.Length; i++) {
			// Loop over every single neuron in current layer...
			uint j;
			for(j = 0; j < Neurons[i].Length; j++) {
				// Loop over every single neuron in the previous layer...
				// Some neurons at the current layer are connected to the neurons of previous one...
				float NewNeuronValue = BiasValue;
				uint k;
				for(k = 0; k < Neurons[i - 1].Length; k++) {
					// Compute new values for neuron...
					NewNeuronValue += Weights[i - 1][j][k] * Neurons[i - 1][k];
				}

				// Convert new neuron value between -1 and 1 using tanh activation function...
				Neurons[i][j] = (float)Math.Tanh(NewNeuronValue);
			}
		}

		// Return the neurons in output layer...
		return Neurons[Neurons.Length - 1];
	}

	// Mutates neural network weights by chance...
	public void Mutate() {
		uint i;
		for(i = 0; i < Weights.Length; i++) {
			uint j;
			for(j = 0; j < Weights[i].Length; j++) {
				uint k;
				for(k = 0; k < Weights[i][j].Length; k++) {
					float NewWeight = Weights[i][j][k];

					// Mutate weight value...
					float MutationChance = (float)UnityEngine.Random.Range(0.0f, 1.0f);

					if(MutationChance < 0.002f) {
						// Flip sign of new weight...
						NewWeight *= -1.0f;
					}
					else if(MutationChance < 0.004f) {
						// Pick random number from -1 and 1...
						NewWeight = (float)UnityEngine.Random.Range(-1.0f, 1.0f);
					}
					else if(MutationChance < 0.006f) {
						// Randomly increase between 0% and 100%...
						float MutationFactor = UnityEngine.Random.Range(0.0f, 1.0f) + 1.0f;
						NewWeight *= MutationFactor;
					}
					else if(MutationChance < 0.008f) {
						// Randomly decrease between 0% and 100%...
						float MutationFactor = UnityEngine.Random.Range(0.0f, 1.0f);
						NewWeight *= MutationFactor;
					}

					Weights[i][j][k] = NewWeight;
				}
			}
		}
	}

	public void Reward() {
		SuccessPoints += 1;
	}

	public void Punish(byte PunishPoints) {
		SuccessPoints -= PunishPoints;
	}

	public int GetSuccessPoints() {
		return SuccessPoints;
	}
}
