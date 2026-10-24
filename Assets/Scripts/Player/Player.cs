using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<AudioClip> footsteps_inside = new List<AudioClip>();

    public List<AudioClip> footsteps_outside = new List<AudioClip>();

    public AudioSource footstep_source;
    public float footstep_delay_min, footstep_delay_max;
    public float footstep_distance;
    public float pitch_shift_min, pitch_shift_max;
    public float volume_min, volume_max;

    public Rigidbody rb;
    public float speed;

    void OnEnable() => PlayerSystem.Register(this);
    void OnDisable() => PlayerSystem.Unregister(this);
}