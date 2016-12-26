using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RaycastVision
{
    public class Enemy
    {
        public Vector2 Position;
        public int Health;
        public bool Drawn;
        public bool Visible;
        public int State;

        public Enemy(Vector2 pos)
        {
            Position = pos;
            Health = 100;
        }

        private int _timer;
        private int _lastHealth = 100;
        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime.Milliseconds;
            if ( Vector2.Distance(Player.Position, Raycasting.LineCast(RayVision.Grid, Player.Position, Position, false)) >= Vector2.Distance(Player.Position, Position))
            {
                Visible = true;
                if (Health <= 0)
                {
                    if (State < 3)
                        _timer = 0;

                    if (_timer < 500)
                        State = _timer/125 + 4;
                }
                else
                {
                    if (_timer > 0 && _timer <= 1000 && State != 1)
                        State = 1;
                    else if (_timer > 1000 && State != 2)
                    {
                        State = 2;
                        Player.Health -= 10;
                        RayVision.Sounds[3].Play();
                    }
                    if (_timer > 1250)
                        _timer = 0;

                    if (Health != _lastHealth)
                    {
                        _timer = -200;
                        _lastHealth = Health;
                    }
                    if (_timer < 0 && State != 0)
                        State = 8;
                }
            }
            else
            {
                Visible = false;
                if (Health > 0)
                {
                State = 0;
                _timer = -1000;
                }
            }
        }
    }
}
