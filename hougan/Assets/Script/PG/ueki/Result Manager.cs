using UnityEngine;
using TMPro;

public class ResultSceneUI : MonoBehaviour
{
    public TMP_Text resultText;

    void Start()
    {
        resultText.text = $" {GameManager.FinalScore:F1}";
    }
}