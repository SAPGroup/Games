using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AIContinent
    {
        internal Continent ThisContinent;

        //AIContinent values
        /// <summary>
        /// Effektivität des Kontinents
        /// E = zusätzliche Einheiten / angrezende Länder
        /// </summary>
        internal double Effectiveness;
        /// <summary>
        /// Wert zu wie viel Prozent Kontinent dem Aktuellen Spieler gehört
        /// </summary>
        internal double OwnedPercentage;


        //use this
        public AIContinent(Continent ContIn)
        {
            ThisContinent = ContIn;
        }
        //normal constructor
        public AIContinent()
        {
            
        }

    }
}
