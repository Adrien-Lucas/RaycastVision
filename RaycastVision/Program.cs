using System;

namespace RaycastVision
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RayVision game = new RayVision())
            {
                game.Run();
            }
        }
    }
#endif
}

