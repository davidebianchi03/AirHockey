using SFML.Graphics;
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
        public int PageDisplayed { get; set; } = 0;  //UsernameInputPage;//Schermata visualizzata adesso sullo schermo
        public const int UsernameInputPage = 0;//ID della schermata di inserimento dello Username
        public const int EstabishConnectionPage = 1;//ID della schermata per inviare una richiesta di connessione verso un altro host
        public const int AcceptConnectionPage = 2;//ID della schermata per accettare la richiesta di connessione proveniente da un altro host
        public const int GamePage = 3;
        public const int GameFinishPage = 4;

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
            VideoMode mode = new VideoMode(1000, 900);//imposto la dimensione della finestra
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
            //creo l'oggetto che serve a disegnare e gestire la pagina per giocare
            PageGame pageGame = null;
            //creo l'oggetto che serve a disegnare e gestire la pagina successiva al gioco
            PageFinish pageFinish = null;

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
                            settings.sendAndReceive.ClearEvents();
                            setUsernamePage = new PageSetUsername(window);
                        }
                        establishConnection = null;
                        acceptConnection = null;
                        pageGame = null;
                        pageFinish = null;
                        setUsernamePage.Draw();
                        break;

                    case EstabishConnectionPage:
                        //visualizzazione della schermata per inviare una richiesta di connessione
                        if(setUsernamePage != null)
                        {
                            setUsernamePage.button.Enable = false;
                            setUsernamePage.textInput.Enable = false;
                        }
                        if(pageFinish != null)
                        {
                            pageFinish.btnRematch.Enable = false;
                            pageFinish.btnEndConnection.Enable = false;
                        }
                        if(establishConnection == null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina per inviare la richiesta di connessione
                            settings.sendAndReceive.ClearEvents();
                            establishConnection = new PageEstablishConnection(window);
                        }                     
                        setUsernamePage = null;
                        acceptConnection = null;
                        pageGame = null;
                        pageFinish = null;
                        establishConnection.Draw();
                        break;
                    case AcceptConnectionPage:
                        if (establishConnection != null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina per inviare la richiesta di connessione
                            establishConnection.btnSendRequest.Position = new SFML.System.Vector2f(0,0);//sposto il pulsante di invio richiesta perchè sa solo lui perchè si bugga
                        }                        
                        if (acceptConnection == null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina per accettare/rifiutare richieste di connessione
                            settings.sendAndReceive.ClearEvents();
                            acceptConnection = new PageAcceptConnection(window);
                        }
                        establishConnection = null;
                        setUsernamePage = null;
                        pageGame = null;
                        pageFinish = null;
                        acceptConnection.Draw();
                        break;
                    case GamePage:
                        if(acceptConnection != null)
                        {
                            acceptConnection.btnYes.Enable = false;
                            acceptConnection.btnNo.Enable = false;
                        }
                        if (establishConnection != null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina per inviare la richiesta di connessione
                            establishConnection.btnSendRequest.Position = new SFML.System.Vector2f(0, 0);//sposto il pulsante di invio richiesta perchè sa solo lui perchè si bugga
                        }
                        if (pageGame == null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina del gioco
                            settings.sendAndReceive.ClearEvents();
                            pageGame = new PageGame(window);
                        }
                        establishConnection = null;
                        setUsernamePage = null;
                        acceptConnection = null;
                        pageFinish = null;
                        pageGame.Draw();
                        break;
                    case GameFinishPage:
                        if(pageFinish == null)
                        {
                            //inizializzo l'oggetto che serve a disegnare e gestire la pagina successiva al gioco
                            settings.sendAndReceive.ClearEvents();
                            pageFinish = new PageFinish(window);
                        }

                        establishConnection = null;
                        setUsernamePage = null;
                        acceptConnection = null;
                        pageGame = null;
                        pageFinish.Draw();
                        break;
                }

                window.Display();
                Thread.Sleep(33);
            }
        }
    }
}
