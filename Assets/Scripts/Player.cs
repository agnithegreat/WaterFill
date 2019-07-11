using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Color Color;
    public HashSet<City> Cities = new HashSet<City>();
    public int Cells;
}