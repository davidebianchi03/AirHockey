using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AirHockey
{
    class SharedSettings
    {
        /*
            Classe che contiene tutti le impostazioni condivise tra le varie classi (si tratta di un singleton)
        */

        public Font font { get; set; } = null;
        private static SharedSettings instance;
        private string fontPath = "";//percorso del file .ttf contenente il font da utilizzare
        public string resourcesPath = "";//percorso della cartella delle risorse

        private SharedSettings()
        {
            resourcesPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString() + "\\resources\\";
            fontPath = resourcesPath  + "RetroGaming.ttf";
            font = new Font(fontPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static SharedSettings GetInstance()//metodo per ottenere l'istanza dell'oggetto
        {
            if (instance == null)
            {
                instance = new SharedSettings();
            }
            return instance;
        }

    }
}
