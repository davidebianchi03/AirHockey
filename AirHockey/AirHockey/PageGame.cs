using AirHockey.window_utilities;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey
{
    class PageGame
    {
        /*
            Classe che serve per visualizzare e gestire il gioco 
        */
        private RenderWindow parentWindow;
        private SFML.System.Vector2f playgroundSize = new SFML.System.Vector2f(500, 800);//dimensione del campo
        private int goalWidth = 180;
        private RectangleShape playground;//campo da hockey
        private RectangleShape myGoal;//la mia porta
        private RectangleShape opponentGoal;//la porta dell'avversario
        public Ball Ball { get; set; }
        private HandleUpdate handleUpdate;//aggiornamento della posizione della manopola dell'avversario
        private BallUpdate ballUpdate;//aggiornamento della posizione della pallina
        private bool GoalSuffered = false;
        private bool GoalScored = false;
        public PageGame(RenderWindow parentWindow)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            this.parentWindow = parentWindow;
            //imposto i parametri fissi del campo da gioco
            playground = new RectangleShape();
            playground.Size = playgroundSize;
            playground.OutlineColor = Color.White;
            playground.OutlineThickness = 1;
            playground.FillColor = Color.Black;
            /*  imposto i parametri fissi delle porte   */
            //la mia porta
            Texture textureGoal = new Texture(settings.resourcesPath + "goal_texture.png");
            myGoal = new RectangleShape();
            myGoal.Size = new SFML.System.Vector2f(goalWidth, 50);
            myGoal.Texture = textureGoal;
            //la porta dell'avversario
            opponentGoal = new RectangleShape();
            opponentGoal.Size = new SFML.System.Vector2f(goalWidth, 50);
            opponentGoal.Texture = textureGoal;
            //creo l'oggetto della pallina
            Ball = new Ball(parentWindow, playgroundSize, goalWidth);
            Ball.StartMovingBall();
            //creo l'oggetto della mia manopola
            settings.MyHandle = new MyHandle(parentWindow, playgroundSize, Ball);
            settings.MyHandle.StartMovingListenerThread();
            settings.opponentHandle = new OpponentHandle(parentWindow, playgroundSize);
            //creo l'oggetto che aggiorna la posizione della manopola
            handleUpdate = new HandleUpdate(settings.opponentHandle);
            //creo l'oggetto che aggiorna la posizione della pallina
            ballUpdate = new BallUpdate(Ball);
            /*   Se sono stato io a richiedere la connessione invio la posizione iniziale della pallina   */

            if (settings.Connection.IEstablish)
            {
                Console.WriteLine("I established");
                //genero l'angolo random della pallina
                Random Rand = new Random();
                Ball.Angle = Rand.NextDouble() * (Math.PI * 2);
                SendAndReceive sendAndReceive = settings.sendAndReceive;
                Message updatePositionMsg = new Message();
                updatePositionMsg.Command = "p";
                string CommandParameters = Ball.Angle.ToString() + ";" + Ball.Speed.ToString() + ";" + Ball.Position.X.ToString() + ";" + Ball.Position.Y.ToString();
                updatePositionMsg.Body = CommandParameters;
                updatePositionMsg.destinationIP = settings.Connection.OpponentIP;
                sendAndReceive.SendMessage(updatePositionMsg);
            }
            //Richiamo l'evento che mi indica quando subisco un goal
            Ball.GoalSuffered += GoalSufferedCallback;
            settings.sendAndReceive.MessageReceived += MessageReceivedCallback;
        }

        /*   Messaggio ricevuto (mi serve per poter rilevare quando faccio goal)   */
        private void MessageReceivedCallback(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if (e.message.Command == "g" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
            {
                Thread t = new Thread(delegate ()
                {
                    settings.Connection.myPoints++;
                    GoalScored = true;
                    //Riproduco il suono del Goal fatto
                    SoundPlayer soundPlayer = new SoundPlayer(settings.resourcesPath + "GoalScoredSound.wav");
                    soundPlayer.Play();
                    Thread.Sleep(2000);
                    GoalScored = false;
                });
                t.Start();
            }
            else if(settings.Connection == null || (e.message.Command == "e" && e.message.sourceIP.Equals(settings.Connection.OpponentIP)))
            {
                //concludo la connessione
                settings.Connection = null;
                //visualizzo un messagggio
                VideoMode msgMode = new VideoMode(500, 150);
                UIMessageBox messageBox = new UIMessageBox(msgMode, "Connessione interrotta", "L'altro host ha interrotto la connessione" ,parentWindow, settings.font);
                messageBox.Show();
                //visualizzo la pagina per stabilire la connessione
                settings.windowManager.PageDisplayed = WindowManager.EstabishConnectionPage;
            }
        }

        /*  Metodo che viene richiamato dall'evento del goal subito  */
        private void GoalSufferedCallback(object sender, EventArgs e)
        {
            Thread t = new Thread(delegate ()
            {
                
                SharedSettings settings = SharedSettings.GetInstance();
                if (settings.Connection != null)
                {
                    /*   Creo il pulsante per chuidere la finestra   */
                    float windowCenterX = parentWindow.Size.X / 2;
                    float windowCenterY = parentWindow.Size.Y / 2;
                    /*   Invio il messaggio di goal subito   */
                    SendAndReceive sendAndReceive = settings.sendAndReceive;
                    Message GoalSufferedMsg = new Message();
                    GoalSufferedMsg.Command = "g";
                    GoalSufferedMsg.Body = "";
                    GoalSufferedMsg.destinationIP = settings.Connection.OpponentIP;
                    sendAndReceive.SendMessage(GoalSufferedMsg);
                    /*   Palla in centro e si riparte   */
                    //Posiziono la pallina in centro
                    Ball.Speed = 300;
                    Ball.Position = new SFML.System.Vector2f(playgroundSize.X / 2, playgroundSize.Y / 2);
                    Random Rand = new Random();
                    Ball.Angle = Rand.NextDouble() * (Math.PI * 2);
                    //invio la nuova posizione e il nuovo angolo della pallina
                    Message updatePositionMsg = new Message();
                    updatePositionMsg.Command = "p";
                    string CommandParameters = Ball.Angle.ToString() + ";" + Ball.Speed.ToString() + ";" + Ball.Position.X.ToString() + ";" + Ball.Position.Y.ToString();
                    updatePositionMsg.Body = CommandParameters;
                    updatePositionMsg.destinationIP = settings.Connection.OpponentIP;
                    sendAndReceive.SendMessage(updatePositionMsg);
                    /*   Cambio il valore alla variabile che indica se è stato subito un goal   */
                    GoalSuffered = true;
                    //Incremento il valore dei punti dell'avversario
                    settings.Connection.OpponentsPoints++;
                    //Riproduco il suono del Goal subito
                    SoundPlayer soundPlayer = new SoundPlayer(settings.resourcesPath + "GoalSufferedSound.wav");
                    soundPlayer.Play();
                    Thread.Sleep(2000);
                    GoalSuffered = false;
                }
            });
            t.Start();
        }

        public void Draw()
        {
            try
            {
                SharedSettings settings = SharedSettings.GetInstance();
                /* DISEGNO IL CAMPO */
                //disegno il campo (rettangolo 500 * 800)
                float windowCenterX = parentWindow.Size.X / 2;
                float windowCenterY = parentWindow.Size.Y / 2;
                playground.Position = new SFML.System.Vector2f(windowCenterX - (playground.Size.X / 2), 50);
                parentWindow.Draw(playground);
                //disegno la mia porta
                myGoal.Position = new SFML.System.Vector2f(windowCenterX - (myGoal.Size.X / 2), playground.Position.Y + playground.Size.Y);
                parentWindow.Draw(myGoal);
                //disegno la porta dell'avversario
                opponentGoal.Position = new SFML.System.Vector2f(windowCenterX - (opponentGoal.Size.X / 2), playground.Position.Y - opponentGoal.Size.Y);
                parentWindow.Draw(opponentGoal);
                //disegno il cerchio in mezzo al campo
                CircleShape circle = new CircleShape();
                circle.Radius = 75;
                circle.FillColor = Color.Black;
                circle.OutlineColor = Color.White;
                circle.OutlineThickness = 1;
                circle.Position = new SFML.System.Vector2f(windowCenterX - circle.Radius, playground.Position.Y + (playground.Size.Y / 2) - circle.Radius);
                parentWindow.Draw(circle);
                //disegno la linea di metà campo
                RectangleShape halfWayLine = new RectangleShape();
                halfWayLine.Size = new SFML.System.Vector2f(playgroundSize.X, 2);
                halfWayLine.FillColor = Color.White;
                halfWayLine.Position = new SFML.System.Vector2f(playground.Position.X, playground.Position.Y + (playground.Size.Y / 2) - 1);
                parentWindow.Draw(halfWayLine);
                //disegno il numero che indica i miei punti
                if (settings.Connection != null)
                {
                    Text myPointTxt = new Text(settings.Connection.myPoints.ToString(), settings.font);
                    myPointTxt.Position = new SFML.System.Vector2f(playground.Position.X + playground.Size.X + 10, playground.Position.Y + (playground.Size.Y / 2) + 10);

                    myPointTxt.CharacterSize = 30;
                    parentWindow.Draw(myPointTxt);
                    //disegno il numero che indica i punti dell'avversario
                    Text opponentPointTxt = new Text(settings.Connection.OpponentsPoints.ToString(), settings.font);
                    opponentPointTxt.Position = new SFML.System.Vector2f(playground.Position.X + playground.Size.X + 10, playground.Position.Y + (playground.Size.Y / 2) - 50);
                    opponentPointTxt.CharacterSize = 30;
                    parentWindow.Draw(opponentPointTxt);
                }
                //disegno la pallina
                Ball.playgroundPosition = playground.Position;
                Ball.DrawBall(playground.Position);
                //disegno la mia manopola
                settings.MyHandle.PlaygroundPosition = playground.Position;
                settings.MyHandle.Draw();
                //disegno la manopola dell'avversario
                settings.opponentHandle.PlaygroundPosition = playground.Position;
                settings.opponentHandle.Draw();
                if (GoalSuffered)
                {
                    DrawGoalSuffered();
                }
                if (GoalScored)
                {
                    DrawGoalScored();
                }
            }catch (Exception ex) { }
        }

        /*   Metodo per disegnare la notifica quando viene subito un goal   */
        public void DrawGoalSuffered()
        {
            SharedSettings settings = SharedSettings.GetInstance();
            //Disegno il rettangolo che contiene tutto (250 * 500)
            float windowCenterX = parentWindow.Size.X / 2;
            float windowCenterY = parentWindow.Size.Y / 2;
            RectangleShape container = new RectangleShape();
            container.Size = new SFML.System.Vector2f(150, 80);
            container.Position = new SFML.System.Vector2f(playgroundSize.X + playground.Position.X + 5, playground.Position.Y + 50);
            container.FillColor = Color.Black;
            container.OutlineColor = Color.Red;
            container.OutlineThickness = 2;
            parentWindow.Draw(container);
            //Disegno il testo che indica che ho subito un goal
            Text text = new Text("Hai subito\nGoal", settings.font);
            text.CharacterSize = 18;
            text.FillColor = Color.White;
            text.Position = new SFML.System.Vector2f(playgroundSize.X + playground.Position.X + 10, playground.Position.Y + 60);
            parentWindow.Draw(text);
            CheckPointsAndChangePage();
        }

        /*   Metodo per disegnare la notifica quando viene fatto un goal    */
        public void DrawGoalScored()
        {
            SharedSettings settings = SharedSettings.GetInstance();
            //Disegno il rettangolo che contiene tutto (250 * 500)
            float windowCenterX = parentWindow.Size.X / 2;
            float windowCenterY = parentWindow.Size.Y / 2;
            RectangleShape container = new RectangleShape();
            container.Size = new SFML.System.Vector2f(150, 80);
            container.Position = new SFML.System.Vector2f(playgroundSize.X + playground.Position.X + 5, playground.Position.Y + 50);
            container.FillColor = Color.Black;
            container.OutlineColor = Color.Green;
            container.OutlineThickness = 2;
            parentWindow.Draw(container);
            //Disegno il testo che indica che ho subito un goal
            Text text = new Text("Hai fatto\nGoal!", settings.font);
            text.CharacterSize = 18;
            text.FillColor = Color.White;
            text.Position = new SFML.System.Vector2f(playgroundSize.X + playground.Position.X + 10, playground.Position.Y + 60);
            parentWindow.Draw(text);
            CheckPointsAndChangePage();
        }

        /* Metodo che controlla se si è raggiunto il massimo dei punti ed eventualmente cambia pagina */
        public void CheckPointsAndChangePage()
        {
            SharedSettings settings = SharedSettings.GetInstance();
            int maxPoints = 10;
            if(settings.Connection.myPoints >= maxPoints || settings.Connection.OpponentsPoints >= maxPoints)
            {
                settings.windowManager.PageDisplayed = WindowManager.GameFinishPage;
            }
        }
    }
}
