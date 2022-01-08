using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using window_utilities;

namespace AirHockey
{
    class HandleUpdate
    {
        private float x, y;
        private OpponentHandle o;


        public HandleUpdate(OpponentHandle o)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            sendAndReceive.MessageReceived += MessageReceived;
            this.o = o;
        }

        private void MessageReceived(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            if (e.message.Command != null)
            {
                //controllo che il comando sia quello della manopola spostata e che l'ip sia corretto
                if (settings.Connection != null && e.message.Command == "m" && e.message.sourceIP.Equals(settings.Connection.OpponentIP))
                {
                    //Se il comando è quello della manopola spostata
                    //salvo le coordinate che mi sono state mandate
                    string[] coordinate = e.message.Body.Split(';');
                    x = float.Parse(coordinate[0]);
                    y = float.Parse(coordinate[1]);

                    //inverto x e y

                    y = Math.Abs(y - o.PlaygroundSize.Y);
                    x = Math.Abs(o.PlaygroundSize.X - x);

                    o.Position = new SFML.System.Vector2f(x,y);
                }
                
            }
        }
    }
}
