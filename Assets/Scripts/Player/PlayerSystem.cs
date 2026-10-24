using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerSystem
{
    public static Player player; // or a list of objects

    float nextFootstepTime;
    Vector3 lastFootstepPos;

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

    AudioClip GetRandomClip(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0)
            return null;

        return clips[UnityEngine.Random.Range(0, clips.Count)];
    }

    public void UpdateFootstepSound(bool outside)
    {
        if(player.rb.velocity.magnitude < 0.1f)
        {
            return;
        }

        if(Time.time >= nextFootstepTime && Vector3.Distance(lastFootstepPos, player.transform.position) > player.footstep_distance)
        {
            AudioClip clip = GetRandomClip(outside ? player.footsteps_outside : player.footsteps_inside);
            player.footstep_source.clip = clip;
            player.footstep_source.volume = UnityEngine.Random.Range(player.volume_min, player.volume_max);
            player.footstep_source.pitch = UnityEngine.Random.Range(player.pitch_shift_min, player.pitch_shift_max);
            player.footstep_source.Play();

            nextFootstepTime = Time.time + UnityEngine.Random.Range(player.footstep_delay_min, player.footstep_delay_max);
            lastFootstepPos = player.transform.position;
        }
    }
}