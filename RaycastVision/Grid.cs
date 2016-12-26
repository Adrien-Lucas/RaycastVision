using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaycastVision
{
    public class Grid
    {
        public Color[,] Points;
        public Vector2 MapSize;
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Object> Objects = new List<Object>();

        public Grid(Texture2D map)
        {
            MapSize = new Vector2(map.Width, map.Height);
            Points = new Color[(int) MapSize.X, (int) MapSize.Y];

            Color[] pixelColours = new Color[(int)(MapSize.X * MapSize.Y)];
            map.GetData<Color>(pixelColours);
            for (int x = 0; x < MapSize.X; x++)
                for (int y = 0; y < MapSize.Y; y++)
                {
                    Points[x, y] = pixelColours[((int)MapSize.Y - 1 - y) * (int)MapSize.X + x];
                    //Player starting position
                    if (Points[x, y] == Color.Blue)
                    {
                        Player.Position = new Vector2(x + 0.5f, y + 0.5f);
                        Points[x, y] = Color.White;
                    }
                    //Enemies
                    else if (Points[x, y] == Color.Green)
                    {
                        Enemies.Add(new Enemy(new Vector2(x + 0.5f, y + 0.5f)));
                        Points[x, y] = Color.White;
                    }
                    //Objects from 1 to 5
                    else if (Points[x, y] == Color.Gray)
                    {
                        Objects.Add(new Object(new Vector2(x + 0.5f, y + 0.5f), ContentBase.ObjectPack[0]));
                        Points[x, y] = Color.White;
                    }
                    else if (Points[x, y] == Color.Cyan)
                    {
                        Objects.Add(new Object(new Vector2(x + 0.5f, y + 0.5f), ContentBase.ObjectPack[1]));
                        Points[x, y] = Color.White;
                    }
                    else if (Points[x, y] == Color.Purple)
                    {
                        Objects.Add(new Object(new Vector2(x + 0.5f, y + 0.5f), ContentBase.ObjectPack[2]));
                        Points[x, y] = Color.White;
                    }
                    else if (Points[x, y] == Color.Yellow)
                    {
                        Objects.Add(new Object(new Vector2(x + 0.5f, y + 0.5f), ContentBase.ObjectPack[3]));
                        Points[x, y] = Color.White;
                    }
                    else if (Points[x, y] == new Color(255,102, 0, 255))
                    {
                        Objects.Add(new Object(new Vector2(x + 0.5f, y + 0.5f), ContentBase.ObjectPack[4]));
                        Points[x, y] = Color.White;
                    }
                }

        }
    }
}