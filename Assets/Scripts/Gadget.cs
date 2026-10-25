using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    public AudioSource beepSource;
    public TextMeshProUGUI clockValue;
    public TextMeshProUGUI treathValue;
    public TextMeshProUGUI sanityValue;


    private void OnEnable()
    {
        ClockSystem.Register(this);
        AnomalySystem.Register(this);
    }

}
