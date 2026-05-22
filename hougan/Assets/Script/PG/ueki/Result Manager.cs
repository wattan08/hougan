using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ResultSceneUI : MonoBehaviour
{
    public TMP_Text resultText;

    // 表示する最大件数
    private const int MaxHistory = 3;

    void Start()
    {
        if (resultText == null)
            return;

        string text = "";

        // 今回のスコア
        text += $"{GameManager.FinalScore:F1}\n過去リザルト\n";

        // スコアを大きい順に並び替え
        List<float> sortedScores = GameManager.ScoreHistory
            .OrderByDescending(score => score)
            .ToList();

        // 表示件数
        int displayCount = Mathf.Min(MaxHistory, sortedScores.Count);

        // 上位3件表示
        for (int i = 0; i < displayCount; i++)
        {
            text += $"{i + 1} : {sortedScores[i]:F1}\n";
        }

        resultText.text = text;
    }
}