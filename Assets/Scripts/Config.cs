using UnityEngine;

public class Config : MonoBehaviour 
{
    public int Width = 100;
    public int Height = 100;
    public int Depth = 20;
    public int Players = 1;
    public int CitiesPerPlayer = 1;
    public int FutureCities = 20;
    
    public float UpgradeCoef = 1f;
    public float UpgradePower = 0.5f;
    public float ScoutingCoef = 0.1f;
    public float ScoutingPower = 0.1f;
    public float CityBirthRateMultiplier = 10f;
    public float BirthRate = 0.5f;
    public float MortalityRate = 0.5f;
    public float DistinctionCoef = 0.0001f;
}