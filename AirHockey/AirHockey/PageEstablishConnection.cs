using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
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
        private UIButton btnSendRequest;

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
        }

        private void ButtonClickedCallBack(object sender, EventArgs e)
        {

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
    }
}
