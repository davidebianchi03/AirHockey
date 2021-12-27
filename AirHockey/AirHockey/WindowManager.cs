﻿using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey
{
    class WindowManager
    {
        public UIWindow window { get; set; } = null;
        public int PageDisplayed { get; set; } = AcceptConnectionPage;//Schermata visualizzata adesso sullo schermo
        public const int UsernameInputPage = 0;//ID della schermata di inserimento dello Username
        public const int EstabishConnectionPage = 1;//ID della schermata per inviare una richiesta di connessione verso un altro host
        public const int AcceptConnectionPage = 2;//ID della schermata per accettare la richiesta di connessione proveniente da un altro host

        public WindowManager()
        {
            window = null;
        }

        /* Metodo della classe che viene eseguito come thread*/
        public void DisplayWindow()
        {
            /*
             Creazione e visualizzazione della finestra del gioco
            */

            SharedSettings settings = SharedSettings.GetInstance();
            //Creo la finestra
            VideoMode mode = new VideoMode(1000, 750);//imposto la dimensione della finestra
            window = new UIWindow(mode, "Air Hockey");
            //imposto l'icona della finestra
            Image image = new Image(settings.resourcesPath + "logo.png");
            window.SetIcon(image.Size.X, image.Size.Y, image.Pixels);
            //Aggiungo l'evento che viene richiamato quando viene premuta la x per chiudere la finestra
            window.Closed += (obj, e) => { window.Close(); };
            //creo l'oggetto che serve a disegnare e gestire la pagina per l'inserimento del proprio username
            PageSetUsername setUsernamePage = null;
            //creo l'oggetto che serve a disegnare e gestire la pagina per inviare la richiesta di connessione
            PageEstablishConnection establishConnection = null;
            //creo l'oggetto che serve a disegnare e gestire la pagina per accettare/rifiutare una richiesta di connessione
            PageAcceptConnection acceptConnection = null;

            while (window.IsOpen)//ciclo principale della finestra
            {
                window.DispatchEvents();
                window.Clear();
                
                switch (PageDisplayed)
                {
                    case UsernameInputPage:
                        //Visualizzazione della schermata per l'inserimento dello username
                        if (setUsernamePage == null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina per l'inserimento del proprio username
                            setUsernamePage = new PageSetUsername(window);
                        }
                        setUsernamePage.Draw();
                        establishConnection = null;
                        acceptConnection = null;
                        break;

                    case EstabishConnectionPage:
                        //visualizzazione della schermata per inviare una richiesta di connessione
                        if(establishConnection == null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina per inviare la richiesta di connessione
                            establishConnection = new PageEstablishConnection(window);
                        }
                        establishConnection.Draw();
                        setUsernamePage = null;
                        acceptConnection = null;
                        break;
                    case AcceptConnectionPage:
                        if(acceptConnection == null)
                        {
                            //inizializzo l'oggetto che server a disegnare e gestire la pagina per accettare/rifiutare richieste di connessione
                            acceptConnection = new PageAcceptConnection(window);
                        }
                        acceptConnection.Draw();
                        establishConnection = null;
                        setUsernamePage = null;
                        break;
                }

                window.Display();
                Thread.Sleep(33);
            }
        }
    }
}
