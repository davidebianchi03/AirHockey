using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AirHockey
{
    class Ball
    {
        /* Classe che serve per gestire la pallina */
        public SFML.System.Vector2f Position { get; set; }//posizione della pallina all'interno del campo (la posizione è il centro della pallina)
        public const int Radius = 25;//raggio della pallina in pixel
        public double Angle { get; set; } = 5.6;//Angolo della pallina in radianti
        public double Speed { get; set; } = 300;//Velocità della pallina il px/s
        private RenderWindow parentWindow;
        private Thread moveBallThread;
        public SFML.System.Vector2f playgroundPosition { get; set; }//posizione del campo sullo schermo
        public SFML.System.Vector2f playgroundSize { get; set; }//dimensione del campo di gioco
        public int GoalWidth { get; set; } = 10;
        private bool moveBall = true;
        /*  Costruttore */
        public Ball(RenderWindow parentWindow, SFML.System.Vector2f playgroundSize, int GoalWidth)
        {
            Position = new SFML.System.Vector2f(100, 200);
            moveBallThread = new Thread(MoveBallThread);
            this.parentWindow = parentWindow;
            this.playgroundSize = playgroundSize;
            this.GoalWidth = GoalWidth;
        }

        /*   Metodo per iniare a muovere la pallina   */
        public void StartMovingBall()
        {
            if (moveBallThread != null)
            {
                moveBallThread.Start();
            }
        }
        /*   Metodo per interrompere il movimento della pallina   */
        public void StopMovingBall()
        {
            if (moveBallThread != null)
            {
                moveBall = false;
            }
        }
        /*   Metodo eseguito dal thread che serve per muovere la pallina   */
        private void MoveBallThread()
        {
            int windowRefreshDelay = 33;//tempo ogni quanto viene aggiornata la finestra in millisecondi
            while (moveBallThread.IsAlive && parentWindow.IsOpen && moveBall)
            {
                CheckBorderCollision();
                double distanceToMove = (Speed * windowRefreshDelay) / 1000;//distanza della quale si sposta la pallina nell'intervallo di tempo dell'aggiornamento dello schermo
                double distanceX = distanceToMove * Math.Cos(Angle);//spostamento della pallina sull'asse delle X
                double distanceY = distanceToMove * Math.Sin(Angle);//spostamento della pallina sull'asse delle Y
                Position = new SFML.System.Vector2f((float)(Position.X + distanceX), (float)(Position.Y + distanceY));
                //Controllo se ho subito goal (la pallina deve entrare tutta nella mia porta)
                SFML.System.Vector2f GlobalPosition = new SFML.System.Vector2f(Position.X + playgroundPosition.X, Position.Y + playgroundPosition.Y);
                if (GlobalPosition.Y - Radius > playgroundPosition.Y + playgroundSize.Y &&
                GlobalPosition.X > playgroundPosition.X + (playgroundSize.X / 2) - (GoalWidth / 2) &&
                GlobalPosition.X < playgroundPosition.X + (playgroundSize.X / 2) + (GoalWidth / 2))
                {
                    GoalSuffered?.Invoke(this, EventArgs.Empty);
                }
                Thread.Sleep(windowRefreshDelay);
            }
        }
        /*   Metodo per controllare eventuali collisioni con i bordi e calcolare l'eventuale angolo di rimbalzo   */
        private void CheckBorderCollision()
        {
            double angleInDegrees = RadiansToDegrees(Angle);
            angleInDegrees %= 360;
            SFML.System.Vector2f GlobalPosition = new SFML.System.Vector2f(Position.X + playgroundPosition.X, Position.Y + playgroundPosition.Y);

            //  Rimbalzo pallina all'interno delle porte
            if (GlobalPosition.Y - Radius < playgroundPosition.Y &&
                GlobalPosition.X > playgroundPosition.X + (playgroundSize.X / 2) - (GoalWidth / 2) &&
                GlobalPosition.X < playgroundPosition.X + (playgroundSize.X / 2) + (GoalWidth / 2))
            {
                //Rimbalzo nella porta dell'avversario
                #region Rimbalzo nella porta dell'avversario 
                double playgroundCenterX = playgroundPosition.X + (playgroundSize.X / 2);
                double goalTopY = playgroundPosition.Y - 50;
                //          collisione bordo superiore
                if (GlobalPosition.Y - Radius < goalTopY)
                {
                    //Console.WriteLine("Top");
                    if (angleInDegrees >= 180 && angleInDegrees < 270)
                    {
                        angleInDegrees = 90 + Math.Abs(angleInDegrees - 270);
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 360);
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    //Position = new SFML.System.Vector2f(Position.X, Position.Y - 10);
                }
                //          collisione bordo sinistro
                if (GlobalPosition.X - Radius < playgroundCenterX - (GoalWidth / 2))
                {
                    //Console.WriteLine("Left");
                    if (angleInDegrees >= 90 && angleInDegrees < 180)
                    {
                        angleInDegrees = 90 - Math.Abs(angleInDegrees - 90);
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 270) + 270;
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X + 10, Position.Y);
                }
                //          collisione bordo destro
                if (GlobalPosition.X + Radius > playgroundCenterX + (GoalWidth / 2))
                {
                    //Console.WriteLine("Right");
                    if (angleInDegrees >= 0 && angleInDegrees < 90)
                    {
                        angleInDegrees = 180 - angleInDegrees;
                    }
                    else
                    {
                        angleInDegrees = 180 + Math.Abs(angleInDegrees - 360);
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X - 10, Position.Y);
                }
                //Console.WriteLine(angleInDegrees);
                #endregion
            }
            else if (GlobalPosition.Y + Radius > playgroundPosition.Y + playgroundSize.Y &&
                GlobalPosition.X > playgroundPosition.X + (playgroundSize.X / 2) - (GoalWidth / 2) &&
                GlobalPosition.X < playgroundPosition.X + (playgroundSize.X / 2) + (GoalWidth / 2))
            {
                //Rimbalzo nella mia porta
                #region Rimbalzo nella mia porta
                double playgroundCenterX = playgroundPosition.X + (playgroundSize.X / 2);
                double goalBottomY = playgroundPosition.Y + playgroundSize.Y + 50;
                //          collisione bordo inferiore
                if (GlobalPosition.Y + Radius > goalBottomY)
                {
                    //Console.WriteLine("Bottom");
                    if (angleInDegrees >= 0 && angleInDegrees < 90)
                    {
                        angleInDegrees = 360 - angleInDegrees;
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 180) + 180;
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    //Position = new SFML.System.Vector2f(Position.X, Position.Y - 10);
                }
                //          collisione bordo sinistro
                if (GlobalPosition.X - Radius < playgroundCenterX - (GoalWidth / 2))
                {
                    //Console.WriteLine("Left");
                    if (angleInDegrees >= 90 && angleInDegrees < 180)
                    {
                        angleInDegrees = 90 - Math.Abs(angleInDegrees - 90);
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 270) + 270;
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X + 10, Position.Y);
                }
                //          collisione bordo destro
                if (GlobalPosition.X + Radius > playgroundCenterX + (GoalWidth / 2))
                {
                    //Console.WriteLine("Right");
                    if (angleInDegrees >= 0 && angleInDegrees < 90)
                    {
                        angleInDegrees = 180 - angleInDegrees;
                    }
                    else
                    {
                        angleInDegrees = 180 + Math.Abs(angleInDegrees - 360);
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X - 10, Position.Y);
                }
                #endregion
            }
            else
            {
                //Rimbalzo sulle pareti
                #region Rimalzo sulle pareti
                //Console.Write(angleInDegrees + " ");
                //          collisione bordo superiore
                if (GlobalPosition.Y - Radius < playgroundPosition.Y && GlobalPosition.X > playgroundPosition.X && GlobalPosition.X < playgroundPosition.X + playgroundSize.X)
                {
                    //Console.WriteLine("Top");
                    if (angleInDegrees >= 180 && angleInDegrees < 270)
                    {
                        angleInDegrees = 90 + Math.Abs(angleInDegrees - 270);
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 360);
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X, Position.Y + 10);
                }
                //          collisione bordo inferiore
                if (GlobalPosition.Y + Radius > playgroundPosition.Y + playgroundSize.Y && GlobalPosition.X > playgroundPosition.X && GlobalPosition.X < playgroundPosition.X + playgroundSize.X)
                {
                    //Console.WriteLine("Bottom");
                    if (angleInDegrees >= 0 && angleInDegrees < 90)
                    {
                        angleInDegrees = 360 - angleInDegrees;
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 180) + 180;
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X, Position.Y - 10);
                }
                //          collisione bordo sinistro
                if (GlobalPosition.X - Radius < playgroundPosition.X && GlobalPosition.Y > playgroundPosition.Y && GlobalPosition.Y < playgroundPosition.Y + playgroundSize.Y)
                {
                    //Console.WriteLine("Left");
                    if (angleInDegrees >= 90 && angleInDegrees < 180)
                    {
                        angleInDegrees = 90 - Math.Abs(angleInDegrees - 90);
                    }
                    else
                    {
                        angleInDegrees = Math.Abs(angleInDegrees - 270) + 270;
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X + 10, Position.Y);
                }
                //          collisione bordo destro
                if (GlobalPosition.X + Radius > playgroundPosition.X + playgroundSize.X && GlobalPosition.Y > playgroundPosition.Y && GlobalPosition.Y < playgroundPosition.Y + playgroundSize.Y)
                {
                    //Console.WriteLine("Right");
                    if (angleInDegrees >= 0 && angleInDegrees < 90)
                    {
                        angleInDegrees = 180 - angleInDegrees;
                    }
                    else
                    {
                        angleInDegrees = 180 + Math.Abs(angleInDegrees - 360);
                    }
                    Angle = DegreesToRadians(angleInDegrees);
                    Position = new SFML.System.Vector2f(Position.X - 10, Position.Y);
                }
                //Console.WriteLine(angleInDegrees);
                #endregion
            }

        }

        /*   Metodo per disegnare la pallina sul campo   */
        public void DrawBall(SFML.System.Vector2f playgroundPosition)
        {
            float GlobalX = playgroundPosition.X + Position.X;//posizione della pallina sulla finestra sull'asse delle X
            float GlobalY = playgroundPosition.Y + Position.Y;//posizione della pallina sulla finestra sull'asse delle Y
            CircleShape ball = new CircleShape();
            ball.Position = new SFML.System.Vector2f(GlobalX - Radius, GlobalY - Radius);
            ball.FillColor = Color.Yellow;
            ball.Radius = Radius;
            parentWindow.Draw(ball);
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

        /*   Evento che viene richiamato quando viene subito un goal   */
        public event EventHandler GoalSuffered;
    }
}
