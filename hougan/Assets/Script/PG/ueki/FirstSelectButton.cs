using UnityEngine;
using UnityEngine.EventSystems;

public class FirstSelect : MonoBehaviour
{
    public GameObject firstButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}   