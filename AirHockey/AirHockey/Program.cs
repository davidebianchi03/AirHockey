using SFML.Graphics;
using SFML.Window;
using System;
using System.Threading;
using window_utilities;//versione semplificata di SFMLUIControls

namespace AirHockey
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Creo la finestra e la visualizzo */
            WindowManager windowManager = new WindowManager();
            SharedSettings settings = SharedSettings.GetInstance();
            settings.windowManager = windowManager;
            windowManager.DisplayWindow();
            //Console.WriteLine("Goodbye" + SharedSettings.GetInstance().username);
            //Console.ReadLine();
        }
    }
}
