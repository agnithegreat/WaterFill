using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StartUp : MonoBehaviour
{
    public Config Config;
    
    public Image Image;
    
    private Texture2D _bitmap;
    private Sprite _sprite;
    
    public Field Field;

    private void Awake()
    {
        Random.InitState((int) DateTime.Now.Ticks);
        
        _bitmap = new Texture2D(Config.Width, Config.Height, TextureFormat.RGB24, false, true);
        _bitmap.filterMode = FilterMode.Point;
        _sprite = Sprite.Create(_bitmap, new Rect(Vector2.zero, new Vector2(Config.Width, Config.Height)), Vector2.zero);
        Image.sprite = _sprite;

        Field = new Field();
        Field.Init(Config);
    }

    private void Update()
    {
        for (int i = 0; i < Config.Width; i++)
        {
            for (int j = 0; j < Config.Height; j++)
            {
                var cell = Field.GetCell(i, j);
                if (cell == null) continue;
                
                if (cell.Changed)
                {
                    _bitmap.SetPixel(cell.X, cell.Y, cell.GetColor(Config.Depth));
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