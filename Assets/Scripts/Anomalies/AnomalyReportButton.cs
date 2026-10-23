using UnityEngine;
using UnityEngine.UI;

public class AnomalyReportButton : MonoBehaviour
{
    public Button button;

    public bool clicked;
    void Awake() => button.onClick.AddListener(() => clicked = true);
}
