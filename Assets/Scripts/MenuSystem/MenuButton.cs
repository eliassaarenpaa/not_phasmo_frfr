using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI tmpro;

    public bool clicked;
    void Awake() => button.onClick.AddListener(() => clicked = true);
}
