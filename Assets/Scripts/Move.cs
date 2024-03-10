using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public MoveBase Base { get; set; }
    public int Energy { get; set; }

    public Move(MoveBase cbase)
    {
        Base = cbase;
        Energy = cbase.Energy;
    }
}
