using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaycastVision
{
    class Quad
    {
        public VertexPositionTexture[] quad;

        public Quad() { }

        public Quad(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            quad = new VertexPositionTexture[6];

            quad[0].Position = new Vector3(A.X, A.Y, 0);
            quad[0].TextureCoordinate.X = 0;
            quad[0].TextureCoordinate.Y = 1;

            quad[1].Position = new Vector3(B.X, B.Y, 0);
            quad[1].TextureCoordinate.X = 0;
            quad[1].TextureCoordinate.Y = 0;

            quad[2].Position = new Vector3(D.X, D.Y, 0);
            quad[2].TextureCoordinate.X = 1;
            quad[2].TextureCoordinate.Y = 1;

            quad[3] = quad[1];

            quad[4].Position = new Vector3(C.X, C.Y, 0);
            quad[4].TextureCoordinate.X = 1;
            quad[4].TextureCoordinate.Y = 0;

            quad[5] = quad[2];
        }

        public void SetLine(Vector2 pos, float height, float progression, float width)
        {
            quad = new VertexPositionTexture[6];

            quad[0].Position = new Vector3(pos.X - width, pos.Y - height, 0);
            quad[0].TextureCoordinate.X = progression - width / 2;
            quad[0].TextureCoordinate.Y = 1;

            quad[1].Position = new Vector3(pos.X - width, pos.Y + height, 0);
            quad[1].TextureCoordinate.X = progression + width / 2;
            quad[1].TextureCoordinate.Y = 0;

            quad[2].Position = new Vector3(pos.X + width, pos.Y - height, 0);
            quad[2].TextureCoordinate.X = progression + width / 2;
            quad[2].TextureCoordinate.Y = 1;

            quad[3] = quad[1];

            quad[4].Position = new Vector3(pos.X + width, pos.Y + height, 0);
            quad[4].TextureCoordinate.X = progression + width / 2;
            quad[4].TextureCoordinate.Y = 0;

            quad[5] = quad[2];
        }

        public void SetSquare(Vector2 pos, Vector2 size)
        {
            quad = new VertexPositionTexture[6];

            quad[0].Position = new Vector3(pos.X - size.X/2, pos.Y - size.Y / 2, 0);
            quad[0].TextureCoordinate.X = 0;
            quad[0].TextureCoordinate.Y = 1;

            quad[1].Position = new Vector3(pos.X - size.X / 2, pos.Y + size.Y / 2, 0);
            quad[1].TextureCoordinate.X = 0;
            quad[1].TextureCoordinate.Y = 0;

            quad[2].Position = new Vector3(pos.X + size.X / 2, pos.Y - size.Y / 2, 0);
            quad[2].TextureCoordinate.X = 1;
            quad[2].TextureCoordinate.Y = 1;

            quad[3] = quad[1];

            quad[4].Position = new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 2, 0);
            quad[4].TextureCoordinate.X = 1;
            quad[4].TextureCoordinate.Y = 0;

            quad[5] = quad[2];
        }

        public void Draw(GraphicsDevice device, Texture2D texture)
        {
            Effect effect = RayVision.SimpleEffect;

            Matrix worldMatrix = Matrix.Identity;
            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 2.4f), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, device.Viewport.Height);
            effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(texture);
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserPrimitives(PrimitiveType.TriangleList, quad, 0, 2, VertexPositionTexture.VertexDeclaration);
            }
        }
    }
}