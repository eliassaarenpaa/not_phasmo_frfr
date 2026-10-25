using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanitySystem
{
    public float playerSanity;

    const float sanityRegenSpeed = 1.0f;


    public void UpdateSanity(bool isPlayerOutside, float dt)
    {
        if(isPlayerOutside)
        {
            playerSanity += sanityRegenSpeed * dt;
        }
    }


}
