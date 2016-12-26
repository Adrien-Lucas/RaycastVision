using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RaycastVision
{
    class ContentBase
    {
        public static Texture2D[] WallPack, GuardPack, WeaponPack, ObjectPack;
        public static ContentManager Content;

        public static void LoadTextures()
        {
            WallPack = MakePack(6, "Wall");
            WeaponPack = MakePack(5, "Weapon");
            GuardPack = MakePack(9, "Guard");
            ObjectPack = MakePack(5, "Object");
        }

        public static Texture2D[] MakePack(int size, string prefix)
        {
            Texture2D[] pack = new Texture2D[size];
            for (int i = 1; i <= size; i++)
            {
                pack[i-1] = Content.Load<Texture2D>(prefix + i);
            }
            return pack;
        }
    }
}