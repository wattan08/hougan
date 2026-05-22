    using UnityEngine;

    public class SimpleProjectile : MonoBehaviour
    {
        private Vector3 velocity;

        public float gravity = -9.81f;
        public bool isActive = true;

        public SimpleProjectile Init(Vector3 direction, float power)
        {
            velocity = direction.normalized * power;
            return this;
        }

        void Update()
        {
            if (!isActive) return;

            transform.position += velocity * Time.deltaTime;

            // 重力
            velocity.y += gravity * Time.deltaTime;

            // 地面（簡易）
            if (transform.position.y <= 0f)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    0f,
                    transform.position.z);

                isActive = false;

                // 着地通知（GameManagerへ）
                GameManager.Instance.OnBallLanded(transform.position);
            }
        }
    }