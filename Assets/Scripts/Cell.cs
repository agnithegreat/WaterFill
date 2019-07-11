using UnityEngine;

public class Cell
{
    public int X;
    public int Y;
    public int Z;

    public float People;
    public bool Changed;

    public float BirthRate;
    public float MortalityRate;

    public City City;

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
    
    public void Init()
    {
        Changed = true;
    }

    public Color GetColor(int depth)
    {
        if (City != null)
        {
            return Owner?.Color ?? Color.red;
        }
        var d = Mathf.Round((Z + People) / depth * 1.5f * 20f) * 0.05f;
        var color = Owner?.Color ?? Color.white;
        return color * d;
    }
}