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
            /*
                Creazione e visualizzazione della finestra del gioco
            */

            //Creo la finestra
            VideoMode mode = new VideoMode(1000, 750);
            UIWindow window = new UIWindow(mode, "Air Hockey");
            //Aggiungo l'evento che viene richiamato quando viene premuta la x per chiudere la finestra
            window.Closed += (obj, e) => { window.Close(); };
            //creo l'oggetto che serve a disegnare e gestire la pagina per l'inserimento del proprio username
            PageSetUsername setUsernamePage = new PageSetUsername(window);

            while (window.IsOpen)//ciclo principale della finestra
            {
                window.DispatchEvents();
                window.Clear();
                setUsernamePage.Draw();
                window.Display();
                Thread.Sleep(33);
            }
        }
    }
}
