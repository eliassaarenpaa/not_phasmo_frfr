using UnityEngine;
using UnityEngine.UI;

public class AnomalyNextButton : MonoBehaviour
{
    public AnomalyScreenType nextScreen;

    public Button button;

    public bool clicked;

    void Awake() => button.onClick.AddListener(() => clicked = true);
}
