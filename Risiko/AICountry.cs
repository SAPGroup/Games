using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AICountry:Country
    {
        // properties
        // scceeded
        // name, owner, unitsstationed, neighbouringcountries, continent

        /// <summary>
        /// Bei eigenen Ländern, wie stark verteidigt werden soll
        /// </summary>
        internal double DefendValue = 0.0;
        /// <summary>
        ///  Bei gegnerischen Ländern, um festzulegen wie "stark"
        ///  Drang anzugreifen ist
        /// </summary>
        internal double AttackValue = 0.0;
        // Constructor
        public AICountry(Country CountryIn)
        {
            Name = CountryIn.name;
            Owner = CountryIn.owner;
            UnitsStationed = CountryIn.unitsStationed;
            NeighbouringCountries = CountryIn.neighbouringCountries;
            Continent = CountryIn.continent;
        }

        // normal constructor
        public AICountry()
        {
            
        }

        /// <summary>
        /// Setzt erneut die geerbten Werte auf die von CountryIn
        /// zum Aktualisieren nötig
        /// </summary>
        /// <param name="CountryIn"></param>
        public void ActualizeWithNormalCountry(Country CountryIn)
        {
            Name = CountryIn.name;
            Owner = CountryIn.owner;
            UnitsStationed = CountryIn.unitsStationed;
            NeighbouringCountries = CountryIn.neighbouringCountries;
            Continent = CountryIn.continent;
        }
        
    }
}
