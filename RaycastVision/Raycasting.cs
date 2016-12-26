using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaycastVision
{
    public class Raycasting
    {
        public static bool Textured = true;

        private static BasicEffect _basicEffect;
        private static float _faceProgress, _ObjectProgress;

        public static int Side;
        public static List<ObjectQuad> Objects = new List<ObjectQuad>();
        public static float progress;

        public struct ObjectQuad
        {
            public float PositionOnScreen;
            public float Height;
            public Texture2D Texture;

            public ObjectQuad(float pos, float h, Texture2D text)
            {
                PositionOnScreen = pos;
                Height = h;
                Texture = text;
            }
        }

        public static void Cast(Grid grid, GraphicsDevice device, float offset)
        {
            //float Distance = 10;
            InitializeEffect(device);
            Thread.Sleep(5);
            var precision = 0.05f;

            Objects = new List<ObjectQuad>();
            foreach (Enemy enemies in grid.Enemies)
            {
                enemies.Drawn = false;
            }
            foreach (Object obj in grid.Objects)
            {
                obj.Drawn = false;
            }
            List<Quaternion> ObjectsHider = new List<Quaternion>();

            for (var i = Player.Rotation - Player.FOV/2 - 10; i < Player.Rotation - Player.FOV / 2; i += precision)
            {
                var x = -0.1f * (float)Math.Cos(i * 0.01745f) - 2 * (float)Math.Sin(i * 0.01745f) * 20 + Player.Position.X;
                var y = -0.1f * (float)Math.Sin(i * 0.01745f) - 2 * (float)Math.Cos(i * 0.01745f) * 20 + Player.Position.Y;
                progress = (i - Player.Rotation - Player.FOV / 2) / (Player.FOV / 2) + 1;
                var cast = LineCast(grid, Player.Position, new Vector2(x, y), true);
            }

            for (var i = Player.Rotation - Player.FOV/2; i < Player.Rotation + Player.FOV/2; i += precision)
            {
                var x = -0.1f*(float) Math.Cos(i*0.01745f) - 2*(float) Math.Sin(i*0.01745f)*20 + Player.Position.X;
                var y = -0.1f*(float) Math.Sin(i*0.01745f) - 2*(float) Math.Cos(i*0.01745f)*20 + Player.Position.Y;

                var cast = LineCast(grid, Player.Position, new Vector2(x, y), true);
                if (cast != new Vector2(-1, -1))
                {
                    progress = (i - Player.Rotation - Player.FOV/2)/(Player.FOV/2) + 1;
                    var h = 1/Vector2.Distance(Player.Position, cast);
                    var col = grid.Points[(int) cast.X, (int) cast.Y];

                    if (!Textured)
                    {
                        if (Side == 1)
                        {
                            col.R /= 2;
                            col.G /= 2;
                            col.B /= 2;
                        }

                        var points = new VertexPositionColor[2];
                        points[0] =
                            new VertexPositionColor(
                                new Vector3(progress*device.Viewport.Width/2 + 150, h*device.Viewport.Height + offset, 0),
                                col);
                        points[1] =
                            new VertexPositionColor(
                                new Vector3(progress*device.Viewport.Width/2 + 150, -h*device.Viewport.Height + offset,
                                    0), col);
                        short[] lineStripIndices = {0, 1};

                        foreach (var pass in _basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            device.DrawUserIndexedPrimitives(
                                PrimitiveType.LineStrip,
                                points,
                                0, // vertex buffer offset to add to each element of the index buffer
                                2, // number of vertices to draw
                                lineStripIndices,
                                0, // first index element to read
                                1 // number of primitives to draw
                                );
                        }
                    }

                    else if (_faceProgress > 0 && _faceProgress < 1 && Side >= 0)
                    {
                        int textIndex = 0;
                        if (col == Color.Black)
                            textIndex = 0;
                        else if (col == Color.Red)
                            textIndex = 2;
                        else if (col == new Color(128, 0, 0, 255))
                            textIndex = 4;

                        var line = new Quad();
                        line.SetLine(new Vector2(progress * device.Viewport.AspectRatio, offset), h * 2, _faceProgress, precision / 15);
                        line.Draw(device, ContentBase.WallPack[textIndex+Side]);

                        foreach (ObjectQuad Object in Objects)
                        {
                            if(progress >= Object.PositionOnScreen - Object.Height*2 && progress <= Object.PositionOnScreen + Object.Height * 2 && h > Object.Height)
                                ObjectsHider.Add(new Quaternion(progress, h, textIndex + Side, _faceProgress));
                        }
                    }
                }
            }

            Objects = Objects.OrderBy(o => o.Height).ToList();
            foreach (ObjectQuad Object in Objects)
            {
                var line = new Quad();
                line.SetSquare(new Vector2(Object.PositionOnScreen * device.Viewport.AspectRatio, offset), new Vector2(Object.Height * 4, Object.Height * 4));
                line.Draw(device, Object.Texture);

                foreach (Quaternion hider in ObjectsHider)
                {
                    var hiderLine = new Quad();
                    line.SetLine(new Vector2(hider.X * device.Viewport.AspectRatio, offset), hider.Y * 2, hider.W, precision / 15);
                    line.Draw(device, ContentBase.WallPack[(int)hider.Z]);
                }
            }
        }

        public static void InitializeEffect(GraphicsDevice device)
        {
            _basicEffect = new BasicEffect(device);
            _basicEffect.VertexColorEnabled = true;

            var worldMatrix = Matrix.CreateTranslation(device.Viewport.Width/2f - 150, device.Viewport.Height/2f - 50, 0);

            var viewMatrix = Matrix.CreateLookAt(
                new Vector3(0.0f, 0.0f, 1.0f),
                Vector3.Zero,
                Vector3.Up
                );

            var projectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                device.Viewport.Width,
                device.Viewport.Height,
                0,
                1.0f, 1000.0f);

            _basicEffect.World = worldMatrix;
            _basicEffect.View = viewMatrix;
            _basicEffect.Projection = projectionMatrix;
        }

        public static Vector2 LineCast(Grid grid, Vector2 from, Vector2 to, bool drawObjects)
        {
            var step = 0.02f;
            var dir = to - from;
            dir.Normalize();
            float MaxDist = 100;

            int a = 0;
            for (float i = 0; i < MaxDist; i += step)
            {
                a++;
                var point = from + dir*i;

                if (point.X > 0 && point.X < grid.MapSize.X - 1 && point.Y > 0 && point.Y < grid.MapSize.Y - 1)
                {
                    if (a%5 == 0 && drawObjects && progress < 0.99f)
                    {
                        a = 0;
                        var h = 1 / Vector2.Distance(Player.Position, point);
                        foreach (Enemy enemy in grid.Enemies)
                        {
                            if (enemy.Visible && enemy.Drawn == false && Vector2.Distance(enemy.Position, point) < 0.05f)
                            {
                                enemy.Drawn = true;

                                Objects.Add(new ObjectQuad(progress, h, ContentBase.GuardPack[enemy.State]));
                            }
                        }
                        foreach (Object obj in grid.Objects)
                        {
                            if (obj.Visible && obj.Drawn == false && Vector2.Distance(obj.Position, point) < 0.05)
                            {
                                obj.Drawn = true;
                                if (progress < 0.99f)
                                    Objects.Add(new ObjectQuad(progress, h, obj.Texture));
                            }
                        }  
                    }

                    if (grid.Points[(int)point.X, (int)point.Y] != Color.White)
                    {
                        Vector2 localDir = point - new Vector2((int)point.X + 0.5f, (int)point.Y + 0.5f);
                        localDir.Normalize();
                        if (localDir.X > -0.7f && localDir.X < 0.7f)
                        {
                            Side = 1;
                            _faceProgress = Math.Abs((int)point.X - point.X);
                        }
                        else if (localDir.Y > -0.7f && localDir.Y < 0.7f)
                        {
                            Side = 0;
                            _faceProgress = Math.Abs((int)point.Y - point.Y);
                        }

                        return point;
                    }
                }
            }

            return new Vector2(-1, -1);
        }

        public static float Sqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = BitConverter.ToInt32(BitConverter.GetBytes(x), 0);
            i = 0x5f3759df - (i >> 1);
            x = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
            x = x * (1.5f - xhalf * x * x);
            return 1f/x;
        }

        public static Vector2 CastObjects(Grid grid, Vector2 from, Vector2 dir)
        {
            var step = 0.1f;
            dir.Normalize();
            float MaxDist = 100;

            for (float i = 0; i < MaxDist; i += step)
            {
                var point = from + dir * i;

                if (point.X > 0 && point.X < grid.MapSize.X - 1 && point.Y > 0 && point.Y < grid.MapSize.Y - 1)
                {
                    var line = new Quad();
                    line.SetSquare(point/100, new Vector2(0.1f, 0.1f));
                    line.Draw(RayVision.Device, ContentBase.WallPack[0]);


                    foreach (Enemy enemy in grid.Enemies)
                    {
                        if (enemy.Visible && enemy.Drawn && Vector2.Distance(enemy.Position, point) < 0.5f)
                            return point;
                    }
                }
            }

            return new Vector2(-1, -1);
        }

        private static Vector2 Project(Vector2 line1, Vector2 line2, Vector2 toProject)
        {
            double m = (double)(line2.Y - line1.Y) / (line2.X - line1.X);
            double b = (double)line1.Y - (m * line1.X);

            double x = (m * toProject.Y + toProject.X - m * b) / (m * m + 1);
            double y = (m * m * toProject.Y + m * toProject.X + b) / (m * m + 1);

            return new Vector2((int)x, (int)y);
        }
    }
}