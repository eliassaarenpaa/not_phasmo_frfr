using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public MenuButton playButton;

    private void OnEnable() => MenuSystem.Register(this);

}
