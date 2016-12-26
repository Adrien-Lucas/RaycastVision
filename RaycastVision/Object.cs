using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaycastVision
{
    public class Object
    {
        public Vector2 Position;
        public Texture2D Texture;
        public bool Drawn;
        public bool Visible;

        public Object(Vector2 pos, Texture2D tex)
        {
            Position = pos;
            Texture = tex;
        }

        public void Update()
        {
            if (Vector2.Distance(Player.Position, Raycasting.LineCast(RayVision.Grid, Player.Position, Position, false)) >= Vector2.Distance(Player.Position, Position))
            {
                Visible = true;
            }
            else 
                Visible = false;
        }
    }
}