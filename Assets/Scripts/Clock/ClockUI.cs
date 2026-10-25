using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    public TextMeshProUGUI tmpro;

    private void OnEnable() => ClockSystem.Register(this);

}
