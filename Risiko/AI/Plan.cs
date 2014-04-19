using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class Plan
    {
        //Plan eines Spielers
        /// <summary>
        /// Index des Kontinents der erobert werden soll
        /// -1, wenn kein Kontinent teilweise besetzt
        /// </summary>
        internal int ContinentToConquer;
        /// <summary>
        /// Index des Spielers der vernichtet/geschwächt werden soll
        /// </summary>
        internal int PlayerToKill;
        /// <summary>
        /// Land das attackiert werden soll, falls nur ganze Kontinente in Besitz
        /// </summary>
        internal int CountryToAttack;

        /// <summary>
        /// Wichtigkeit Kontinent zu erobern
        /// </summary>
        internal double ContinentImportance;
        /// <summary>
        /// Wichtigkeit Spieler zu vernichten/schwächen
        /// </summary>
        internal double PlayerImportance;


        public Plan()
        {
            ContinentToConquer = -1;
            PlayerToKill = -1;
            CountryToAttack = -1;
        }
    }
}
