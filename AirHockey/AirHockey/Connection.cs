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
        public bool IEstablish { get; set; } = false;//indica chi ha richiesto la connessione (true -> la ho richiesta io)
        public int myPoints { get; set; } = 0;
        public int OpponentsPoints { get; set; } = 0;
        public Connection()
        {

        }
    }
}
