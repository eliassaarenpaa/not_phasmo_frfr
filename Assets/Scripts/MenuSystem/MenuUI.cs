using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public MenuButton playButton;

    public GameObject menuScreen;
    public GameObject winScreen;
    public GameObject loseScreen;


    private void OnEnable() => MenuSystem.Register(this);

}
