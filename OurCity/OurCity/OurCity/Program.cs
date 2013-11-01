#region Using directives
using System;
#endregion

namespace OurCity
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Starting the game
            using (OurCityGame game = new OurCityGame())
            {
                game.Run();
            }
        } // Main(args)
    } // class Program
} // namespace OurCity

