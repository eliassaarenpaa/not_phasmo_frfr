using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerArea : MonoBehaviour
{
    public bool isPlayerOnTrigger;

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
