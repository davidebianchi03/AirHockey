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
        public SFML.System.Vector2f GlobalPosition = new SFML.System.Vector2f(0, 0);//posizione della manopola sulla finestra intera
        public SFML.System.Vector2f LastPosition = new SFML.System.Vector2f(0, 0);//ultima posizione registrata del cursore
        const long TicksPerSecond = 10000000;

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
            int updateTime = 1;//tempo ogni quanto viene aggiornata la posizione del cursore sullo schermo
            long lastContactTicks = DateTime.Now.Ticks;
            SharedSettings settings = SharedSettings.GetInstance();
            movementsList = new List<Movement>();
            const int maxMovementsListLength = 100;//numero massimo di movimenti contenuti della lista

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
                    if (distance < Radius + Ball.Radius && (DateTime.Now.Ticks - lastContactTicks) >= 10000 * 500 /*&& distance > (Radius + Ball.Radius - 30)*/)//in ogni millisecondo ci sono 10000 ticks
                    {
                        if (movementsList.Count <= 2 || movementsList[movementsList.Count - 1].Ticks - DateTime.Now.Ticks > TicksPerSecond * 2)
                        {
                            Ball.Speed = Ball.Speed * 0.6;
                            Ball.Angle += Math.PI / 2;
                        }
                        else
                        {
                            Ball.Angle = calculateNewAngle2();//CalculateBallRebounceAngle();
                            /*if (Ball.Speed < settings.SpeedIncrease * 20 + 300)
                            {
                                Ball.Speed += settings.SpeedIncrease;
                            }*/
                            Ball.Speed = CalculateNewBallSpeed();
                            movementsList = new List<Movement>();
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

                        //Console.WriteLine(CalculateNewBallSpeed());
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
                        //aggiunta della nuova posizione alla lista
                        if (movementsList.Count > maxMovementsListLength)
                        {
                            movementsList.RemoveAt(0);
                        }
                        Movement m = new Movement();
                        m.Position = Position;
                        m.Ticks = DateTime.Now.Ticks;
                        movementsList.Add(m);
                    }

                    Thread.Sleep(updateTime);
                }
                catch (Exception ex) { }
            }
        }

        private List<Movement> movementsList = null;

        private double CalculateNewBallSpeed()
        {
            double speed = 300;
            double minSpeed = 150;
            if (movementsList.Count >= 2)
            {
                SFML.System.Vector2f firstPoint = movementsList[movementsList.Count - 1].Position;//fisso
                long firstPointTicks = movementsList[movementsList.Count - 1].Ticks;
                SFML.System.Vector2f lastPoint = movementsList[movementsList.Count - 2].Position;//cambia
                long lastPointTicks = movementsList[movementsList.Count - 2].Ticks;
                //valori di m e q della linea presa come riferimento (y = mx + q)
                double m = (lastPoint.Y - firstPoint.Y) / (lastPoint.X - firstPoint.X);
                double q = ((firstPoint.X * lastPoint.Y) - (lastPoint.X * firstPoint.X)) / (firstPoint.X - lastPoint.X);
                //trovo qual'è l'ultimo punto appartenente alla linea
                for (int i = movementsList.Count - 3; i > 0; i--)
                {
                    double m2 = 0;
                    if (movementsList[i].Position.X != firstPoint.X)
                    {
                        m2 = (movementsList[i].Position.Y - firstPoint.Y) / (movementsList[i].Position.X - firstPoint.X);
                    }
                    double q2 = ((firstPoint.X * movementsList[i].Position.Y) - (movementsList[i].Position.X * firstPoint.X)) / (firstPoint.X - movementsList[i].Position.X);

                    //se il valore delle nuove m e q non sono molto diversi
                    if (Math.Abs(m - m2) < 2 && Math.Abs(q - q2) < 2)
                    {
                        //il nuovo punto è accettabile
                        lastPoint = movementsList[i].Position;
                        lastPointTicks = movementsList[i].Ticks;
                    }
                    else
                    {
                        //altrimenti i punti da qui in poi non sono più accettabili
                        break;
                    }
                }

                double distance = Math.Sqrt(Math.Pow(firstPoint.X - lastPoint.X, 2) + Math.Pow(firstPoint.Y - lastPoint.Y, 2));
                speed = distance / (Math.Abs(lastPointTicks - firstPointTicks)) / TicksPerSecond;
                speed *= Math.Pow(10, 13) * 8;
                /*if(speed > 1500)
                    speed = 1500;*/
                speed += Ball.Speed * 0.6;
                
                if(speed < minSpeed)
                    speed = minSpeed;
            }
            else
            {
                speed = 0;
            }

            
            return speed;
        }

        private double calculateNewAngle2()
        {
            double dx = movementsList[movementsList.Count - 1].Position.X - movementsList[movementsList.Count - 2].Position.X;
            double dy = movementsList[movementsList.Count - 1].Position.Y - movementsList[movementsList.Count - 2].Position.Y;

            double newAngle = Math.Atan2(dy ,dx);
            return newAngle;
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

            float d = (float)(33 * CalculateNewBallSpeed() * 0.005);

            if (BallX < HandleX)
            {
                //se la palla è a sinistra della manopola
                lastBallX = BallX - distanceX;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X - d, Ball.Position.Y);
            }
            else
            {
                //se la palla è a destra della manopola
                lastBallX = BallX + distanceX;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X + d, Ball.Position.Y);
            }

            if (BallY < HandleY)
            {
                //se la palla è sopra alla manopola
                lastBallY = BallY - distanceY;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X, Ball.Position.Y - d);
            }
            else
            {
                //se la palla è sotto alla manopola
                lastBallY = BallY + distanceY;
                Ball.Position = new SFML.System.Vector2f(Ball.Position.X, Ball.Position.Y + d);
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

    public struct Movement
    {
        public SFML.System.Vector2f Position;
        public long Ticks;
    }
}
