using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AirHockey
{
    class SendAndReceive
    {
        /*
         Classe che serve per poter inviare e ricevere i messaggi dall'altro peer
        */

        private UdpClient udpClient = null;
        private Thread waitFormMessage = null;//thread che ascolta i messaggi in arrivo

        public SendAndReceive()
        {
            SharedSettings sharedSettings = SharedSettings.GetInstance();
            udpClient = new UdpClient(sharedSettings.PortNumber);
            waitFormMessage = new Thread(ListenForMessages);
            waitFormMessage.Start();
        }

        /*   Metodo eseguito dal thread che ascolta i messaggi in arrivo   */
        public void ListenForMessages()
        {
            SharedSettings settings = SharedSettings.GetInstance();
            while (settings.windowManager.window == null || settings.windowManager.window.IsOpen)
            {
                IPEndPoint receiveIP = new IPEndPoint(IPAddress.Any, settings.PortNumber);
                try
                {
                    byte[] dataReceived = udpClient.Receive(ref receiveIP);
                    string stringReceived = Encoding.ASCII.GetString(dataReceived);
                    Message msg = Message.CreateFromCsv(receiveIP.Address, null, stringReceived);
                    MessageReceivedArgs args = new MessageReceivedArgs();
                    args.message = msg;
                    MessageReceived?.Invoke(this, args);
                }
                catch (Exception ex) { }
            }
        }

        /*   Metodo per interrompere l'ascolto dei messaggi   */
        public void StopListening()
        {
            udpClient.Close();
        }

        /*   Metodo per inviare un messaggio   */
        public void SendMessage(Message message)
        {
            SharedSettings settings = SharedSettings.GetInstance();
            byte[] dataToSend = Encoding.ASCII.GetBytes(message.ToCSV());
            udpClient.Send(dataToSend, dataToSend.Length, message.destinationIP.ToString(), settings.PortNumber);
        }

        /*   Evento che viene richiamato quando viene ricevuto un messaggio   */
        public event EventHandler<MessageReceivedArgs> MessageReceived;

    }

    public class MessageReceivedArgs : EventArgs
    {
        public Message message { get; set; }
    }

}
