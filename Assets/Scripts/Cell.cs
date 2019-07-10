using UnityEngine;

public class Cell
{
    public int X;
    public int Y;
    public int Z;
    public float Top => Z + Creeper;

    public float Creeper;
    public bool Changed;

    public Shrine Shrine;

    private Player _owner;

    public Player Owner
    {
        get { return _owner; }
        set
        {
            if (_owner != null)
            {
                _owner.Cells--;
            }

            _owner = value;
            if (_owner != null)
            {
                _owner.Cells++;
            }
        }
    }
    
    public Color Color => GetColor();

    public void Init()
    {
        Changed = true;
    }

    private Color GetColor()
    {
        var depth = Mathf.Round(Top / Field.DEPTH * 1.5f * 20f) * 0.05f;
        var color = Owner?.Color ?? Color.white;
        return color * depth;
    }
}