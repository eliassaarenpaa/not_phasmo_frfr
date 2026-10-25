using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem
{
    static InteractUI ui;

    public static void Register(InteractUI _ui)
    {
        ui = _ui;
    }

    public static void SetInteractText(string text = "")
    {
        ui.tmpro.text = text;
    }

    public void Update(bool isOnDoor, bool isOnReport, bool isReporting, bool isWaitingForResult)
    {
        ui.gameObject.SetActive(isOnDoor || isOnReport && !isReporting || isReporting && !isWaitingForResult);

        if (isOnDoor)
        {
            ui.tmpro.text = "Open Door (Left Mouse)";
        }
        if (isOnReport && !isReporting)
        {
            ui.tmpro.text = "Report Anomaly (Left Mouse)";
        }
        if (isReporting && !isWaitingForResult)
        {
            ui.tmpro.text = "Return (Right Mouse)";
        }

    }

}
