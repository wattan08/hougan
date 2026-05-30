using UnityEngine;
using System.Collections;

public class RouletteController : MonoBehaviour
{
    private bool spinning = false;

    public int Result { get; private set; }

    [Header("矢印")]
    public GameObject arrow;

    private float[] resultAngles =
    {
        0f,
        120f,
        240f
    };

    public IEnumerator SpinRoulette()
    {
        // ルーレットと矢印を表示
        gameObject.SetActive(true);

        if (arrow != null)
            arrow.SetActive(true);

        spinning = true;

        Result = Random.Range(0, 3);

        float targetAngle = resultAngles[Result];
        float totalRotation = 360f * 5 + targetAngle;

        float currentRotation = 0f;
        float duration = 4f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            float eased = Mathf.Lerp(
                totalRotation,
                0,
                Mathf.Pow(t, 3)
            );

            float rotationThisFrame =
                currentRotation - eased;

            transform.Rotate(0, 0, rotationThisFrame);

            currentRotation = eased;

            yield return null;
        }

        spinning = false;

        Debug.Log("ルーレット終了");

        yield return new WaitForSeconds(0.5f);

        // ルーレットと矢印を非表示
        if (arrow != null)
            arrow.SetActive(false);

        gameObject.SetActive(false);
    }
}