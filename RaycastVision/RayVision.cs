using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RaycastVision
{
    public class RayVision : Game
    {
        public static Effect SimpleEffect;
        public static SoundEffect[] Sounds = new SoundEffect[4];
        public static Grid Grid;
        public static string InfoText = "";
        public static float Offset, WeapOffset, OffsetVariation = 0.005f;
        public static int WeapAnimState, LastAnim;
        private readonly GraphicsDeviceManager _graphics;
        public static GraphicsDevice Device;
        private SpriteBatch _spriteBatch;
        public SpriteFont Font;
        public Texture2D Map, UniColor;

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        public RayVision()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 720;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "RaycastVision";
            IsMouseVisible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Device = _graphics.GraphicsDevice;

            SimpleEffect = Content.Load<Effect>("effects");

            Font = Content.Load<SpriteFont>("myFont");

            ContentBase.Content = Content;
            ContentBase.LoadTextures();

            Map = Content.Load<Texture2D>("Map2");
            Grid = new Grid(Map);

            Sounds[0] = Content.Load<SoundEffect>("shot");
            Sounds[1] = Content.Load<SoundEffect>("reload");
            Sounds[2] = Content.Load<SoundEffect>("no_ammo");
            Sounds[3] = Content.Load<SoundEffect>("shot_enemy");

            UniColor = Content.Load<Texture2D>("UniColor");
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            Player.Update(gameTime);
            foreach (Enemy enemy in Grid.Enemies)
                enemy.Update(gameTime);
            foreach (Object obj in Grid.Objects)
                obj.Update();

            base.Update(gameTime);
        }

        private float _timer;
        private float lastFPS;
        protected override void Draw(GameTime gameTime)
        {
            if (_timer == 0)
            {
                Device.Clear(Color.Black);
                _spriteBatch.Begin();
                _spriteBatch.Draw(UniColor, new Rectangle(0, 0, Device.Viewport.Width, Device.Viewport.Height / 2),
                    new Color(50, 50, 50));
                _spriteBatch.Draw(UniColor,
                    new Rectangle(0, Device.Viewport.Height / 2, Device.Viewport.Width, Device.Viewport.Height / 2),
                    new Color(75, 75, 75));
                _spriteBatch.End();

                Raycasting.Cast(Grid, Device, Offset);

                Device.BlendState = BlendState.AlphaBlend;

                var weap = new Quad();
                weap.SetSquare(new Vector2(0, -0.55f - Offset + WeapOffset), new Vector2(1, 1));
                weap.Draw(Device, ContentBase.WeaponPack[WeapAnimState]);

                _spriteBatch.Begin();
                _spriteBatch.DrawString(Font, InfoText,
                    new Vector2(Device.Viewport.Width / 2 - Font.MeasureString(InfoText).X / 2, 20), Color.Red);
                _spriteBatch.DrawString(Font,
                    "Ammos : " + Player.LoadedAmmo + Environment.NewLine + "Magazines : " + Player.Magazines,
                    new Vector2(5, Device.Viewport.Height - 50), Color.Red);
                _spriteBatch.DrawString(Font,
                    "HP : " + Player.Health + "%",
                    new Vector2(Device.Viewport.Width - 105, Device.Viewport.Height - 25), Color.Red);

                _spriteBatch.DrawString(Font,
                    "FPS : " + (int)(1f/((float)gameTime.ElapsedGameTime.Milliseconds/1000f)),
                    new Vector2(5, 20), Color.Red);
                _spriteBatch.End();

                lastFPS = (int)(1f / ((float)gameTime.ElapsedGameTime.Milliseconds / 1000f));
            }
            _timer += gameTime.ElapsedGameTime.Milliseconds;
            if (_timer >= 10)
            {
                _timer = 0;
            }
            

            base.Draw(gameTime);
        }
    }
}