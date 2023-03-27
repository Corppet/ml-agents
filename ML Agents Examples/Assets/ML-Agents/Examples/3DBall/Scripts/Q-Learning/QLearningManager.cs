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

        [HideInInspector] public int Episodes { get; set; }
        [HideInInspector] public int Steps { get; set; }

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
        [SerializeField] private string outputFilePath;
        [SerializeField] private References references;

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
