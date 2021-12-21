using SFML.Graphics;
using SFML.Window;
using SFMLUIComponentsLibrary;
using System;

namespace AirHockey
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoMode mode = new VideoMode(1000, 500);
            RenderWindow window = new RenderWindow(mode, "Air Hockey");
            window.Closed += (obj, e) => { window.Close(); };

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();
                window.Display();
            }
        }
    }
}
