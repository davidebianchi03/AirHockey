using AirHockey.window_utilities;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey
{
    class PageFinish
    {
        /*
            Classe che serve per gestire e disegnare la scherma successiva alla partita 
        */
        private RenderWindow parentWindow;
        private const int borderWidth = 600;
        private const int borderHeight = 600;
        public UIButton btnRematch;
        public UIButton btnEndConnection;
        private bool RematchResponseReceived;
        private Thread showMessageThread;
        public PageFinish(RenderWindow parentWindow)
        {
            float windowCenterX = parentWindow.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = parentWindow.Size.Y / 2;//centro della finestra sull'asse delle Y
            this.parentWindow = parentWindow;
            SharedSettings settings = SharedSettings.GetInstance();
            //inizializzo il pulsante di rematch
            btnRematch = new UIButton(parentWindow, "Rematch", settings.font);
            btnRematch.BorderThickness = 2;
            btnRematch.BorderColor = Color.White;
            btnRematch.textColor = Color.White;
            btnRematch.FillColor = Color.Black;
            btnRematch.Size = new SFML.System.Vector2f(200, 50);//-> da sistemare
            btnRematch.Position = new SFML.System.Vector2f(windowCenterX - (btnRematch.Size.X / 2), windowCenterY);//-> da sistemare
            btnRematch.textSize = 18;
            btnRematch.ButtonPressed += ButtonRematchPressedCallback;
            //inizializzo il pulsante di chiusura connessione
            btnEndConnection = new UIButton(parentWindow, "Termina connessione", settings.font);
            btnEndConnection.BorderThickness = 2;
            btnEndConnection.BorderColor = Color.White;
            btnEndConnection.textColor = Color.White;
            btnEndConnection.FillColor = Color.Black;
            btnEndConnection.Size = new SFML.System.Vector2f(300, 50);//-> da sistemare
            btnEndConnection.Position = new SFML.System.Vector2f(windowCenterX - (btnEndConnection.Size.X / 2), windowCenterY + 100);//-> da sistemare
            btnEndConnection.textSize = 18;
            btnEndConnection.ButtonPressed += ButtonCloseConnPressedCallback;
            settings.sendAndReceive.MessageReceived += MessageReceivedCallback;
            RematchResponseReceived = false;
        }

        /* Metodo richiamato quando viene ricevuto un messaggio */
        private void MessageReceivedCallback(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if (settings.Connection != null && e.message.Command == "e" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
            {
                //concludo la connessione
                settings.Connection = null;
                //visualizzo un messagggio
                VideoMode msgMode = new VideoMode(500, 150);
                UIMessageBox messageBox = new UIMessageBox(msgMode, "Connessione interrotta", "L'altro host ha interrotto la connessione", parentWindow, settings.font);
                messageBox.Show();
                //visualizzo la pagina per stabilire la connessione
                settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
                if(showMessageThread != null)
                {
                    showMessageThread.Abort();
                    showMessageThread = null;
                }
            }
            else if(settings.Connection != null && e.message.Command == "r" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
            {
                //comando rematch
                VideoMode msgMode = new VideoMode(500, 150);
                UIAcceptDiscardBox testUI = new UIAcceptDiscardBox(msgMode, "", "Vuoi accettare", parentWindow, settings.font);
                testUI.Show();
                Message responseMsg = new Message();
                responseMsg.Body = "";
                responseMsg.destinationIP = settings.Connection.OpponentIP;
                if(testUI.getResponseCode() == UIAcceptDiscardBox.Ok)
                {
                    responseMsg.Command = "y";
                    //se la risposta è positiva vado alla pagina del gioco
                    settings.Connection.myPoints = 0;
                    settings.Connection.OpponentsPoints = 0;
                    settings.windowManager.PageDisplayed = WindowManager.GamePage;
                }
                else
                {
                    responseMsg.Command = "e";
                    //se la risposta è negativa vado alla pagina dove si stabilisce la connessione
                    settings.Connection = null;
                    settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
                }
                settings.sendAndReceive.SendMessage(responseMsg);
            }
            else if (settings.Connection != null && e.message.Command == "y" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
            {
                RematchResponseReceived = true;
            }
        }

        /*   Evento richiamato quando viene premuto il pulsante per chiudere la connessione   */
        private void ButtonCloseConnPressedCallback(object sender, EventArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if(settings.Connection != null)
            {
                //invio il messaggio di chiusura connessione
                Message closeConnMsg = new Message();
                closeConnMsg.Command = "e";
                closeConnMsg.Body = "";
                closeConnMsg.destinationIP = settings.Connection.OpponentIP;
                settings.sendAndReceive.SendMessage(closeConnMsg);
            }
            
            //cambio pagina
            settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
        }

        /*   Metodo richiamato quando viene premuto il pulsante di rematch   */
        private void ButtonRematchPressedCallback(object sender, EventArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if(settings.Connection != null)
            {
                //invio il messaggio di rematch
                Message rematchMsg = new Message();
                rematchMsg.Command = "r";
                rematchMsg.Body = "";
                rematchMsg.destinationIP = settings.Connection.OpponentIP;
                settings.sendAndReceive.SendMessage(rematchMsg);
                //aspetto per la risposta
                RematchResponseReceived = false;
                VideoMode msgMode = new VideoMode(500, 150);
                UIMessageBox messageWait = new UIMessageBox(msgMode, "Rematch", "Waiting...", parentWindow, settings.font);
                showMessageThread = new Thread(delegate ()
                {
                    messageWait.Show();
                });
                while(showMessageThread != null && !RematchResponseReceived)
                {
                    if (messageWait.IsOpen)
                    {
                        messageWait.Show();
                    }
                    Thread.Sleep(10);
                }
                messageWait.IsOpen = false;

                if (RematchResponseReceived)
                {
                    //se la risposta è positiva vado alla pagina del gioco
                    settings.Connection.myPoints = 0;
                    settings.Connection.OpponentsPoints = 0;
                    settings.windowManager.PageDisplayed = WindowManager.GamePage;
                }
                else
                {
                    //se la risposta è negativa vado alla pagina dove si stabilisce la connessione
                    settings.Connection = null;
                    settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
                }
            }
        }

        /* Metodo per disegnare la pagina */
        public void Draw()
        {
            SharedSettings settings = SharedSettings.GetInstance();

            float windowCenterX = parentWindow.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = parentWindow.Size.Y / 2;//centro della finestra sull'asse delle Y
            /*  disegno il rettangolo di contorno di tutto */

            /*  disegno il bordo che contiene tutto */
            RectangleShape border = new RectangleShape();
            border.Size = new SFML.System.Vector2f(borderWidth, borderHeight);
            border.Position = new SFML.System.Vector2f(windowCenterX - (borderWidth / 2), windowCenterY - (borderHeight / 2));
            border.OutlineColor = Color.White;
            border.OutlineThickness = 4;
            border.FillColor = Color.Black;
            parentWindow.Draw(border);
            /*  disegno il titolo   */
            if (settings.Connection != null && settings.Connection.myPoints > settings.Connection.OpponentsPoints)
            {
                Text title = new Text("Hai vinto", settings.font);
                title.FillColor = Color.White;
                title.CharacterSize = 40;
                title.Position = new SFML.System.Vector2f(windowCenterX - (title.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 50);
                parentWindow.Draw(title);
            }
            else
            {
                Text title = new Text("Hai perso", settings.font);
                title.FillColor = Color.White;
                title.CharacterSize = 40;
                title.Position = new SFML.System.Vector2f(windowCenterX - (title.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 50);
                parentWindow.Draw(title);
            }
            //disegno il pulsante di rematch
            btnRematch.draw();
            //disegno il pulsante di fine connessione
            btnEndConnection.draw();
        }
    }
}
