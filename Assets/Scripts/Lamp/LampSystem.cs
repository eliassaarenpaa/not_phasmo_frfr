using UnityEngine;

public class LampSystem 
{
    public static Lamp lamp;

    public static void Register(Lamp l)
    {
        lamp = l;
    }

    public static void Unregister(Lamp l)
    {
        lamp = null;
    }

    public static void SetLampActive(bool active)
    {
        lamp.light.enabled = active;
    }
}
