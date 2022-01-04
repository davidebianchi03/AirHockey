using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AirHockey
{
    class MyHandle
    {
        /*
            Classe che serve per gestire la mia manopola
        */
        private RenderWindow parentWindow;
        public Color FillColor { get; set; } = new Color(5, 35, 226);//colore della manopola
        public Color BorderColor { get; set; } = new Color(0, 128, 255);//colore del bordo della manopola
        public int Radius { get; set; } = 40;//raggio della manopola in pixels
        public SFML.System.Vector2f Position { get; set; } = new SFML.System.Vector2f(150, 150);//Posizione della pallina all'interno del campo da gioco
        public SFML.System.Vector2f PlaygroundSize { get; set; }//dimensione del campo da gioco
        public SFML.System.Vector2f PlaygroundPosition { get; set; } = new SFML.System.Vector2f(0, 0);//posizione del campo da gioco
        private Thread movingListenerThread;//thread che ascolta e aggiorna lo spostamento della del cursore
        public Ball Ball { get; set; }

        public MyHandle(RenderWindow parentWindow, SFML.System.Vector2f PlaygroundSize, Ball Ball)
        {
            this.parentWindow = parentWindow;
            this.PlaygroundSize = PlaygroundSize;
            this.Ball = Ball;
            this.Position = new SFML.System.Vector2f((PlaygroundSize.X / 2), (PlaygroundSize.Y / 2) + Radius + 50);
            movingListenerThread = new Thread(MovingListenerThreadMethod);
        }

        /*
            Metodo che serve per disegnare il proprio cursore
        */
        public void Draw()
        {
            SFML.System.Vector2f GlobalPosition = new SFML.System.Vector2f(Position.X + PlaygroundPosition.X, Position.Y + PlaygroundPosition.Y);//posizione globale della manopola sulla finestra
            /*   Disegno la manopola   */
            //  Disegno il cerchio esterno
            CircleShape outerCircle = new CircleShape();
            outerCircle.Position = new SFML.System.Vector2f(GlobalPosition.X - Radius, GlobalPosition.Y - Radius);
            outerCircle.Radius = Radius;
            outerCircle.FillColor = FillColor;
            outerCircle.OutlineColor = BorderColor;
            outerCircle.OutlineThickness = 10;
            parentWindow.Draw(outerCircle);
            //  Disegno il cerchio interno
            int innerRadius = 10;
            CircleShape innerCircle = new CircleShape();
            outerCircle.Position = new SFML.System.Vector2f(GlobalPosition.X - innerRadius, GlobalPosition.Y - innerRadius);
            outerCircle.Radius = innerRadius;
            outerCircle.FillColor = BorderColor;
            outerCircle.OutlineColor = BorderColor;
            outerCircle.OutlineThickness = 7;
            parentWindow.Draw(outerCircle);
        }

        /*
            Metodo che serve per avviare il thread che ascolta la posizione del mouse 
        */
        public void StartMovingListenerThread()
        {
            movingListenerThread.Start();
        }

        /*
            Metodo che serve per fermare il thread che ascolta la posizione del mouse 
        */
        public void StopMovingListenerThread()
        {
            movingListenerThread.Abort();
        }

        /*
            Metodo che viene eseguito dal thread per aggiornare la posizione del proprio cursore 
        */
        public void MovingListenerThreadMethod()
        {
            int updateTime = 2;//tempo ogni quanto viene aggiornata la posizione del cursore sullo schermo
            while (parentWindow.IsOpen)
            {
                float globalX = Mouse.GetPosition(parentWindow).X;
                float globalY = Mouse.GetPosition(parentWindow).Y;
                //Controllo se il cursore si trova all'interno del campo da gioco nella mia metà campo
                if (globalX - Radius > PlaygroundPosition.X
                    && globalX + Radius < PlaygroundPosition.X + PlaygroundSize.X &&
                    globalY - Radius > PlaygroundPosition.Y + (PlaygroundSize.Y / 2) &&
                    globalY + Radius < PlaygroundSize.Y + PlaygroundPosition.Y)
                {
                    //calcolo la posizione locale (nel campo del cursore)
                    Position = new SFML.System.Vector2f(globalX - PlaygroundPosition.X, globalY - PlaygroundPosition.Y);//posizione globale della manopola sulla finestra
                }

                /* Calcolo la pallina se è andata a scontrarsi con la manopola */
                double distance = Math.Sqrt(Math.Pow((Position.X - Ball.Position.X),2) + Math.Pow((Position.Y - Ball.Position.Y), 2));

                if(distance < Radius + Ball.Radius)
                {
                    double new_angle = calculateNewAngle(Position.X, Position.Y, Ball.Position.X, Ball.Position.Y);
                    Ball.Angle = new_angle;
                    Console.WriteLine(new_angle);
                }

                Thread.Sleep(updateTime);
            }
        }

        private double calculateNewAngle(double malletX, double malletY, double ballPosX, double ballPosY)
        {
            double deltaY = ballPosX - malletY;
            double deltaX = ballPosY - malletX;

            // keep track if original x or y was negative so know which end direction should be negative
            // otherwise 2 negatives will just cancel out or don't know if x or y was negative
            int xneg = 1;
            int yneg = 1;
            if (deltaX < 0)
            {
                xneg = -1;
            }

            if (deltaY < 0)
            {
                yneg = -1;
            }
            deltaX = Math.Abs(deltaX);
            deltaY = Math.Abs(deltaY);
            if (deltaX != 0 && deltaY != 0)
            {
                // calculate the inverse tangent of the slope
                double angle1 = Math.Atan(deltaX / deltaY);
                double angle2 = 90 - angle1;
                deltaY = Math.Sin(angle2) * yneg;
                deltaX = Math.Sin(angle1) * xneg;
            }
            else if (deltaX == 0)
            {
                deltaY = yneg;
            }
            else
            {
                deltaX = xneg;
            }

            double angle = Math.Atan2(deltaY, deltaX);
            if (angle < 0)
            {
                angle = (Math.PI * 2) + angle;
            }
            return angle;
        }





    }
}
