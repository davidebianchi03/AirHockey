using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AirHockey
{
    /*
        Classe contenente i dati di un messaggio 
    */
    public class Message
    {
        public IPAddress sourceIP { get; set; }//Indirizzo IP sorgente del messaggio
        public IPAddress destinationIP { get; set; }//Indirizzo IP del destinatario del messaggio
        public string Command { get; set; }//Comando contenuto nel messaggio
        public string Body { get; set; }//Corpo del messaggio

        public Message()
        {
            sourceIP = null;
            destinationIP = null;
            Command = "";
            Body = "";
        }

        /*   Metodo per creare un nuovo messaggio in base alla stringa in formato csv ricevuta   */
        public static Message CreateFromCsv(IPAddress sourceIP, IPAddress destinationIP, string message)
        {
            Message msg = new Message();
            //inserisco gli indirizzi ip nel messaggio
            msg.destinationIP = destinationIP;
            msg.sourceIP = sourceIP;

            /*  faccio il parsing del contenuto del messaggio   */
            //prendo il comando
            int separatorPosition = message.IndexOf(';');
            string command = message.Substring(0, separatorPosition);
            msg.Command = command;
            //prendo il corpo del messaggio
            string body = message.Substring(separatorPosition + 1);
            msg.Body = body;

            return msg;
        }

        /*   Metodo per trasformare l'oggetto in una stringa in formato CSV   */
        public string ToCSV()
        {
            return Command + ";" + Body;
        }


    }
}
