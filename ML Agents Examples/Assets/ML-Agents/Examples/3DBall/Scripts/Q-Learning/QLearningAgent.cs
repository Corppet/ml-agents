using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ball3D
{
    public class QLearningAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody ballRb;

        private float ballStartHeight;

        private bool isTerminal
        {
            get
            {
                return (ballRb.position.y - transform.position.y) < -2f ||
                    Mathf.Abs(ballRb.position.x - transform.position.x) > 3f ||
                    Mathf.Abs(ballRb.position.z - transform.position.z) > 3f;
            }
        }
        private float totalReward;
        private State prevState;
        private Action prevAction;

        private void Awake()
        {
            ballStartHeight = ballRb.position.y - transform.position.y;
            totalReward = 0f;
        }

        private void FixedUpdate()
        {
            // get the agent's current state
            State state = new(new Vector2(ballRb.position.x - transform.position.x, ballRb.position.z - transform.position.z),
                new Vector2(transform.rotation.x, transform.rotation.z));

            // find and perform an action
            Action action = QLearningManager.Instance.GetAction(state);
            PerformAction(action);

            // get the reward
            float reward = .1f;
            if (isTerminal)
            {
                reward = -1f;
                NewEpisode();
            }
            else
            {
                totalReward += reward;
            }

            // update the Q-value
            QLearningManager.Instance.UpdateQValue(prevState, prevAction, reward, state);

            // update Q-Learning stats
            QLearningManager.Instance.Steps++;
            prevState = state;
            prevAction = action;
        }

        private void NewEpisode()
        {
            QLearningManager.Instance.CompleteEpisode(totalReward);

            // start at a random state
            ballRb.position = new Vector3(Random.Range(-2.25f, 2.25f) + transform.position.x,
                ballStartHeight + transform.position.y,
                Random.Range(-2.25f, 2.25f) + transform.position.z);
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;

            totalReward = 0f;
        }

        private void PerformAction(Action action)
        {
            float actionX = 2f * Mathf.Clamp((float)action.X, -1f, 1f);
            float actionZ = 2f * Mathf.Clamp((float)action.Z, -1f, 1f);

            if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
                (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
            {
                gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
            }

            if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
                (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
            {
                gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
            }
        }
    }
}
