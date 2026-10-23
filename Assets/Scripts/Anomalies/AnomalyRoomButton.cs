using UnityEngine;
using UnityEngine.UI;

public class AnomalyRoomButton : MonoBehaviour
{
    public RoomType room;
    public Button button;

    public bool clicked;

    void Awake() => button.onClick.AddListener(() => clicked = true);

}
