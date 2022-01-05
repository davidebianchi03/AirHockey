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
            
            if (e.message.Command != null)
            {
                //controllo che il comando sia quello della connessione
                if (e.message.Command == "m")
                {
                    //Se il comando è quello di richiesta della connessione
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
