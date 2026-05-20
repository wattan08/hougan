using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("最初に選択するボタン")]
    public GameObject firstButton;

    [Header("ボタン")]
    public Button startButton;
    public Button optionButton;
    public Button quitButton;

    [Header("文字色")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        ChangeButtonColor(startButton);
        ChangeButtonColor(optionButton);
        ChangeButtonColor(quitButton);
    }

    void ChangeButtonColor(Button button)
    {
        if (button == null)
            return;

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText == null)
            return;

        if (EventSystem.current.currentSelectedGameObject == button.gameObject)
        {
            buttonText.color = selectedColor;
        }
        else
        {
            buttonText.color = normalColor;
        }
    }

    // ゲーム開始
    public void StartGame()
    {
        SceneManager.LoadScene("Main scene");
    }

    // オプション画面へ
    public void OpenOption()
    {
        SceneManager.LoadScene("SettingScene");
    }

    // ゲーム終了
    public void QuitGame()
    {
        Debug.Log("ゲーム終了");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}