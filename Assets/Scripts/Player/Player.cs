using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;

    void OnEnable() => PlayerSystem.Register(this);
    void OnDisable() => PlayerSystem.Unregister(this);
}