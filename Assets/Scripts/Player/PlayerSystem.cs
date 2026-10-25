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

        bool grounded = Physics.SphereCast(
           player.transform.position + Vector3.up * 0.5f, 0.1f, Vector3.down,
           out RaycastHit hit, 2.0f, player.groundMask, QueryTriggerInteraction.Ignore);

        Vector3 groundNormal = Vector3.up;
        if (grounded) groundNormal = hit.normal;
        //Debug.Log(groundNormal);

        Vector3 n = groundNormal.sqrMagnitude > 0.001f ? groundNormal.normalized : Vector3.up;

        fwd.y = 0f;
        right.y = 0f;
        fwd.Normalize();
        right.Normalize();

        Vector3 wish = fwd * input.moveZ + right * input.moveX;
        wish = Vector3.ClampMagnitude(wish, 1f);

        float mag = wish.magnitude;
        if (mag > 0.0001f)
        {
            wish = Vector3.ProjectOnPlane(wish, n).normalized * mag;
        }

        Vector3 target = wish * player.speed;

        Vector3 vel = player.rb.velocity;
        float normalSpeed = Vector3.Dot(vel, n);

        player.rb.velocity = target + n * normalSpeed;
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