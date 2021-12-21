using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirHockey
{
    class PageSetUsername
    {
        public string Username { get; set; } = "";
        private RenderWindow window = null;
        public PageSetUsername(RenderWindow window)
        {
            this.window = window;
        }

        public void Draw()
        {
            float windowCenterX = window.Size.X / 2;//centro della finestra sull'asse delle X
            float windowCenterY = window.Size.Y / 2;//centro della finestra sull'asse delle Y
            //disegno il rettangolo di contorno di tutto
            int borderWidth = 400;
            int borderHeight = 600;
            RectangleShape border = new RectangleShape(new SFML.System.Vector2f(borderWidth, borderHeight));
            border.Position = new SFML.System.Vector2f(windowCenterX - (borderWidth / 2), windowCenterY - (borderHeight / 2));
        }
    }
}
