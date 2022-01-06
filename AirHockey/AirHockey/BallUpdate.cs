using System;
using System.Collections.Generic;
using System.Text;

namespace AirHockey
{
    class BallUpdate
    {
        /*
            Classe che riceve la posizione della pallina dall'altro host quando viene colpita e aggiorna i valori 
        */
        private Ball Ball;

        public BallUpdate(Ball Ball)
        {
            this.Ball = Ball;
            SharedSettings settings = SharedSettings.GetInstance();
            settings.sendAndReceive.MessageReceived += MessageReceivedCallback;
        }

        private void MessageReceivedCallback(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if (e.message.Command != null)
            {
                //controllo che il comando sia quello della direzione cambiata e che l'ip sia corretto
                if (e.message.Command == "p" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
                {
                    //Se il comando è quello della direzione cambiata
                    //salvo le coordinate che mi sono state mandate
                    string[] fields = e.message.Body.Split(';');
                    float newAngle = float.Parse(fields[0]);
                    float newSpeed = float.Parse(fields[1]);
                    float newX = Math.Abs(float.Parse(fields[2]) - Ball.playgroundSize.X);
                    float newY = Math.Abs(Ball.playgroundSize.Y - float.Parse(fields[3]));
                    Ball.Angle = newAngle + Math.PI;
                    Ball.Speed = newSpeed;
                    Ball.Position = new SFML.System.Vector2f(newX, newY);

                }

            }
        }
    }
}
