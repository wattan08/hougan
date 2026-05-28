using UnityEngine;
using System.Collections;

public class RouletteController : MonoBehaviour
{
    private bool spinning = false;

    public int Result { get; private set; }

    private float[] resultAngles =
    {
        0f,
        120f,
        240f
    };

    public IEnumerator SpinRoulette()
    {
        spinning = true;

        // 結果決定
        Result = Random.Range(0, 3);

        float targetAngle = resultAngles[Result];

        float totalRotation =
            360f * 5 + targetAngle;

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

        // 1秒後に非表示にしたい場合
        yield return new WaitForSeconds(0.5f);

        // ルーレット非表示
        gameObject.SetActive(false);
    }
}