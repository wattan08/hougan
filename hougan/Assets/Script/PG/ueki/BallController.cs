using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool landed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (landed)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            landed = true;

            GameManager.Instance.OnBallLanded(
                transform.position);

            //Destroy(gameObject, 1f);
        }
    }
}