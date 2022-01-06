using AirHockey.window_utilities;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
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
        }

        /* Metodo richiamato quando viene ricevuto un messaggio */
        private void MessageReceivedCallback(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if (e.message.Command == "e" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
            {
                //concludo la connessione
                settings.Connection = null;
                //visualizzo un messagggio
                VideoMode msgMode = new VideoMode(400, 150);
                UIMessageBox messageBox = new UIMessageBox(msgMode, "Connessione interrotta", "L'altro host ha interrotto la connessione", parentWindow, settings.font);
                messageBox.Show();
                //visualizzo la pagina per stabilire la connessione
                settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
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
            Console.WriteLine("Rematch");
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
