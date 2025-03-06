using System;
using UnityEngine;

public class PlayerSwitchEventArgs : EventArgs
{
    public KeyCode arrow;
    public PlayerSwitchEventArgs(KeyCode arrow)
    {
        this.arrow = arrow;
    }
}