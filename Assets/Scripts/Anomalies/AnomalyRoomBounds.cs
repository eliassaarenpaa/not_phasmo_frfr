using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyRoomBounds : MonoBehaviour
{
    public RoomType room;

    public bool isPlayerInRoom;

    private void OnEnable() => AnomalySystem.Register(this);
    private void OnDisable() => AnomalySystem.Unregister(this);

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Player>())
        {
            isPlayerInRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.GetComponent<Player>())
        {
            isPlayerInRoom = false;
        }
    }
}
