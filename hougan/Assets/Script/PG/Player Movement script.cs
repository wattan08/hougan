using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 100f;

    float xRotation = 0f;

    void Update()
    {
        Vector2 lookInput = Vector2.zero;

        // 右スティック入力
        if (Gamepad.current != null)
        {
            lookInput = Gamepad.current.rightStick.ReadValue();
        }

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}