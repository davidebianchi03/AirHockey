using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AirHockey
{
    class Connection
    {
        /*
            Classe che contiene tutti i dati riguardanti la connessione in corso
        */

        public IPAddress OpponentIP { get; set; } = null;//Indirizzo ip dell'host dell'avversario
        public string OpponentUsername { get; set; } = "";//Username dell'avversario

        public Connection()
        {

        }
    }
}
