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

    public void Move(PlayerInput input, float dt)
    {
        Assert.IsNotNull(player);
        if (!player) { return; }

        player.rb.velocity = new Vector3(player.speed * input.moveX, 0, player.speed * input.moveZ);

        //Vector3 pos = player.transform.position;
        //pos.x += dt * player.speed * input.move_input.x;
        //pos.z += dt * player.speed * input.move_input.y;
        //player.transform.position = pos;
    }
}