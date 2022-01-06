using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
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
        private UIButton CloseGoalSufferedWindow = null;
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
        }
        
        /*  Metodo che viene richiamato dall'evento del goal subito  */
        private void GoalSufferedCallback(object sender, EventArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if (!GoalSuffered)
            {
                /*   Creo il pulsante per chuidere la finestra   */
                float windowCenterX = parentWindow.Size.X / 2;
                float windowCenterY = parentWindow.Size.Y / 2;
                CloseGoalSufferedWindow = null;
                CloseGoalSufferedWindow = new UIButton(parentWindow, "Chiudi", settings.font);
                CloseGoalSufferedWindow.BorderThickness = 2;
                CloseGoalSufferedWindow.BorderColor = Color.White;
                CloseGoalSufferedWindow.textColor = Color.White;
                CloseGoalSufferedWindow.FillColor = Color.Black;
                CloseGoalSufferedWindow.Size = new SFML.System.Vector2f(200, 50);//-> da sistemare
                CloseGoalSufferedWindow.Position = new SFML.System.Vector2f(windowCenterX - (CloseGoalSufferedWindow.Size.X / 2), windowCenterY + 100);//-> da sistemare
                CloseGoalSufferedWindow.textSize = 18;
                CloseGoalSufferedWindow.ButtonPressed += CloseGoalSufferedWindowCallback;
                /*   Invio il messaggio di goal subito   */
                SendAndReceive sendAndReceive = settings.sendAndReceive;
                Message GoalSufferedMsg = new Message();
                GoalSufferedMsg.Command = "g";
                GoalSufferedMsg.Body = "";
                GoalSufferedMsg.destinationIP = settings.Connection.OpponentIP;
                sendAndReceive.SendMessage(GoalSufferedMsg);
                /*   Cambio il valore alla variabile che indica se è stato subito un goal   */
                GoalSuffered = true;
                //Incremento il valore dei punti dell'avversario
                settings.Connection.OpponentsPoints++;
            }
        }

        private void CloseGoalSufferedWindowCallback(object sender, EventArgs e)
        {
            GoalSuffered = false;
            //Invio il messaggio con posizione e angolo random della pallina
            SharedSettings settings = SharedSettings.GetInstance();
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            Message updatePositionMsg = new Message();
            updatePositionMsg.Command = "p";
            string CommandParameters = Ball.Angle.ToString() + ";" + Ball.Speed.ToString() + ";" + Ball.Position.X.ToString() + ";" + Ball.Position.Y.ToString();
            updatePositionMsg.Body = CommandParameters;
            updatePositionMsg.destinationIP = settings.Connection.OpponentIP;
            sendAndReceive.SendMessage(updatePositionMsg);
            /*   Disabilito il pulsante   */
            CloseGoalSufferedWindow.Enable = false;
        }

        public void Draw()
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
            Text myPointTxt = new Text(settings.Connection.myPoints.ToString(), settings.font);
            myPointTxt.Position = new SFML.System.Vector2f(playground.Position.X + playground.Size.X + 10, playground.Position.Y + (playground.Size.Y / 2) + 10);
            
            myPointTxt.CharacterSize = 30;
            parentWindow.Draw(myPointTxt);
            //disegno il numero che indica i punti dell'avversario
            Text opponentPointTxt = new Text(settings.Connection.OpponentsPoints.ToString(), settings.font);
            opponentPointTxt.Position = new SFML.System.Vector2f(playground.Position.X + playground.Size.X + 10, playground.Position.Y + (playground.Size.Y / 2) - 50);
            opponentPointTxt.CharacterSize = 30;
            parentWindow.Draw(opponentPointTxt);
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
        }

        /*   Metodo per disegnare la schermata quando viene subito un goal   */
        void DrawGoalSuffered()
        {
            //Posiziono la pallina in centro
            Ball.Speed = 300;
            Ball.Position = new SFML.System.Vector2f(playgroundSize.X / 2, playgroundSize.Y / 2);
            Random Rand = new Random();
            Ball.Angle = Rand.NextDouble() * (Math.PI * 2);
            SharedSettings settings = SharedSettings.GetInstance();
            //Disegno il rettangolo che contiene tutto (250 * 500)
            float windowCenterX = parentWindow.Size.X / 2;
            float windowCenterY = parentWindow.Size.Y / 2;
            RectangleShape container = new RectangleShape();
            container.Size = new SFML.System.Vector2f(250, 500);
            container.Position = new SFML.System.Vector2f(windowCenterX - (container.Size.X / 2), windowCenterY - (container.Size.Y / 2));
            container.FillColor = Color.Black;
            container.OutlineColor = Color.Red;
            container.OutlineThickness = 2;
            parentWindow.Draw(container);
            //Disegno il testo che indica che ho subito un goal
            Text text = new Text("Hai subito\nGoal", settings.font);
            text.CharacterSize = 18;
            text.FillColor = Color.White;
            text.Position = new SFML.System.Vector2f(windowCenterX - (text.GetGlobalBounds().Width / 2), windowCenterY - 10);
            parentWindow.Draw(text);
            //Disegno il pulsante per chiudere la finestra
            CloseGoalSufferedWindow.draw();
        }
    }
}
