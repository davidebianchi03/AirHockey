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
        public SFML.System.Vector2f GlobalPosition = new SFML.System.Vector2f(0,0);//posizione della manopola sulla finestra intera

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
            long lastContactTicks = DateTime.Now.Ticks;
            while (parentWindow.IsOpen)
            {
                float globalX = Mouse.GetPosition(parentWindow).X;
                float globalY = Mouse.GetPosition(parentWindow).Y;
                GlobalPosition = new SFML.System.Vector2f(globalX, globalY);
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
                //Console.WriteLine(DateTime.Now.Ticks);
                if(distance < Radius + Ball.Radius && (DateTime.Now.Ticks - lastContactTicks) >= 10000 * 500)//in ogni millisecondo ci sono 10000 ticks
                {
                    Ball.Angle = CalculateBallRebounceAngle();
                    lastContactTicks = DateTime.Now.Ticks;
                }

                Thread.Sleep(updateTime);
            }
        }

        public double CalculateBallRebounceAngle()
        {
            SharedSettings settings = SharedSettings.GetInstance();
            /*   Calcolo in che quadrante rispetto alla manopola si trova il disco   */
            double ballX = Ball.Position.X;
            double ballY = Ball.Position.Y;
            double handleX = Position.X;
            double handleY = Position.Y;
            
            double tangent = Math.Atan2(ballY - handleY, ballX - handleX);
            double newAngle = Math.PI / 2 + tangent;
           
            if(newAngle < 0)
            {
                newAngle = ((Math.PI * 2) - (Math.PI / 2)) - newAngle;
            }

            return newAngle;
        }

        /*  Metodo per convertire i gradi in radianti   */
        public double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        /*  Metodo per convertire i radianti in gradi   */
        public double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }




    }
}
