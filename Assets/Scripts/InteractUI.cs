using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    public TextMeshProUGUI tmpro;
    private void OnEnable() => InteractSystem.Register(this);

}
