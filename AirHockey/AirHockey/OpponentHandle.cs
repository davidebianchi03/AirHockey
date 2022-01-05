using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace AirHockey
{
    class OpponentHandle
    {
        private RenderWindow parentWindow;
        public Color FillColor { get; set; } = new Color(226, 5, 5);//colore della manopola
        public Color BorderColor { get; set; } = new Color(254, 127, 0);//colore del bordo della manopola
        public int Radius { get; set; } = 40;//raggio della manopola in pixels
        public SFML.System.Vector2f Position { get; set; } = new SFML.System.Vector2f(150, 150);//Posizione della pallina all'interno del campo da gioco
        public SFML.System.Vector2f PlaygroundSize { get; set; }//dimensione del campo da gioco
        public SFML.System.Vector2f PlaygroundPosition { get; set; } = new SFML.System.Vector2f(0, 0);//posizione del campo da gioco
       
       
        

        public OpponentHandle(RenderWindow parentWindow, SFML.System.Vector2f PlaygroundSize)
        {
            this.parentWindow = parentWindow;
            this.PlaygroundSize = PlaygroundSize;
          
            this.Position = new SFML.System.Vector2f((PlaygroundSize.X / 2), (PlaygroundSize.Y / 2) + Radius + 50);
            
        }

        public void Draw()
        {
            SFML.System.Vector2f GlobalPosition = new SFML.System.Vector2f(Position.X + PlaygroundPosition.X, Position.Y + PlaygroundPosition.Y);//posizione globale della manopola sulla finestra
            /*   Disegno la manopola   */
            //  Disegno il cerchio esterno
            CircleShape outerCircle = new CircleShape();
            outerCircle.Position = new SFML.System.Vector2f(GlobalPosition.X - Radius, GlobalPosition.Y - Radius);
            outerCircle.Radius = Radius - 4;
            outerCircle.FillColor = FillColor;
            outerCircle.OutlineColor = BorderColor;
            outerCircle.OutlineThickness = 4;
            parentWindow.Draw(outerCircle);
            //  Disegno il cerchio interno
            int innerRadius = 10;
            CircleShape innerCircle = new CircleShape();
            outerCircle.Position = new SFML.System.Vector2f(GlobalPosition.X - innerRadius - 3, GlobalPosition.Y - innerRadius - 3);
            outerCircle.Radius = innerRadius;
            outerCircle.FillColor = BorderColor;
            outerCircle.OutlineColor = BorderColor;
            outerCircle.OutlineThickness = 7;
            parentWindow.Draw(outerCircle);
        }
    }
}
