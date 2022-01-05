using AirHockey.window_utilities;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey
{
    class PageEstablishConnection
    {
        /* Pagina che si occupa di visualizzare l'interfaccia per stabilire una nuova connessione */
        private RenderWindow window;//finestra su cui bisogna disegnare la pagina
        private const int borderWidth = 600;
        private const int borderHeight = 600;
        private UITextInput txtIpAddress;
        public UIButton btnSendRequest;

        /* Costruttore */
        public PageEstablishConnection(RenderWindow window)
        {
            float windowCenterX = window.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = window.Size.Y / 2;//centro della finestra sull'asse delle Y
            SharedSettings settings = SharedSettings.GetInstance();
            this.window = window;
            txtIpAddress = new UITextInput(settings.font, window);
            txtIpAddress.Content = "192.168.1.45";
            txtIpAddress.Size = new SFML.System.Vector2f(300, 50);
            txtIpAddress.Position = new SFML.System.Vector2f(windowCenterX - (txtIpAddress.Size.X / 2), windowCenterY - (borderHeight / 2) + 400);
            txtIpAddress.BackgroundColor = Color.Black;
            txtIpAddress.ForegroundColor = Color.White;
            txtIpAddress.TextAlignment = UITextInput.AlignmentLeft;
            txtIpAddress.TextSize = 16;
            txtIpAddress.BorderColor = Color.White;
            txtIpAddress.BorderThickness = 2;
            btnSendRequest = new UIButton(window, "Invia richiesta", settings.font);
            btnSendRequest.BorderThickness = 2;
            btnSendRequest.BorderColor = Color.White;
            btnSendRequest.textColor = Color.White;
            btnSendRequest.FillColor = Color.Black;
            btnSendRequest.Size = new SFML.System.Vector2f(200, 50);//-> da sistemare
            btnSendRequest.Position = new SFML.System.Vector2f(windowCenterX - (btnSendRequest.Size.X / 2), windowCenterY - (borderHeight / 2) + 480);//-> da sistemare
            btnSendRequest.textSize = 18;
            btnSendRequest.ButtonPressed += ButtonClickedCallBack;
            /*   Mi metto in ascolto di eventuali richieste di connessione   */
            if (settings.sendAndReceive == null)
            {
                settings.sendAndReceive = new SendAndReceive();
            }
            settings.sendAndReceive.MessageReceived += MessageReceived;
        }

        private Message lastMessage = null;

        /*   Metodo richiamato dall'evento della classe SendAndReceive quando viene ricevuto un messaggio   */
        private void MessageReceived(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            //Console.WriteLine(e.message.Command);
            if (e.message.Command != null)
            {
                lastMessage = e.message;
                //controllo che il comando sia quello della connessione
                if (e.message.Command == "c")
                {
                    //Se il comando è quello di richiesta della connessione
                    //Visualizzo la schermata per accettare/rifiutare
                    settings.hostRequestorIP = e.message.sourceIP.ToString();
                    settings.hostRequestorUsername = e.message.Body;
                    settings.windowManager.PageDisplayed = WindowManager.AcceptConnectionPage;
                }
                else if(e.message.Command != "y" || e.message.Command != "n")
                {
                    //Se mi viene inviato un comando che non è quello di connessione invio il comando di chiusura della connessione
                    Message response = new Message();
                    response.Command = "e";
                    response.Body = "";
                    response.destinationIP = e.message.sourceIP;
                    sendAndReceive.SendMessage(response);
                }
            }
        }

        private void ButtonClickedCallBack(object sender, EventArgs e)
        {
            //Console.WriteLine("button pressed");
            //Invio la richiesta di connessione
            SharedSettings settings = SharedSettings.GetInstance();
            string ipAddress = txtIpAddress.Content;
            try
            {
                Message requestMsg = new Message();
                requestMsg.Command = "c";
                requestMsg.Body = settings.username;
                requestMsg.destinationIP = IPAddress.Parse(ipAddress);
                //procedo con l'invio del messaggio della richiesta di connessione
                SendAndReceive sendAndReceive = settings.sendAndReceive;
                sendAndReceive.SendMessage(requestMsg);
                //aspetto la risposta da parte dell'altro host
                lastMessage = null;
                do
                {
                    while (lastMessage == null)
                    {
                        Thread.Sleep(10);
                        //Console.WriteLine("Waiting");
                    }
                    

                } while ((lastMessage.Command != "y" && lastMessage.Command != "n") || lastMessage.sourceIP != IPAddress.Parse(ipAddress));

                if (lastMessage.Command == "y")
                {
                    //Risposta affermativa
                    //Invio la risposta dell'handshake a 3 vie
                    Message lastMsg = new Message();
                    lastMsg.Command = "y";
                    lastMsg.Body = "";
                    lastMsg.destinationIP = IPAddress.Parse(ipAddress);
                    sendAndReceive.SendMessage(lastMsg);
                    //vado alla pagina del gioco
                    settings.windowManager.PageDisplayed = WindowManager.GamePage;
                }
                else
                {
                    //Risposta negativa
                    //Non faccio nulla
                }

            }
            catch (Exception ex)
            {
                //Viene data eccezione se non è valido l'indirizzo IP inserito
                VideoMode messageBoxMode = new VideoMode(450, 150);
                UIMessageBox errorMessageBox = new UIMessageBox(messageBoxMode, "Errore", "L'indirizzo ip inserito non è valido", window, settings.font);
                errorMessageBox.Show();
                Console.Write(ex.Message);
            }
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
            border.OutlineColor = Color.White;
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
            Text textboxIpLabel = new Text("Inserisci l'ip dell'avversario", settings.font);
            textboxIpLabel.FillColor = Color.White;
            textboxIpLabel.CharacterSize = 25;
            textboxIpLabel.Position = new SFML.System.Vector2f(windowCenterX - (textboxIpLabel.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 350);
            window.Draw(textboxIpLabel);
            //disegno la textbox (uso l'oggetto UITextInput della libreria SFMLUIComponentsLibrary.dll [è da ottimizzare])
            txtIpAddress.draw();
            /* Disegno il pulsante per conferma lo Username */
            btnSendRequest.draw();
        }

        public void RemoveButtonPressedEvent()
        {
            btnSendRequest.ButtonPressed -= ButtonClickedCallBack;
        }
    }
}
