using UnityEngine;
using UnityEngine.InputSystem; //入力するために必要

public class RouletteController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float rotSpeed = 0; //回転速度
    void Start()
    {
        //フレームレートを60に固定
        Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void Update()
    {
        //マウスが押されら回転速度を設定する
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            this.rotSpeed = 20;
        }

        //回転速度ぶん、ルーレットを回転させる
        transform.Rotate(0, 0, this.rotSpeed);

        //ルーレットを減速させる(追加)
        this.rotSpeed *= 0.99f;

    }
}
