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
        private static SharedSettings instance = null;
        private string fontPath = "";//percorso del file .ttf contenente il font da utilizzare
        public string resourcesPath = "";//percorso della cartella delle risorse
        public string username { get; set; } = "";//nome utente del giocatore
        public WindowManager windowManager { get; set; } = null;

        /*
            Oggetto che contiene l'IP dell'avversario che sta richiedendo la connessione
            QUESTO OGGETTO VIENE SETTATO AD UN VALORE DIVERSO DA "" SOLO QUANDO DEVE ESSERE VISUALIZZATA LA SCHERMATA 'PageAcceptConnection'
            L'OGGETTO DEVE ESSERE IMPOSTATO NELLO STATO DIVERSO DA "" PRIMA DI VISUALIZZARE LA PAGINA
         */
        public string hostRequestorIP { get; set; } = "";

        /*
            Oggetto che contiene lo username dell'avversario che sta richiedendo la connessione
            QUESTO OGGETTO VIENE SETTATO AD UN VALORE DIVERSO DA "" SOLO QUANDO DEVE ESSERE VISUALIZZATA LA SCHERMATA 'PageAcceptConnection'
            L'OGGETTO DEVE ESSERE IMPOSTATO NELLO STATO DIVERSO DA "" PRIMA DI VISUALIZZARE LA PAGINA
         */
        public string hostRequestorUsername { get; set; } = "";

        /*
            Oggetto che serve per gestire la mia manopola    
        */
        public MyHandle MyHandle { get; set; }

        /*   Porta utilizzata dal peer per ascoltare e inviare i messaggi   */
        public int PortNumber {get;} = 2003;

        public SendAndReceive sendAndReceive { get; set; } = null;

        public OpponentHandle opponentHandle { get; set; } = null;




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
