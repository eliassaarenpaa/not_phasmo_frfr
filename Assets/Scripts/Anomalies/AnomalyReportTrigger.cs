using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyReportTrigger : MonoBehaviour
{
    public bool isPlayerOnTrigger;
    private void OnEnable() => AnomalySystem.Register(this);


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Player>())
        {
            isPlayerOnTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.GetComponent<Player>())
        {
            isPlayerOnTrigger = false;
        }
    }
}
