using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace Ball3D
{
    public class QLearningManager : MonoBehaviour
    {
        [HideInInspector] public static QLearningManager Instance { get; private set; }
        [HideInInspector] public Dictionary<(State, Action), float> QValues { get; private set; }
        private int _episodes;
        [HideInInspector] public int Episodes
        {
            get { return _episodes; }
            set
            {
                references.episodesText.text = "Episodes: " + value;
                _episodes = value;
            }
        }
        private int _steps;
        [HideInInspector] public int Steps
        {
            get { return _steps;  }
            set
            {
                references.stepsText.text = "Steps: " + value;
                _steps = value;
            }
        }

        [Header("Hyperparameters")]
        [Tooltip("Learning Rate")]
        [Range(0f, 1f)]
        public float alpha = 0.5f;
        [Tooltip("Discount Factor")]
        [Range(0f, 1f)]
        public float gamma = 0.9f;
        [Tooltip("Noise Rate")]
        [Range(0f, 1f)]
        public float epsilon = 0.3f;

        [Space(10)]

        [Header("References")]
        [SerializeField] private string outputDirectory = "/Data/";
        [SerializeField] private string outputFileName = "3DBall-QLearning";
        [SerializeField] private References references;

        private StreamWriter outputStream;

        public void CompleteEpisode(float totalReward)
        {
            if (Application.isEditor)
            {
                outputStream.WriteLine(Episodes + "," + totalReward);
                outputStream.Flush();
            }
            Episodes++;
        }

        public void UpdateAlpha()
        {
            alpha = references.alphaSlider.value;
            references.alphaText.text = alpha.ToString("F2");
        }

        public void UpdateGamma()
        {
            gamma = references.gammaSlider.value;
            references.gammaText.text = gamma.ToString("F2");
        }

        public void UpdateEpsilon()
        {
            epsilon = references.epsilonSlider.value;
            references.epsilonText.text = epsilon.ToString("F2");
        }

        public void UpdateTimeScale()
        {
            Time.timeScale = references.timeScaleSlider.value;
            references.timeScaleText.text = Time.timeScale.ToString("F2");
        }

        public void UpdateQValue(State state, Action action, float reward, State nextState)
        {
            float qValue = QValues.ContainsKey((state, action)) ? QValues[(state, action)] : 0f;
            float nextQValue = GetValue(nextState);
            float newQValue = qValue + alpha * (reward + gamma * nextQValue - qValue);
            QValues[(state, action)] = newQValue;
        }

        public float GetValue(State state)
        {
            float maxValue = float.MinValue;
            for (decimal i = 0m; i <= 1m; i += .01m)
            {
                for (decimal j = 0m; j <= 1m; j += .01m)
                {
                    Action action = new(i, j);
                    float qValue = QValues.ContainsKey((state, action)) ? QValues[(state, action)] : 0f;
                    if (qValue > maxValue)
                    {
                        maxValue = qValue;
                    }
                }
            }
            return maxValue;
        }

        public Action GetPolicy(State state)
        {
            float maxValue = float.MinValue;
            Action bestAction = new(0m, 0m);
            for (decimal i = 0m; i <= 1m; i += .01m)
            {
                for (decimal j = 0m; j <= 1m; j += .01m)
                {
                    Action action = new(i, j);
                    float qValue = QValues.ContainsKey((state, action)) ? QValues[(state, action)] : 0f;
                    if (qValue > maxValue)
                    {
                        maxValue = qValue;
                        bestAction = action;
                    }
                }
            }
            return bestAction;
        }

        public Action GetAction(State state)
        {
            // account for noise
            if (Random.value < epsilon)
            {
                return new Action(decimal.Round((decimal)Random.Range(-1f, 1f), 2),
                    decimal.Round((decimal)Random.Range(-1f, 1f), 2));
            }
            else
            {
                return GetPolicy(state);
            }
        }
            
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Initialize variables
            QValues = new();
            Episodes = 0;
            Steps = 0;
        }

        private void Start()
        {
            // Initialize UI
            references.alphaText.text = alpha.ToString("F2");
            references.alphaSlider.value = alpha;
            references.gammaText.text = gamma.ToString("F2");
            references.gammaSlider.value = gamma;
            references.epsilonText.text = epsilon.ToString("F2");
            references.epsilonSlider.value = epsilon;
            references.timeScaleText.text = Time.timeScale.ToString("F2");
            references.timeScaleSlider.value = Time.timeScale;

            SetupOutputFile();
        }

        private void Update()
        {
            if (!Application.isPlaying && Application.isEditor)
            {
                // close file
                if (outputStream != null)
                {
                    outputStream.Close();
                    outputStream = null;
                }
            }
        }

        private void SetupOutputFile()
        {
            if (!Application.isEditor)
                return;

            Directory.CreateDirectory(Application.streamingAssetsPath + outputDirectory);

            int version = 0;
            string filePath = Application.streamingAssetsPath + outputDirectory + outputFileName + "-"
                + version + ".csv";
            while (File.Exists(filePath))
            {
                version++;
                filePath = Application.streamingAssetsPath + outputDirectory + outputFileName + "-"
                    + version + ".csv";
            }

            File.WriteAllText(filePath, "Episode,Total Reward");
            outputStream = new(filePath);
        }

        [System.Serializable]
        public struct References
        {
            [Header("Learning Rate")]
            public TMP_Text alphaText;
            public Slider alphaSlider;

            [Header("Discount Factor")]
            public TMP_Text gammaText;
            public Slider gammaSlider;

            [Header("Noise Rate")]
            public TMP_Text epsilonText;
            public Slider epsilonSlider;

            [Header("Time Scale")]
            public TMP_Text timeScaleText;
            public Slider timeScaleSlider;

            [Header("Other UI")]
            public TMP_Text stepsText;
            public TMP_Text episodesText;
        }
    }

    public struct State
    {
        public Vector2 BallPosition { get; private set; }
        public Vector2 AgentRotation { get; private set; }

        public State(Vector2 ballPosition, Vector2 agentRotation)
        {
            // round values to nearest int
            BallPosition = new(Mathf.Round(ballPosition.x), Mathf.Round(ballPosition.y));
            AgentRotation = new(Mathf.Round(agentRotation.x), Mathf.Round(agentRotation.y));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            State state = (State)obj;
            return BallPosition == state.BallPosition && AgentRotation == state.AgentRotation;
        }

        public override int GetHashCode()
        {
            return BallPosition.GetHashCode() ^ AgentRotation.GetHashCode();
        }
    }

    public struct Action
    {
        public decimal X { get; private set; }
        public decimal Z { get; private set; }

        public Action(decimal x, decimal z)
        {
            // round each parameter to two decimal places
            X = decimal.Round(x, 2);
            Z = decimal.Round(z, 2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null ||  typeof(Action) != obj.GetType())
            {
                return false;
            }

            Action action = (Action)obj;
            return X == action.X && Z == action.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Z.GetHashCode();
        }
    }
}
