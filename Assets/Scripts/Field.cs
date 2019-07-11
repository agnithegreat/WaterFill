using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field
{
    public Config Config;
    
    public Cell[,] Cells;
    public HashSet<Cell> InhabitedCells = new HashSet<Cell>();

    public HashSet<Player> Players = new HashSet<Player>();

    public void Init(Config config)
    {
        Config = config;
        
        Cells = new Cell[Config.Width, Config.Height];
        for (int i = 0; i < Config.Width; i++)
        {
            for (int j = 0; j < Config.Height; j++)
            {
                var depth = 0;
                
                var x = Mathf.Round((float) i / 2) * 2 / Config.Width;
                var y = Mathf.Round((float) j / 2) * 2 / Config.Height;
                depth = Mathf.RoundToInt(Config.Depth * Mathf.PerlinNoise(x, y));

                var cell = new Cell
                {
                    X = i,
                    Y = j,
                    Z = depth,
                    BirthRate = Config.BirthRate,
                    MortalityRate = Config.MortalityRate * (0.7f + 0.6f * depth / Config.Depth)
                };
                cell.Init();
                Cells[i, j] = cell;
            }
        }

        for (int i = 0; i < Config.Players; i++)
        {
            var player = new Player();
            player.Color = i == 0 ? Color.yellow : i == 1 ? Color.green : Color.cyan;
            Players.Add(player);

            for (int j = 0; j < Config.CitiesPerPlayer; j++)
            {
                var city = new City();
                city.BirthRateMultiplier = Config.CityBirthRateMultiplier;
                player.Cities.Add(city);
                
                var cell = GetRandom();
                cell.People = 1;
                cell.Owner = player;
                cell.City = city;
                city.Cell = cell;
                
                InhabitedCells.Add(cell);
            }
        }

        for (int i = 0; i < Config.FutureCities; i++)
        {
            var city = new City();
            city.BirthRateMultiplier = Config.CityBirthRateMultiplier;
            
            var cell = GetRandom();
            cell.City = city;
            city.Cell = cell;
        }
    }

    private Cell GetRandom()
    {
        var x = Random.Range(0, Config.Width);
        var y = Random.Range(0, Config.Height);
        return Cells[x, y];
    }

    public void Step(float delta)
    {
        var changes = new List<CreeperChange>();
        foreach (var cell in InhabitedCells)
        {
            if (cell.Owner == null) continue;
            
            var birthRate = cell.BirthRate;
            if (cell.City != null)
            {
                birthRate *= cell.City.BirthRateMultiplier;
            }
            cell.People *= 1f + birthRate - cell.MortalityRate;

            if (cell.People >= Config.UpgradeCoef)
            {
                var neighbour = GetNeighbours(cell, true).Random();
                if (neighbour != null)
                {
                    var city = new City();
                    city.BirthRateMultiplier = Config.CityBirthRateMultiplier;
                    city.Cell = neighbour;
                    neighbour.City = city;
                    cell.People *= Config.UpgradePower;
                }
            }

            if (cell.People < Config.DistinctionCoef)
            {
                cell.People = 0;
                cell.Owner = null;
                continue;
            }

            if (cell.People >= Config.ScoutingCoef)
            {
                var neighbour = GetNeighbours(cell, false).Random();
                if (neighbour == null) continue;
                
                changes.Add(new CreeperChange
                {
                    From = cell,
                    To = neighbour
                });
                neighbour.Changed = true;
    
                cell.Changed = true;
            }
        }
        
        for (var i = 0; i < changes.Count; i++)
        {
            var change = changes[i];
            var from = change.From;
            var cell = change.To;

            var amount = from.People * Config.ScoutingPower;
            from.People -= amount;

            if (cell.Owner == null || cell.Owner == from.Owner)
            {
                cell.People += amount;
            }
            else
            {
                cell.People -= amount;
            }
            
            if (cell.Owner == null)
            {
                cell.Owner = from.Owner;
                InhabitedCells.Add(cell);
            }
            
            if (cell.People > 0) continue;
            if (cell.Owner == from.Owner) continue;

            cell.People *= -1f;
            cell.Owner = from.Owner;
            
            if (cell.City != null)
            {
                cell.Owner.Cities.Remove(cell.City);
                from.Owner.Cities.Add(cell.City);
            }
        }
    }

    private List<Cell> GetNeighbours(Cell cell, bool checkCity)
    {
        var list = new List<Cell>();
        for (int i = 0; i < _directions.Length; i++)
        {
            var direction = _directions[i];
            var neighbour = GetCell(cell.X + direction.X, cell.Y + direction.Y);
            if (neighbour == null) continue;
            if (checkCity && neighbour.City != null && neighbour.Owner == cell.Owner) continue;
            list.Add(neighbour);
        }
        return list;
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= Config.Width || y < 0 || y >= Config.Height) return null;
        return Cells[x, y];
    }

    private Direction[] _directions =
    {
        new Direction(1, 0),
        new Direction(0, 1),
        new Direction(-1, 0),
        new Direction(0, -1),
    };

    struct Direction
    {
        public int X;
        public int Y;

        public Direction(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    struct CreeperChange
    {
        public Cell From;
        public Cell To;
    }
}
