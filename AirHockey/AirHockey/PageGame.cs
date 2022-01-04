using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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
        public int myPoints { get; set; } = 0;//i miei punti
        public int opponentPoints { get; set; } = 0;//i punti dell'avversario
        public Ball Ball { get; set; }
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
            Text myPointTxt = new Text(myPoints.ToString(), settings.font);
            myPointTxt.Position = new SFML.System.Vector2f(playground.Position.X + playground.Size.X + 10, playground.Position.Y + (playground.Size.Y / 2) - 50);
            myPointTxt.CharacterSize = 30;
            parentWindow.Draw(myPointTxt);
            //disegno il numero che indica i punti dell'avversario
            Text opponentPointTxt = new Text(opponentPoints.ToString(), settings.font);
            opponentPointTxt.Position = new SFML.System.Vector2f(playground.Position.X + playground.Size.X + 10, playground.Position.Y + (playground.Size.Y / 2) + 10);
            opponentPointTxt.CharacterSize = 30;
            parentWindow.Draw(opponentPointTxt);
            //disegno la pallina
            Ball.playgroundPosition = playground.Position;
            Ball.DrawBall(playground.Position);
        }
    }
}
