using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPlatform : MonoBehaviour
{
    public PlayerMovement player;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            player.SwitchState(player.stateAir);
            player.previousUp = player.previousUp * -1;
        }
    }
}
