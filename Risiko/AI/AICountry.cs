using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AICountry
    {
        // Land selbst
        internal Country ThisCountry;
        //AI - Properties
        

        //Calculated
        /// <summary>
        /// Prozentsatz, wenn zu groß verstärkt Einheiten einsetzen
        /// Relation zwischen Einheiten in Land und potentiell angreifenden Einheiten
        /// </summary>
        internal double DefendValue = 0.0;
        /// <summary>
        ///  Bei gegnerischen Ländern, um festzulegen wie "stark"
        ///  Drang anzugreifen ist
        /// </summary>
        internal double AttackValue = 0.0;
        /// <summary>
        /// Wert ob Land feindliche Nachbarländer hat
        /// false = keine feindlichen Nachbarländer
        /// true = schon
        /// </summary>
        internal bool IsBorderCountryForPlayer = false;
        /// <summary>
        /// Legt Wichtigkeit des Landes für Verteidigung fest
        /// falls keine Nachbarländer = 0.0
        /// </summary>
        internal double DefenceImportance = 0.0;


        // Constructor
        public AICountry(Country CountryIn)
        {
            ThisCountry = CountryIn;
        }
        // normal constructor
        public AICountry()
        {
            
        }

        //Sonstiges
        /// <summary>
        /// Setzt erneut die geerbten Werte auf die von CountryIn
        /// zum Aktualisieren nötig
        /// </summary>
        /// <param name="CountryIn"></param>
        public void ActualizeWithNormalCountry(Country CountryIn)
        {
            ThisCountry = CountryIn;
        }
    }
}
