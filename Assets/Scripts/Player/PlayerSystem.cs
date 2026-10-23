using System;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerSystem
{
    public static Player player; // or a list of objects

    public static void Register(Player p)
    {
        player = p;
    }

    public static void Unregister(Player a)
    {
        player = null;
    }

    public void Move(PlayerInput input, Vector3 fwd, Vector3 right, float dt)
    {
        Assert.IsNotNull(player);
        if (!player) { return; }

        fwd.y = 0f;
        right.y = 0f;
        fwd.Normalize();
        right.Normalize();

        Vector3 wish = fwd * input.moveZ + right * input.moveX;
        wish = Vector3.ClampMagnitude(wish, 1f);

        Vector3 vel = player.rb.velocity;
        Vector3 target = wish * player.speed;
        
        player.rb.velocity = new Vector3(target.x, vel.y, target.z);
    }
}