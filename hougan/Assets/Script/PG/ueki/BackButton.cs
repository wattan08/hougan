using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BackToTitle : MonoBehaviour
{
    [Header("最初に選択するボタン")]
    public GameObject firstButton;

    [Header("戻るボタン")]
    public Button backButton;

    [Header("文字色")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    void Start()
    {
        // 最初に選択
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        ChangeButtonColor(backButton);
    }

    void ChangeButtonColor(Button button)
    {
        if (button == null)
            return;

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText == null)
            return;

        // 選択中なら色変更
        if (EventSystem.current.currentSelectedGameObject == button.gameObject)
        {
            buttonText.color = selectedColor;
        }
        else
        {
            buttonText.color = normalColor;
        }
    }

    // タイトルへ戻る
    public void BackTitle()
    {
        SceneManager.LoadScene("title Scene");
    }
}