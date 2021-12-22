using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using window_utilities;//versione semplificata di SFMLUIControls

namespace AirHockey
{
    class PageSetUsername
    {
        public string Username { get; set; } = "";
        private RenderWindow window;
        private UITextInput textInput;
        private UIButton button;
        private const int borderWidth = 600;
        private const int borderHeight = 600;
        public PageSetUsername(RenderWindow window)
        {
            float windowCenterX = window.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = window.Size.Y / 2;//centro della finestra sull'asse delle Y
            SharedSettings settings = SharedSettings.GetInstance();
            this.window = window;
            textInput = new UITextInput(settings.font, window);
            textInput.Content = "";
            textInput.Size = new SFML.System.Vector2f(300, 50);
            textInput.Position = new SFML.System.Vector2f(windowCenterX - (textInput.Size.X / 2), windowCenterY - (borderHeight / 2) + 400);
            textInput.BackgroundColor = Color.Black;
            textInput.ForegroundColor = Color.White;
            textInput.TextAlignment = UITextInput.AlignmentLeft;
            textInput.TextSize = 16;
            textInput.BorderColor = Color.White;
            textInput.BorderThickness = 2;
            button = new UIButton(window, "Conferma" ,settings.font);
            button.BorderThickness = 2;
            button.BorderColor = Color.White;
            button.textColor = Color.White;
            button.FillColor = Color.Black;
            button.Size = new SFML.System.Vector2f(150, 50);//-> da sistemare
            button.Position = new SFML.System.Vector2f(windowCenterX - (button.Size.X / 2), windowCenterY - (borderHeight / 2) + 480);//-> da sistemare
            button.textSize = 18;
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
            sprite.Scale = new SFML.System.Vector2f(0.30f,0.30f);//dimensione scalata dell'immagine
            sprite.Position = new SFML.System.Vector2f(windowCenterX - (sprite.GetGlobalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 150);
            window.Draw(sprite);
            /*  disegno la textbox per l'inserimento dello username   */
            //disegno la label della textbox
            Text textboxUsernameLbl = new Text("Inserisci il tuo username", settings.font);
            textboxUsernameLbl.FillColor = Color.White;
            textboxUsernameLbl.CharacterSize = 25;
            textboxUsernameLbl.Position = new SFML.System.Vector2f(windowCenterX - (textboxUsernameLbl.GetLocalBounds().Width / 2), windowCenterY - (borderHeight / 2) + 350);
            window.Draw(textboxUsernameLbl);
            //disegno la textbox (uso l'oggetto UITextInput della libreria SFMLUIComponentsLibrary.dll [è da ottimizzare])
            textInput.draw();
            /* Disegno il pulsante per conferma lo Username */
            button.draw();
        }
    }
}
