using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using window_utilities;

namespace AirHockey
{
    class HandleUpdate
    {
        private int x, y;

        private void MessageReceived(object sender, MessageReceivedArgs e)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            SendAndReceive sendAndReceive = settings.sendAndReceive;
            if (e.message.Command != null)
            {
                //controllo che il comando sia quello della connessione
                if (e.message.Command == "m")
                {
                    //Se il comando è quello di richiesta della connessione
                    //salvo le coordinate che mi sono state mandate
                    string[] coordinate = e.message.Body.Split(';');
                    x = Int32.Parse(coordinate[0]);
                    y = Int32.Parse(coordinate[1]);
                }
                
            }
        }
    }
}
