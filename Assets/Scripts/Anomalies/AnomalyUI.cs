using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyUI : MonoBehaviour
{
    public Transform cameraPcLookat;
    public AnomalyReportButton reportButton;
    public List<AnomalyRoomButton> roomButtons;
    public List<AnomalyTypeButton> typeButtons;
    public List<AnomalyNextButton> nextButtons;
    public GameObject roomSelectScreen;
    public GameObject typeSelectScreen;
    public GameObject reportScreen;

    private void OnEnable() => AnomalySystem.Register(this);
}
