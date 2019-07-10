using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field
{
    public static int WIDTH = 50;
    public static int HEIGHT = 50;
    public static int DEPTH = 20;
    public const int PLAYERS = 3;
    public static int SHRINES_PER_PLAYER = 3;
    
    public static float TAR_COEF = 0.25f;
    public static float SHRINE_SPEED = 0.5f;
    public static float CELLS_POWER = 0.01f;
    
    public Cell[,] Cells = new Cell[WIDTH,HEIGHT];
    public HashSet<Cell> CellsWithCreeper = new HashSet<Cell>();

    public HashSet<Player> Players = new HashSet<Player>();

    public void Init()
    {
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                var depth = 0;
                
                var x = Mathf.Round((float) i / 2) * 2 / WIDTH;
                var y = Mathf.Round((float) j / 2) * 2 / HEIGHT;
                depth = Mathf.RoundToInt(DEPTH * Mathf.PerlinNoise(x, y));
                
                var cell = new Cell
                {
                    X = i,
                    Y = j,
                    Z = depth
                };
                cell.Init();
                Cells[i, j] = cell;
            }
        }

        for (int i = 0; i < PLAYERS; i++)
        {
            var player = new Player();
            player.Color = i == 0 ? Color.yellow : i == 1 ? Color.green : Color.cyan;
            Players.Add(player);

            for (int j = 0; j < SHRINES_PER_PLAYER; j++)
            {
                var shrine = new Shrine();
                shrine.Speed = SHRINE_SPEED;
                player.Shrines.Add(shrine);
                
                var cell = GetRandom();
                cell.Owner = player;
                cell.Shrine = shrine;
                shrine.Cell = cell;
                
                CellsWithCreeper.Add(cell);
            }
        }
    }

    private Cell GetRandom()
    {
        var x = Random.Range(0, WIDTH);
        var y = Random.Range(0, HEIGHT);
        return Cells[x, y];
    }

    public void Step(float delta)
    {
        var changes = new List<CreeperChange>();
        foreach (var cell in CellsWithCreeper)
        {
            if (cell.Creeper <= TAR_COEF) continue;
            for (int i = 0; i < _directions.Length; i++)
            {
                var direction = _directions[i];
                var neighbour = GetCell(cell.X + direction.X, cell.Y + direction.Y);
                if (neighbour == null) continue;
                if (cell.Top <= neighbour.Top) continue;
                changes.Add(new CreeperChange
                {
                    From = cell,
                    To = neighbour
                });
                neighbour.Changed = true;
            }

            cell.Changed = true;
        }
        
        for (var i = 0; i < changes.Count; i++)
        {
            var change = changes[i];
            var from = change.From;
            var cell = change.To;

            var amount = from.Creeper / 4f;
            if (cell.Owner == from.Owner)
            {
                var diff = from.Top - cell.Top;
                amount = Mathf.Min(diff / 2f, amount);
            }
            from.Creeper -= amount;

            if (cell.Owner == null || cell.Owner == from.Owner)
            {
                cell.Creeper += amount;
            }
            else
            {
                cell.Creeper -= amount;
            }
            
            if (cell.Owner == null)
            {
                cell.Owner = from.Owner;
                CellsWithCreeper.Add(cell);
            }
            
            if (cell.Creeper > 0) continue;
            if (cell.Owner == from.Owner) continue;

            cell.Creeper *= -1f;
            cell.Owner = from.Owner;
            
            if (cell.Shrine != null)
            {
                cell.Owner.Shrines.Remove(cell.Shrine);
                from.Owner.Shrines.Add(cell.Shrine);
            }
        }
        
        foreach (Player player in Players)
        {
            foreach (var shrine in player.Shrines)
            {
                var creeperProduced = shrine.Speed * (1f + player.Cells * CELLS_POWER);
                shrine.Cell.Creeper += creeperProduced;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT) return null;
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
