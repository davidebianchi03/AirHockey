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
        public SFML.System.Vector2f LastPosition = new SFML.System.Vector2f(0, 0);//ultima posizione registrata del cursore

        public MyHandle(RenderWindow parentWindow, SFML.System.Vector2f PlaygroundSize, Ball Ball)
        {
            this.parentWindow = parentWindow;
            this.PlaygroundSize = PlaygroundSize;
            this.Ball = Ball;
            this.Position = new SFML.System.Vector2f((PlaygroundSize.X / 2), (PlaygroundSize.Y / 2) + Radius + 50);
            movingListenerThread = new Thread(MovingListenerThreadMethod);
            //invio la posizione iniziale della racchetta se è nella propria metà campo il cursore
            SharedSettings settings = SharedSettings.GetInstance();
            if (Mouse.GetPosition(parentWindow).X > Ball.playgroundPosition.X
                && Mouse.GetPosition(parentWindow).X < Ball.playgroundPosition.X + Ball.playgroundSize.X
                && Mouse.GetPosition(parentWindow).Y > Ball.playgroundPosition.X + (Ball.playgroundSize.X / 2) 
                && Mouse.GetPosition(parentWindow).Y < Ball.playgroundPosition.Y + Ball.playgroundSize.Y)
            {
                SendAndReceive sendAndReceive = settings.sendAndReceive;
                Message newHandlePosMessage = new Message();
                newHandlePosMessage.Command = "m";
                string body = Position.X.ToString() + ";" + Position.Y.ToString();
                newHandlePosMessage.Body = body;
                newHandlePosMessage.destinationIP = settings.Connection.OpponentIP;
                sendAndReceive.SendMessage(newHandlePosMessage);
            }
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
            listen = true;
            movingListenerThread.Start();
        }

        /*
            Metodo che serve per fermare il thread che ascolta la posizione del mouse 
        */
        public void StopMovingListenerThread()
        {
            listen = false;
        }
        private bool listen = true;
        /*
            Metodo che viene eseguito dal thread per aggiornare la posizione del proprio cursore 
        */
        public void MovingListenerThreadMethod()
        {
            int updateTime = 2;//tempo ogni quanto viene aggiornata la posizione del cursore sullo schermo
            long lastContactTicks = DateTime.Now.Ticks;
            SharedSettings settings = SharedSettings.GetInstance();

            while (parentWindow.IsOpen && settings.Connection != null && listen)
            {
                try
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
                    double distance = Math.Sqrt(Math.Pow((Position.X - Ball.Position.X), 2) + Math.Pow((Position.Y - Ball.Position.Y), 2));
                    //Console.WriteLine(DateTime.Now.Ticks);
                    if (distance < Radius + Ball.Radius && (DateTime.Now.Ticks - lastContactTicks) >= 10000 * 500)//in ogni millisecondo ci sono 10000 ticks
                    {
                        Ball.Angle = CalculateBallRebounceAngle();
                        if (Ball.Speed < settings.SpeedIncrease * 20 + 300)
                        {
                            Ball.Speed += settings.SpeedIncrease;
                        }
                        lastContactTicks = DateTime.Now.Ticks;
                        //invio la nuova posizione e il nuovo angolo della pallina
                        SendAndReceive sendAndReceive = settings.sendAndReceive;
                        Message updatePositionMsg = new Message();
                        updatePositionMsg.Command = "p";
                        string CommandParameters = Ball.Angle.ToString() + ";" + Ball.Speed.ToString() + ";" + Ball.Position.X.ToString() + ";" + Ball.Position.Y.ToString();
                        updatePositionMsg.Body = CommandParameters;
                        updatePositionMsg.destinationIP = settings.Connection.OpponentIP;
                        sendAndReceive.SendMessage(updatePositionMsg);
                    }

                    /*   Controllo se è cambiata la posizione del cursore e in caso invio la nuova posizione   */
                    if (LastPosition.X != Position.X || LastPosition.Y != Position.Y)
                    {
                        LastPosition = Position;
                        //invio il messaggio con la nuova posizione
                        SendAndReceive sendAndReceive = settings.sendAndReceive;
                        Message newHandlePosMessage = new Message();
                        newHandlePosMessage.Command = "m";
                        string body = Position.X.ToString() + ";" + Position.Y.ToString();
                        newHandlePosMessage.Body = body;
                        newHandlePosMessage.destinationIP = settings.Connection.OpponentIP;
                        sendAndReceive.SendMessage(newHandlePosMessage);
                    }

                    Thread.Sleep(updateTime);
                }catch (Exception ex) { }
            }
        }

        public double CalculateBallRebounceAngle()
        {
            double newAngle = 0;
            //Scorro in avanti la posizione della pallina fino a trovare il punto che passa dalla circonferenza
            /*   Posizione della pallina   */
            float BallX = Ball.Position.X;
            float BallY = Ball.Position.Y;
            /*   Posizione della manopola   */
            float HandleX = Position.X;
            float HandleY = Position.Y;
            //Angolo con cui si muove la pallina
            double BallAngle = Ball.Angle;

            double InteresectionX = BallX;//coordinata del punto di intersezione sull'asse delle X
            double InteresectionY = BallY;//coordinata del punto di intersezione sull'asse delle Y

            double distanceToMove = (Ball.Speed * 33) / 1000;
            double distanceX = distanceToMove * Math.Cos(BallAngle);//spostamento della pallina sull'asse delle X
            double distanceY = distanceToMove * Math.Sin(BallAngle);//spostamento della pallina sull'asse delle Y
            double lastBallX = 0;
            double lastBallY = 0;

            if (BallX < HandleX)
            {
                //se la palla è a sinistra della manopola
                lastBallX = BallX - distanceX;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X - 5, Ball.Position.Y);
            }
            else
            {
                //se la palla è a destra della manopola
                lastBallX = BallX + distanceX;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X + 5, Ball.Position.Y);
            }

            if (BallY < HandleY)
            {
                //se la palla è sopra alla manopola
                lastBallY = BallY - distanceY;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X, Ball.Position.Y - 5);
            }
            else
            {
                //se la palla è sotto alla manopola
                lastBallY = BallY + distanceY;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X, Ball.Position.Y + 5);
            }

            //trovo il punto di contatto della pallina con la manopola (punto di intersezione della linea che percorre la pallina e il cerchio della manopola)
            double dx = lastBallX - BallX;
            double dy = lastBallY - BallY;

            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (BallX - HandleX) + dy * (BallY - BallY));
            double C = (BallX - HandleX) * (BallX - HandleX) +
                (BallY - BallY) * (BallY - BallY) -
                Radius * Radius;
            double delta = B * B - 4 * A * C;

            if (delta >= 0)
            {
                float t1 = (float)((-B + Math.Sqrt(delta)) / (2 * A));
                SFML.System.Vector2f p1 = new SFML.System.Vector2f((float)(BallX + t1 * dx), (float)(BallY + t1 * dy));
                float t2 = (float)((-B - Math.Sqrt(delta)) / (2 * A));
                SFML.System.Vector2f p2 = new SFML.System.Vector2f((float)(BallX + t2 * dx), (float)(BallY + t2 * dy));

                double d1 = Math.Sqrt(Math.Pow(p1.X - BallX, 2) + Math.Pow(p1.Y - BallY, 2));
                double d2 = Math.Sqrt(Math.Pow(BallX - p2.X, 2) + Math.Pow(BallY - p2.Y, 2));

                if (d1 < d2)
                {
                    InteresectionX = p1.X;
                    InteresectionY = p1.Y;
                }
                else
                {
                    InteresectionX = p2.X;
                    InteresectionY = p2.Y;
                }
                //trovo l'angolo rispetto all'asse delle x della retta tangente al punt di contatto
                double tangentAngle = Math.Atan2(InteresectionY, InteresectionX);//angolo rispetto all'asse delle x della tangente al punto di contatto
                //trovo l'angolo riflesso rispetto alla tangente
                double gamma = BallAngle - tangentAngle;
                newAngle = gamma - tangentAngle;
                //Console.WriteLine(newAngle);
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
