using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool hasLanded = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Šù‚É’…’nÏ‚İ‚È‚ç–³‹
        if (hasLanded)
            return;

        // GroundÚG”»’è
        if (collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;

            Vector3 landingPoint =
                collision.contacts[0].point;

            BallLanded(landingPoint);
        }
    }

    private void BallLanded(Vector3 landingPoint)
    {
        Debug.Log("’…’n");

        GameManager.Instance.OnBallLanded(landingPoint);
    }
}