using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Color Color;
    public HashSet<Shrine> Shrines = new HashSet<Shrine>();
    public int Cells;
}