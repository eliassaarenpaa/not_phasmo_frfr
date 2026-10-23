using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyUI : MonoBehaviour
{
    public AnomalyReportButton reportButton;
    public List<AnomalyRoomButton> roomButtons;
    public List<AnomalyTypeButton> typeButtons;

    private void OnEnable() => AnomalySystem.Register(this);
}
