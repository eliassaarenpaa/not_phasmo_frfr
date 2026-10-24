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
        if (!lamp) return;

        if(lamp.gameObject.activeSelf == active)
        {
            return;
        }

        if (lamp.gameObject.activeSelf)
        {
            // play disable sound   
        }
        else
        {
            // play ...
        }

        lamp.gameObject.SetActive(active);
    }
}
