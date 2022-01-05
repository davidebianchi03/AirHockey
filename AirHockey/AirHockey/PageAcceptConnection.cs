using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey
{
    class PageAcceptConnection
    {
        private RenderWindow window = null;//finestra su cui disegnare tutto
        private UIButton btnYes = null;//pulsante per accettare la richiesta
        private UIButton btnNo = null;//pulsante per rifiutare la richiesta
        private Text txtOpponent = null;//oggetto text in cui viene inserito l'ip dell'altro host e lo username dell'avversario
        private const int borderWidth = 600;
        private const int borderHeight = 600;

        public PageAcceptConnection(RenderWindow window)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            this.window = window;
            float windowCenterX = window.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = window.Size.Y / 2;//centro della finestra sull'asse delle Y
            txtOpponent = new Text("", settings.font);
            txtOpponent.DisplayedString = settings.hostRequestorUsername + " (" + settings.hostRequestorIP + ")";
            txtOpponent.CharacterSize = 18;
            txtOpponent.FillColor = Color.White;
            /* Creo il pulsante yes */
            btnYes = new UIButton(window, settings.font);
            btnYes.content = "SI";
            btnYes.BorderThickness = 2;
            btnYes.BorderColor = Color.White;
            btnYes.FillColor = Color.Black;
            btnYes.textColor = Color.White;
            btnYes.textSize = 20;
            btnYes.Size = new SFML.System.Vector2f(75, 50);
            btnYes.Position = new SFML.System.Vector2f(windowCenterX - btnYes.Size.X - 25, windowCenterY - (borderHeight / 2) + 480);
            btnYes.ButtonPressed += BtnYesCallback;
            /* Creo il pulsante no */
            btnNo = new UIButton(window, settings.font);
            btnNo.content = "NO";
            btnNo.BorderThickness = 2;
            btnNo.BorderColor = Color.White;
            btnNo.FillColor = Color.Black;
            btnNo.textColor = Color.White;
            btnNo.textSize = 20;
            btnNo.Size = new SFML.System.Vector2f(75, 50);
            btnNo.Position = new SFML.System.Vector2f(windowCenterX + 25, windowCenterY - (borderHeight / 2) + 480);
            btnNo.ButtonPressed += BtnNoCallback;
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            sendAndReceive.MessageReceived += MessageReceived;
        }

        private void BtnNoCallback(object sender, EventArgs e)
        {
            //Rifiuto la connessione
            //rispondo con la lettera n;
            SharedSettings settings = SharedSettings.GetInstance();
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            try
            {
                Message response = new Message();
                response.Command = "n";
                response.Body = "";
                response.destinationIP = IPAddress.Parse(settings.hostRequestorIP);
                sendAndReceive.SendMessage(response);
            }
            catch (Exception ex) { }

            settings.hostRequestorUsername = "";
            settings.hostRequestorIP = "";
            settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
        }

        private void BtnYesCallback(object sender, EventArgs e)
        {
            //Accetto la connessione
            //rispondo con la lettera y e il mio username
            Console.WriteLine("Y");
            SharedSettings settings = SharedSettings.GetInstance();
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            try
            {
                Message response = new Message();
                response.Command = "y";
                response.Body = settings.username;
                response.destinationIP = IPAddress.Parse(settings.hostRequestorIP);
                sendAndReceive.SendMessage(response);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }

            lastHandShakeMessageReceived = false;
            //aspetto fino a quando non ricevo una risposta da parte dell'altro host (handshake a 3 vie)
            while (lastHandShakeMessageReceived == false)
            {
                Thread.Sleep(10);
            }
            //se è positiva procedo
            if (lastHandShakeMessage.Command == "y")
            {

                //Inserisco i dati della connessione nell'oggetto connection
                settings.Connection = new Connection();
                settings.Connection.OpponentUsername = settings.hostRequestorUsername;
                settings.Connection.OpponentIP = IPAddress.Parse(settings.hostRequestorIP);
                //cancello i dati temporanei
                settings.hostRequestorUsername = "";
                settings.hostRequestorIP = "";
                //Visualizzo la schermata di gioco
                settings.windowManager.PageDisplayed = WindowManager.GamePage;
            }
            else
            {
                //se è negativa è come se avessi rifiutato io
                settings.hostRequestorUsername = "";
                settings.hostRequestorIP = "";
                //Visualizzo la schermata precedente
                settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
            }
        }

        Message lastHandShakeMessage;
        bool lastHandShakeMessageReceived;
        private void MessageReceived(object sender, MessageReceivedArgs e)
        {

            Console.WriteLine(e.message.Command);
            lastHandShakeMessage = e.message;
            lastHandShakeMessageReceived = true;
        }

        public void Draw()
        {
            SharedSettings settings = SharedSettings.GetInstance();
            float windowCenterX = window.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = window.Size.Y / 2;//centro della finestra sull'asse delle Y
            /*  disegno il rettangolo di contorno di tutto */

            /*  disegno il bordo che contiene tutto */
            RectangleShape border = new RectangleShape();
            border.Size = new SFML.System.Vector2f(borderWidth, borderHeight);
            border.Position = new SFML.System.Vector2f(windowCenterX - (borderWidth / 2), windowCenterY - (borderHeight / 2));
            border.OutlineColor = Color.Yellow;//Coloro il bordo di giallo per evidenziare che si tratta della finestra di richiesta di connessione
            border.OutlineThickness = 4;
            border.FillColor = Color.Black;
            window.Draw(border);
            /*  disegno il titolo   */
            Text title = new Text("Air Hockey", settings.font);
            title.FillColor = Color.White;
            title.CharacterSize = 40;
            title.Position = new SFML.System.Vector2f(windowCenterX - (title.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 50);
            window.Draw(title);
            /*  disegno il logo */
            //creo la texture
            Texture textureLogo = new Texture(settings.resourcesPath + "logo.png");
            Sprite sprite = new Sprite(textureLogo);
            sprite.Scale = new SFML.System.Vector2f(0.30f, 0.30f);//dimensione scalata dell'immagine
            sprite.Position = new SFML.System.Vector2f(windowCenterX - (sprite.GetGlobalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 150);
            window.Draw(sprite);
            /*  disegno la textbox per l'inserimento dello username   */
            //disegno la label della textbox
            Text txtDescription = new Text("Hai ricevuto una richiesta di connessione da:", settings.font);
            txtDescription.FillColor = Color.White;
            txtDescription.CharacterSize = 18;
            txtDescription.Position = new SFML.System.Vector2f(windowCenterX - (txtDescription.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 350);
            window.Draw(txtDescription);
            /* Disegno la textbox contenente le informazioni sull'avversario */
            txtOpponent.Position = new SFML.System.Vector2f(windowCenterX - txtOpponent.GetGlobalBounds().Width / 2, windowCenterY - (borderHeight / 2) + 390);
            window.Draw(txtOpponent);
            Text txtAccept = new Text("Vuoi accettarla?:", settings.font);
            txtAccept.FillColor = Color.White;
            txtAccept.CharacterSize = 18;
            txtAccept.Position = new SFML.System.Vector2f(windowCenterX - (txtAccept.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 430);
            window.Draw(txtAccept);
            /* Disegno il pulsante yes */
            btnYes.draw();
            /* Disegno il pulsante no */
            btnNo.draw();
        }
    }
}
