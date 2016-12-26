using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RaycastVision
{
    class Player
    {
        public static Vector2 Position = new Vector2(50, 50);
        public static float Rotation = 0;
        public static float FOV = 30;
        public static int Health = 100;

        public static int LoadedAmmo = 6;
        public static int Magazines = 4;

        public static float ShotTime = 500;
        public static float ReloadTime = 1000;

        public static void Reload()
        {
            Magazines--;
            LoadedAmmo = 6;
        }

        public static void Shoot()
        {
            LoadedAmmo--;
            RayVision.Sounds[0].Play();
            Vector2 dir = new Vector2();
            dir.X = 1 * (float)Math.Cos(180 - (Rotation + 10) * 0.01754f) - 1 * (float)Math.Sin( 180 - (Rotation + 10) * 0.01745f);
            dir.Y = 1 * (float)Math.Sin(180 - (Rotation + 10) * 0.01754f) + 1 * (float)Math.Cos( 180 - (Rotation + 10) * 0.01745f);

            Console.WriteLine(-dir);
            foreach (Enemy enemy in RayVision.Grid.Enemies)
            {
                Vector2 cast = Raycasting.CastObjects(RayVision.Grid, Player.Position, dir);
                if (Vector2.Distance(cast, enemy.Position) < 0.5f)
                {
                    enemy.Health -= 35;
                }
            }
        }

        private static float _shotTimer, _reloadTimer, _infoTextTimer, _keyDownDelay;
        public static void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            var direction = new Vector2();
            if (state.IsKeyDown(Keys.Z))
                direction.Y = -0.1f;
            if (state.IsKeyDown(Keys.S))
                direction.Y = 0.1f;
            if (state.IsKeyDown(Keys.D))
                Rotation += 1f;
            if (state.IsKeyDown(Keys.Q))
                Rotation -= 1f;

            if (state.IsKeyDown(Keys.OemPlus))
                FOV += 1f;
            if (state.IsKeyDown(Keys.OemMinus))
                FOV -= 1f;

            if (state.IsKeyDown(Keys.R) && _reloadTimer == 0 && LoadedAmmo == 0 && Magazines > 0)
            {
                _reloadTimer = 1;
                RayVision.Sounds[1].Play();
            }
            else if (state.IsKeyDown(Keys.R) && LoadedAmmo > 0)
            {
                RayVision.InfoText = "Don't waste a magazine !";
            }
            if (state.IsKeyDown(Keys.R) && Magazines <= 0 && _keyDownDelay == 0)
            {
                RayVision.InfoText = "You don't have any magazine";
                RayVision.Sounds[2].Play();
                _keyDownDelay = 1;
            }

            if (_reloadTimer == 0)
            {
                if (state.IsKeyDown(Keys.Space) && _shotTimer == 0 && LoadedAmmo > 0)
                {
                    _shotTimer = ShotTime / 6;
                    RayVision.WeapAnimState = 1;
                }
                else if (state.IsKeyDown(Keys.Space) && LoadedAmmo <= 0 && _keyDownDelay == 0)
                {
                    RayVision.InfoText = "You don't have any loaded ammo";
                    RayVision.Sounds[2].Play();
                    _keyDownDelay = 1;
                }
            }

            if (_shotTimer > 0)
            {

                RayVision.WeapAnimState = (int)(_shotTimer / (ShotTime / 4));
                _shotTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (RayVision.LastAnim == 1 && RayVision.LastAnim != RayVision.WeapAnimState)
                {
                    Shoot();
                }
                if (_shotTimer > ShotTime)
                {
                    _shotTimer = 0;
                    RayVision.WeapAnimState = 0;
                }
                RayVision.LastAnim = RayVision.WeapAnimState;
            }
            if (_reloadTimer > 0)
            {
                _reloadTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (_reloadTimer < ReloadTime / 2)
                    RayVision.WeapOffset -= 0.01f;
                else
                    RayVision.WeapOffset += 0.01f;

                if (_reloadTimer >= ReloadTime)
                {
                    _reloadTimer = 0;
                    RayVision.WeapOffset = 0;
                    Reload();
                }
            }
            if (_keyDownDelay > 0)
            {
                _keyDownDelay += gameTime.ElapsedGameTime.Milliseconds;
                if (_keyDownDelay > 1000)
                    _keyDownDelay = 0;
            }
            if (RayVision.InfoText.Length > 0)
            {
                _infoTextTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (_infoTextTimer > 1000)
                {
                    RayVision.InfoText = "";
                    _infoTextTimer = 0;
                }
            }

            var movement =
                new Vector2(
                    direction.X * (float)Math.Cos((360 - Rotation) * 0.01745f) -
                    direction.Y * (float)Math.Sin((360 - Rotation) * 0.01745f),
                    direction.X * (float)Math.Sin((360 - Rotation) * 0.01745f) +
                    direction.Y * (float)Math.Cos((360 - Rotation) * 0.01745f));

            if (movement != new Vector2() &&
                Vector2.Distance(Position,
                    Raycasting.LineCast(RayVision.Grid, Position, Position + movement * 100, false)) > 1)
            {
                Position += movement;
                if (RayVision.Offset > 0.05f)
                    RayVision.OffsetVariation = -0.005f;
                else if (RayVision.Offset < -0.05f)
                    RayVision.OffsetVariation = 0.005f;

                RayVision.Offset += RayVision.OffsetVariation;
            }
        }
    }
}