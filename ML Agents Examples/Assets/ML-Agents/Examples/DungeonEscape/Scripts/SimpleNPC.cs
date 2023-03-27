using UnityEngine;

namespace DungeonEscape
{
    public class SimpleNPC : MonoBehaviour
    {
        public Transform target;

        public float walkSpeed = 1f;

        private Vector3 dirToGo;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            dirToGo = target.position - transform.position;
            dirToGo.y = 0;
            rb.rotation = Quaternion.LookRotation(dirToGo);
            rb.MovePosition(transform.position + Time.deltaTime * walkSpeed * transform.forward);
        }

        public void SetRandomWalkSpeed()
        {
            walkSpeed = Random.Range(1f, 7f);
        }
    }
}
