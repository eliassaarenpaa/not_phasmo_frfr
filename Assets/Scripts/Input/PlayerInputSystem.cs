using UnityEngine;

public struct PlayerInput
{
    public float moveX;
    public float moveZ;
    public float mouseDx;
    public float mouseDy;

    public bool isInteract;
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

        input.isInteract = Input.GetKeyDown(KeyCode.E);

        return input;
    }
}