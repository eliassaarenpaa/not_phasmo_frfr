using UnityEngine;
using UnityEngine.UI;

public class AnomalyTypeButton : MonoBehaviour
{
    public AnomalyType type;
    public Button button;
    public GameObject selectedTickbox;


    public bool clicked;
    void Awake() => button.onClick.AddListener(() => clicked = true);
}
