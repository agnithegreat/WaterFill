using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StartUp : MonoBehaviour
{
    public int Width = 100;
    public int Height = 100;
    public int Depth = 20;
    public int ShrinesPerPlayer = 3;
    
    public float TarCoef = 0.25f;
    public float ShrineSpeed = 0.5f;
    public float CellsPower = 0.01f;
    
    public Image Image;
    
    private Texture2D _bitmap;
    private Sprite _sprite;
    
    public Field Field;

    private void Awake()
    {
        Random.InitState((int) DateTime.Now.Ticks);
        
        Field.WIDTH = Width;
        Field.HEIGHT = Height;
        Field.DEPTH = Depth;
        Field.SHRINES_PER_PLAYER = ShrinesPerPlayer;

        _bitmap = new Texture2D(Field.WIDTH, Field.HEIGHT, TextureFormat.RGB24, false, true);
        _bitmap.filterMode = FilterMode.Point;
        _sprite = Sprite.Create(_bitmap, new Rect(Vector2.zero, new Vector2(Field.WIDTH, Field.HEIGHT)), Vector2.zero);
        Image.sprite = _sprite;

        Field = new Field();
        Field.Init();
    }

    private void Update()
    {
        Field.TAR_COEF = TarCoef;
        Field.SHRINE_SPEED = ShrineSpeed;
        Field.CELLS_POWER = CellsPower;

        for (int i = 0; i < Field.WIDTH; i++)
        {
            for (int j = 0; j < Field.HEIGHT; j++)
            {
                var cell = Field.GetCell(i, j);
                if (cell == null) continue;
                
                if (cell.Changed)
                {
                    _bitmap.SetPixel(cell.X, cell.Y, cell.Color);
                }

                cell.Changed = false;
            }
        }
        _bitmap.Apply();
    }

    private void FixedUpdate()
    {
        Field.Step(Time.fixedDeltaTime);
    }
}