using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ReastartOrTiltle: MonoBehaviour
{
    [Header("最初に選択するボタン")]
    public GameObject firstButton;

    [Header("ボタン")]
    public Button restartButton;
    public Button titleButton;

    [Header("色設定")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    void Start()
    {
        // 初期選択（コントローラー対応）
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        UpdateButtonColor(restartButton);
        UpdateButtonColor(titleButton);
    }

    void UpdateButtonColor(Button button)
    {
        if (button == null) return;

        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        if (text == null) return;

        if (EventSystem.current.currentSelectedGameObject == button.gameObject)
        {
            text.color = selectedColor;   // ホバー中（選択中）
        }
        else
        {
            text.color = normalColor;
        }
    }

    // リスタート
    public void RestartGame()
    {
        SceneManager.LoadScene("Main Scene");
    }

    // タイトルへ
    public void BackToTitle()
    {
        SceneManager.LoadScene("title Scene");
    }
}