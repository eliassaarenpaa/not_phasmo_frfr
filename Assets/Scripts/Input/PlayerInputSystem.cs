using UnityEngine;
using UnityEngine.UIElements;

public struct PlayerInput
{
    public float moveX;
    public float moveZ;
    public float mouseDx;
    public float mouseDy;

    public bool isInteract;
    public bool isBack;
    public bool isZoom;
}

public class PlayerInputSystem
{
    public PlayerInput input;

    public PlayerInput ReadInputs()
    {
        Vector2 move = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));
        move = Vector2.ClampMagnitude(move, 1f);

        input.moveX = move.x;
        input.moveZ = move.y;

        input.mouseDx = Input.GetAxisRaw("Mouse X");
        input.mouseDy = Input.GetAxisRaw("Mouse Y");

        input.isInteract = Input.GetMouseButtonDown(0);
        input.isBack = Input.GetMouseButtonDown(1);

        input.isZoom = Input.GetMouseButton(1);

        return input;
    }
}